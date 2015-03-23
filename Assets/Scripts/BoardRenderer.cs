using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Match3Core;

// ReSharper disable once CheckNamespace
public class BoardRenderer : MonoBehaviour
{
    public delegate void LaserShotHandler();
    public static event LaserShotHandler LaserShot;

    public Pool pool;
    public float swapAnimationDuration = 0.2f;
    public float lightningAnimationDuration = 0.5f;
    public float disappearanceAnimationDuration = 0.3f;
    public float laserAnimationDuration = 0.5f;
    public float fallingAnimationDurationByBlock = 0.1f;
    public float shuffleAnimationDuration = 0.5f;
    public float creationAnimationDuration = 0.2f;

    private readonly Dictionary<int, GameObject> blockObjects = new Dictionary<int, GameObject>();
    private Match3Game match3Game;
    private float cellWidth;
    private float cellHeight;
    private int rowCount;
    private int columnCount;
    private int scoreIncreaseNormalDestruction;
    private int scoreIncreaseDestructionByBombExplosion;
    private int scoreIncreaseDestructionByCrossExplosion;
    private int scoreIncreaseDestructionByLaserBlock;
    private string scoreIncreaseNormalDestructionString;
    private string scoreIncreaseDestructionByBombExplosionString;
    private string scoreIncreaseDestructionByCrossExplosionString;
    private string scoreIncreaseDestructionByLaserBlockString;

    public delegate void BlockRemovedHandler(int score);

    public static event BlockRemovedHandler OnScoreIncreased = delegate { };

    public void SetGame(Match3Game game)
    {
        match3Game = game;
        rowCount = game.RowCount;
        columnCount = game.ColumnCount;
    }

    public void Awake()
    {
        var boardSettings = GetComponent<BoardSettings>();
        cellWidth = boardSettings.cellWidth;
        cellHeight = boardSettings.cellHeight;
        scoreIncreaseNormalDestruction = boardSettings.scoreIncreaseNormalDestruction;
        scoreIncreaseDestructionByBombExplosion = boardSettings.scoreIncreaseDestructionByBombExplosion;
        scoreIncreaseDestructionByCrossExplosion = boardSettings.scoreIncreaseDestructionByCrossExplosion;
        scoreIncreaseDestructionByLaserBlock = boardSettings.scoreIncreaseDestructionByLaserBlock;
        scoreIncreaseNormalDestructionString = Convert.ToString(scoreIncreaseNormalDestruction);
        scoreIncreaseDestructionByBombExplosionString = Convert.ToString(scoreIncreaseDestructionByBombExplosion);
        scoreIncreaseDestructionByCrossExplosionString = Convert.ToString(scoreIncreaseDestructionByCrossExplosion);
        scoreIncreaseDestructionByLaserBlockString = Convert.ToString(scoreIncreaseDestructionByLaserBlock);
    }

    public void Render()
    {
        for (int row = 0; row < rowCount; row++)
        {
            for (int column = 0; column < columnCount; column++)
            {
                var block = match3Game[row, column];
                if (block.color == BlockColor.Empty)
                {
                    continue;
                }

                var blockObj = GetOrCreateBlockGameObject(block);
                blockObj.transform.localPosition = GetLocalPosition(row, column);
            }
        }
    }

    private Vector3 GetLocalPosition(int row, int column)
    {
        return new Vector3(column - (columnCount - 1)/2f*cellWidth, (rowCount - 1)/2f*cellHeight - row);
    }

    private GameObject GetOrCreateBlockGameObject(Block block)
    {
        GameObject blockObj;
        if (!blockObjects.TryGetValue(block.id, out blockObj))
        {
            blockObj = pool.GetBlock(block.color, block.type);
            blockObj.SetActive(true);
            blockObjects.Add(block.id, blockObj);
        }
        return blockObj;
    }

    private GameObject GetBlockGameObject(Block block)
    {
        return GetBlockGameObject(block.id);
    }

    private GameObject GetBlockGameObject(int id)
    {
        GameObject blockGameObject;
        blockObjects.TryGetValue(id, out blockGameObject);
        return blockGameObject;
    }

    public Tween AnimateSwap(int row0, int column0, int row1, int column1, bool pingPong)
    {
        var block0 = match3Game[row0, column0];
        var block1 = match3Game[row1, column1];
        var block0Obj = GetBlockGameObject(block0);
        var block1Obj = GetBlockGameObject(block1);
        var block0InitialPosition = block0Obj.transform.localPosition;
        var block1InitialPosition = block1Obj.transform.localPosition;

        var doMove0 = block0Obj.transform.DOLocalMove(block1InitialPosition, swapAnimationDuration);
        var doMove1 = block1Obj.transform.DOLocalMove(block0InitialPosition, swapAnimationDuration);

        Sequence sequence = DOTween.Sequence();
        sequence.InsertAtBegin(doMove0).InsertAtBegin(doMove1);

        if (pingPong)
        {
            var doMove0Reverse = block0Obj.transform.DOLocalMove(block0InitialPosition, swapAnimationDuration);
            var doMove1Reverse = block1Obj.transform.DOLocalMove(block1InitialPosition, swapAnimationDuration);
            Sequence reverseAnimation = DOTween.Sequence();
            reverseAnimation.InsertAtBegin(doMove0Reverse);
            reverseAnimation.InsertAtBegin(doMove1Reverse);
            sequence.Append(reverseAnimation);
        }

        return sequence;
    }

    public Tween AnimateRemoval(List<SpecialBlockActivation> blockActivations, List<BlockRemoval> removeBlocks)
    {
        Sequence removalSequence = DOTween.Sequence();
        Vector3 laserStartPosition = new Vector3();

        for (int i = 0; i < blockActivations.Count; i++)
        {
            var blockActiviation = blockActivations[i];
            if (blockActiviation.type == BlockType.Mega)
            {
                laserStartPosition = GetLocalPosition(blockActiviation.row, blockActiviation.column);
                var sequence = DOTween.Sequence().OnStart(() =>
                {
                    if (LaserShot != null)
                    {
                        LaserShot();
                    }
                });
                removalSequence.InsertAtBegin(sequence);
            }
            else if (blockActiviation.type == BlockType.Bomb)
            {
                var sequence = DOTween.Sequence().OnStart(() =>
                {
                    var explosionObj = pool.GetExplosion();

                    explosionObj.SetActive(true);
                    explosionObj.transform.localPosition = GetLocalPosition(blockActiviation.row, blockActiviation.column);
                    var animator = explosionObj.GetComponent<Animator>();
                    animator.Play("Explosion");
                });
                removalSequence.InsertAtBegin(sequence);
            }
            else if (blockActiviation.type == BlockType.Cross)
            {
                var temp = DOTween.Sequence().OnStart(() =>
                {
                    var localPosition = GetLocalPosition(blockActiviation.row, blockActiviation.column);

                    var verticalLightning = pool.GetLightning();

                    verticalLightning.SetActive(true);
                    verticalLightning.transform.localPosition = new Vector3(localPosition.x, 0, 0);
                    verticalLightning.GetComponent<LightningController>().StartAnimation(lightningAnimationDuration);

                    var horizontalLightning = pool.GetLightning();
                    horizontalLightning.SetActive(true);
                    horizontalLightning.transform.rotation = Quaternion.Euler(0, 0, 90);
                    horizontalLightning.transform.localPosition = new Vector3(0, localPosition.y, 0);
                    horizontalLightning.GetComponent<LightningController>().StartAnimation(lightningAnimationDuration);
                });
                removalSequence.InsertAtBegin(temp);
            }
        }

        for (int i = 0; i < removeBlocks.Count; i++)
        {
            var removedBlock = removeBlocks[i];
            if (removedBlock.block.type != BlockType.Normal)
            {
                Debug.Log("Remove type: " + removedBlock.block.type + " position: " + removedBlock.destinationRow + "," + removedBlock.destinationColumn + " reason: " +
                          removedBlock.removalReason);
            }

            var blockGameObject = GetBlockGameObject(removedBlock.block);
            if (blockGameObject == null)
            {
                continue;
            }

            blockObjects.Remove(removedBlock.block.id);

            switch (removedBlock.removalReason)
            {
                case BlockRemovalReason.DestroyedByMatching:
                {
                    var scaleTween = CreateDisappearanceTween(blockGameObject, scoreIncreaseNormalDestructionString);
                    OnScoreIncreased(scoreIncreaseNormalDestruction);
                    removalSequence.InsertAtBegin(scaleTween);
                    break;
                }
                case BlockRemovalReason.DestroyedByBombExplosion:
                {
                    var scaleTween = CreateDisappearanceTween(blockGameObject, scoreIncreaseDestructionByBombExplosionString);
                    OnScoreIncreased(scoreIncreaseDestructionByBombExplosion);
                    removalSequence.InsertAtBegin(scaleTween);
                    break;
                }
                case BlockRemovalReason.DestroyedByCrossExplosion:
                {
                    var scaleTween = CreateDisappearanceTween(blockGameObject, scoreIncreaseDestructionByCrossExplosionString);
                    OnScoreIncreased(scoreIncreaseDestructionByCrossExplosion);
                    removalSequence.InsertAtBegin(scaleTween);
                    break;
                }
                case BlockRemovalReason.DestroyedByLaserBurst:
                {
                    var scaleTween = blockGameObject.transform.DOScale(Vector3.zero, disappearanceAnimationDuration).OnComplete(() =>
                    {
                        blockGameObject.SetActive(false);
                        blockGameObject.transform.localScale = Vector3.one;
                    }).OnStart(() =>
                    {
                        var laser = pool.GetLaser();
                        laser.SetActive(true);
                        laser.GetComponent<LaserController>().Draw(laserStartPosition, blockGameObject.transform.localPosition, laserAnimationDuration);
                    }).OnComplete(() =>
                    {
                        var textPopup = pool.GetTextPopup();
                        textPopup.SetActive(true);
                        textPopup.GetComponent<PopupController>().ShowPopup(blockGameObject.transform.localPosition, scoreIncreaseDestructionByLaserBlockString);
                        OnScoreIncreased(scoreIncreaseDestructionByLaserBlock);
                    });
                    removalSequence.InsertAtBegin(scaleTween);
                    break;
                }
            }
        }
        return removalSequence;
    }

    private Tweener CreateDisappearanceTween(GameObject blockGameObject, string scoreText)
    {
        var scaleTween = blockGameObject.transform.DOScale(Vector3.zero, disappearanceAnimationDuration).OnComplete(() =>
        {
            blockGameObject.SetActive(false);
            blockGameObject.transform.localScale = Vector3.one;

            var textPopup = pool.GetTextPopup();
            textPopup.SetActive(true);
            textPopup.GetComponent<PopupController>().ShowPopup(blockGameObject.transform.localPosition, scoreText);
        });
        return scaleTween;
    }

    public Tween AnimateFalling(List<BlockAndMovement> activeBlocks)
    {
        Sequence fallingSequence = DOTween.Sequence();
        foreach (var activeBlock in activeBlocks)
        {
            var blockObj = GetOrCreateBlockGameObject(activeBlock.block);
            blockObj.transform.localPosition = GetLocalPosition(activeBlock.initialRow, activeBlock.initialColumn);
            var moveTween = blockObj.transform.DOLocalMove(GetLocalPosition(activeBlock.destionationRow, activeBlock.initialColumn),
                fallingAnimationDurationByBlock*(activeBlock.destionationRow - activeBlock.initialRow));
            moveTween.SetEase(Ease.InQuad);
            fallingSequence.InsertAtBegin(moveTween);
        }
        return fallingSequence;
    }

    public Tween AnimateShuffle()
    {
        Sequence shufflingSequence = DOTween.Sequence();
        for (int row = 0; row < match3Game.RowCount; row++)
        {
            for (int column = 0; column < match3Game.ColumnCount; column++)
            {
                var block = match3Game[row, column];
                if (block.color == BlockColor.Empty)
                {
                    continue;
                }

                var blockObj = GetOrCreateBlockGameObject(block);
                var moveTween = blockObj.transform.DOLocalMove(GetLocalPosition(row, column), shuffleAnimationDuration);
                shufflingSequence.InsertAtBegin(moveTween);
            }
        }
        return shufflingSequence;
    }

    public void AnimateMerging(List<BlockAndPosition> mergedBlocks)
    {
        for (int i = 0; i < mergedBlocks.Count; i++)
        {
            var blockAndPosition = mergedBlocks[i];
            var blockObj = GetOrCreateBlockGameObject(blockAndPosition.block);
            blockObj.transform.localPosition = GetLocalPosition(blockAndPosition.row, blockAndPosition.column);
        }
    }

    public Tween AnimateCreation(List<BlockAndPosition> createdBlocks)
    {
        Sequence creationSequence = DOTween.Sequence();
        for (int i = 0; i < createdBlocks.Count; i++)
        {
            var blockAndPosition = createdBlocks[i];
            var blockObj = GetOrCreateBlockGameObject(blockAndPosition.block);
            blockObj.transform.localPosition = GetLocalPosition(blockAndPosition.row, blockAndPosition.column);
            blockObj.transform.localScale = Vector3.zero;
            var tween = blockObj.transform.DOScale(Vector3.one, creationAnimationDuration);
            creationSequence.InsertAtBegin(tween);
        }
        return creationSequence;
    }
}