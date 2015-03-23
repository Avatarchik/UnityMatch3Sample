using UnityEngine;

[ExecuteInEditMode]
public class BoardSettings : MonoBehaviour
{
    [Range(3, 20)]
    public int rows = 9;
    [Range(3, 20)]
    public int columns = 9;

    [Range(0.00001f, 1000f)]
    public float cellWidth = 1f;
    [Range(0.00001f, 1000f)]
    public float cellHeight = 1f;

    public int scoreIncreaseNormalDestruction = 20;
    public int scoreIncreaseDestructionByBombExplosion = 30;
    public int scoreIncreaseDestructionByCrossExplosion = 50;
    public int scoreIncreaseDestructionByLaserBlock = 50;

    private int savedRows;
    private int savedColumns;

    private BoxCollider2D boardCollider;

	void Start ()
	{
	    boardCollider = GetComponent<BoxCollider2D>();
	}
	
	void Update () {
	    #if UNITY_EDITOR
	    if (savedRows != rows || savedColumns != columns)
	    {
            boardCollider.size = new Vector2(cellWidth * columns, cellHeight * rows);
	        savedRows = rows;
	        savedColumns = columns;
	    }
        #endif
	}
}
