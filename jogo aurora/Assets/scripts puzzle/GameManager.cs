using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private GameObject playButton;
    

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
    private List<int> leveltales;
    private int currentIndex = 0;

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
         if (gameMode == GameMode.Playing)
         {


             StartCoroutine(FlashTiles(index));
             if (index == leveltales[currentIndex])
             {
                 PlayTone(index);
                 currentIndex++;
                 if (currentIndex == leveltales.Count)
                 {
                     leveltales.Add(Random.Range(0, numTiles));
                     StartCoroutine(PlaySequence());
                 }

             }
             else
             {
                 Debug.LogFormat($"you got to level{leveltales.Count -2}");
                 gameMode = GameMode.Menu;
                 playButton.SetActive(true);
                 PlayErrorTone();

             }

         }
     }

     private void PlayErrorTone()
     {
         audioSource.pitch = 0.5f;
         double currentTime = AudioSettings.dspTime;
         audioSource.PlayScheduled(currentTime);
         audioSource.SetScheduledEndTime(currentTime + 3 * duration);
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

     public void Play()
     {
         playButton.SetActive(false);
         
         StopCoroutine(MenuTileAnimation());

         leveltales = new()
         {
             Random.Range(0, numTiles),
             Random.Range(0, numTiles),
             Random.Range(0, numTiles),
         };

         StartCoroutine(PlaySequence());
     }

     private IEnumerator PlaySequence()
     {
         gameMode = GameMode.Listeng;
         yield return new WaitForSeconds(2f);
         foreach (int index in leveltales)
         {
             PlayTone(index);
             yield return FlashTiles(index);
             yield return new WaitForSeconds(duration);
         }

         currentIndex = 0;
         gameMode = GameMode.Playing;
     }

}
