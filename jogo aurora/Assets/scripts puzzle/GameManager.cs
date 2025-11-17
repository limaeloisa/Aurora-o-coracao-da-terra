using System.Collections;
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

    [Header("Audio Setup")] [SerializeField]
    private float duration = 0.2f;
    [SerializeField] private AudioSource audioSource;

    enum GameMode
    {
        None,
        Menu,
        Listeng,
        Playing
    }
    
    private GameMode gameMode = GameMode.None;

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

        gameMode = GameMode.Menu;
        StartCoroutine(MenuTileAnimation());


     }

     private IEnumerator MenuTileAnimation()
     {
         while (gameMode == GameMode.Menu)
         {
             yield return FlashTiles(Random.Range(0, numTiles));
             yield return new WaitForSeconds(duration);
         }
        
     }
     
     private IEnumerator FlashTiles(int index)
     {
         tile[index].TurnOn();
         yield return new WaitForSeconds(duration);
         tile[index].TurnOff();
         
     }

     public void PlayLightAndTone(int index)
     {
         StartCoroutine(FlashTiles(index));
         PlayTone(index);
     }

     private void PlayTone(int index)
     {
         if (numTiles > 1)
         {
             audioSource.pitch = Mathf.Lerp(0.5f, 2.0f, index / (numTiles - 1f) );
         }
         
         double currenTime = AudioSettings.dspTime;
         audioSource.PlayScheduled(currenTime);
         audioSource.SetScheduledEndTime(currenTime + duration);
        
     }

}
