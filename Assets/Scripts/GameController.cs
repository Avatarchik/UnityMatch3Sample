using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Match3Core;

[RequireComponent(typeof(BoardSettings))]
[RequireComponent(typeof(BoardRenderer))]
public class GameController : MonoBehaviour
{
    public AudioClip scoreSound;
    public Pool pool;
    private int rowCount;
    private int columnCount;

    private float cellWidth;
    private float cellHeight;

    private bool isBlockSelected;
    private BlockPosition selectedBlockPosition;

    public new Camera camera;
    private Match3Game match3Game;
    private BoardRenderer boardRenderer;
    private float boardWidth;
    private float boardHeight;
    private IInput input;
    private bool canControl;

	void Start ()
	{
	    var boardSettings = GetComponent<BoardSettings>();
	    rowCount = boardSettings.rows;
	    columnCount = boardSettings.columns;
	    cellWidth = boardSettings.cellWidth;
        cellHeight = boardSettings.cellHeight;
        boardWidth = columnCount * cellWidth;
        boardHeight = rowCount * cellHeight;

        #if (UNITY_ANDROID || UNITY_IO || UNITY_WP8)
        input = new MobileInput();
        #endif

        #if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER)
        input = new StandaloneInput();
        #endif

	    if (camera == null)
	    {
	        camera = Camera.main;
	    }

        match3Game = new Match3Game(rowCount, columnCount, new HashSet<BlockColor> { BlockColor.Yellow, BlockColor.Blue, BlockColor.Orange, BlockColor.Green, BlockColor.Red, BlockColor.Purple });

	    boardRenderer = GetComponent<BoardRenderer>();
        boardRenderer.SetGame(match3Game);
	    boardRenderer.Render();

        StartCoroutine(MainLoop());
	}
	
	void Update () {

	    if (!canControl)
	    {
	        return;
	    }

        if (isBlockSelected && input.IsPointerHeld())
        {
            BlockPosition blockToSwapPosition;
            if (GetBlockPositionByMousePosition(out blockToSwapPosition))
            {
                var distanceBetweenRows = Mathf.Abs(selectedBlockPosition.row - blockToSwapPosition.row);
                var distanceBetweenColumns = Math.Abs(selectedBlockPosition.column - blockToSwapPosition.column);
                if ((distanceBetweenRows == 1 && distanceBetweenColumns == 0) || (distanceBetweenRows == 0 && distanceBetweenColumns == 1)) // можно соединять только соседние
                {
                    // try swap
                    var canSwapBlocks = match3Game.TrySwapBlocks(selectedBlockPosition.row, selectedBlockPosition.column, blockToSwapPosition.row, blockToSwapPosition.column);
                    var swapAnimation = boardRenderer.AnimateSwap(selectedBlockPosition.row, selectedBlockPosition.column, blockToSwapPosition.row, blockToSwapPosition.column, !canSwapBlocks);
                    if (canSwapBlocks)
                    {
                        swapAnimation.OnComplete(() =>
                        {
                            StartCoroutine(MainLoop());
                        });
                    }
                    isBlockSelected = false;
                }
            }
        }
        else if (input.IsPointerDown())
	    {       
            BlockPosition position;
	        if (GetBlockPositionByMousePosition(out position))
	        {
                isBlockSelected = true;
                selectedBlockPosition = position;
	        }
	    }
	    else if (input.IsPointerUp())
	    {
	        isBlockSelected = false;
	    }
	}

    private readonly List<SpecialBlockActivation> blockActivations = new List<SpecialBlockActivation>();
    private readonly List<BlockRemoval> removedBlocks = new List<BlockRemoval>();
    private readonly List<BlockAndPosition> createdBlocks = new List<BlockAndPosition>();

    IEnumerator MainLoop()
    {
        canControl = false;
        bool gravityUpdated;
        bool justShuffled;
        do
        {
            justShuffled = false;
            bool containsNewActivatedBlocks = match3Game.RemoveMatches(blockActivations, removedBlocks, createdBlocks);

            if (removedBlocks.Count > 0)
            {
                AudioSource.PlayClipAtPoint(scoreSound, Vector3.zero);
            }

            if (blockActivations.Count > 0 || removedBlocks.Count > 0)
            {
                var removal = boardRenderer.AnimateRemoval(blockActivations, removedBlocks);
                yield return removal.WaitForCompletion();
            }

            if (createdBlocks.Count > 0)
            {
                var creation = boardRenderer.AnimateCreation(createdBlocks);
                yield return creation.WaitForCompletion();
            }

            while (containsNewActivatedBlocks)
            {
                containsNewActivatedBlocks = match3Game.TryChainReaction(blockActivations, removedBlocks);

                if (blockActivations.Count > 0 || removedBlocks.Count > 0)
                {
                    var removal = boardRenderer.AnimateRemoval(blockActivations, removedBlocks);
                    yield return removal.WaitForCompletion();
                }
            }

            var activeBlocks = match3Game.GetMovingBlocks();
            var falling = boardRenderer.AnimateFalling(activeBlocks);
            yield return falling.WaitForCompletion();

            gravityUpdated = match3Game.UpdateGravity();
            if (!gravityUpdated)
            {
                while (!match3Game.HasAvailableActions())
                {
                    match3Game.Shuffle();
                    var shuffling = boardRenderer.AnimateShuffle();
                    yield return shuffling.WaitForCompletion();
                    justShuffled = true;
                }
            }
        }
        while (gravityUpdated || justShuffled);
        canControl = true;
    }

    bool GetBlockPositionByMousePosition(out BlockPosition position)
    {
        RaycastHit2D hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            var inverseTransformPoint = hit.collider.transform.InverseTransformPoint(hit.point);
            position = GetBoardCell(inverseTransformPoint);
            return true;
        }
        position = new BlockPosition(-1, -1);
        return false;
    }

    BlockPosition GetBoardCell(Vector2 localPosition)
    {
        float x = localPosition.x + boardWidth / 2;
        float y = boardHeight / 2 - localPosition.y;

        int row = Mathf.Clamp((int)(y / cellHeight), 0, rowCount - 1);
        int column = Mathf.Clamp((int) (x/cellWidth), 0, columnCount - 1);
        return new BlockPosition(row, column);
    }

    struct BlockPosition
    {
        public readonly int row;
        public readonly int column;

        public BlockPosition(int row, int column)
        {
            this.row = row;
            this.column = column;
        }

        public override string ToString()
        {
            return string.Format("Row: {0}, Column: {1}", row, column);
        }

    }
}
