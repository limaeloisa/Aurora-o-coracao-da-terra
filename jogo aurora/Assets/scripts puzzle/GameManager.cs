using UnityEngine;

public class GameManager : MonoBehaviour
{
   [Header("Game Setup")]
   [SerializeField] private int numRows = 3;
    [SerializeField] private int numCols = 4;
    private int numTiles;
    private Tile[] tile;

    [Header("Game Objects")]
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform gameArea;

     void Start()
     {
        numTiles = numRows * numCols;
        tile = new Tile[numTiles];

        for( int row = 0; row < numRows; row++)
        {
        for( int col = 0; col < numCols; col++)
        { 
          int index = (row * numCols) + col;

         
         tile[index] = Instantiate(tilePrefab, gameArea);
         tile[index].Init(this, index, Color.HSVToRGB((float)index / numTiles, 0.8f, 0.9f ));

         float rowStart = (numRows / 2f) - 0.5f;
         float colStart = (-numCols / 2f) + 0.5f;
         tile[index].transform.localPosition = new Vector3(colStart + col, rowStart - row, 0f);

         
        }

         

        }

        float scale = 6f / numRows;
        gameArea.localScale = Vector3.one * scale;

     }
}
