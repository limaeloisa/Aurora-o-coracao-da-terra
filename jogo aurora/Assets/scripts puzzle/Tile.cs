using UnityEngine;

public class Tile : MonoBehaviour
{
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;

    private int tileId;
    private Color coluor;

    public void Init(GameManager gameManager, int tileId, Color coluor)
    {
        this.gameManager = gameManager;
        this.tileId = tileId;
        this.coluor = coluor;
        spriteRenderer = GetComponent<SpriteRenderer>();
        TurnOff();
    }

    public void TurnOff()
    {
        spriteRenderer.color = coluor * 0.3f;
    }

    public void TurnOn()
    {
        spriteRenderer.color = coluor;
    }

    private void OnMouseDown()
    {
        
    }
 
}
