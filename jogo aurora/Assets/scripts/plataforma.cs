using UnityEngine;

public class plataforma : MonoBehaviour
{

    private bool moveDown = true;


    public float velocidade = 3f;
    public Transform pontoA;
    public Transform pontoB;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > pontoA.position.y)
        {
            moveDown = true;
        }

        if (transform.position.y < pontoB.position.y)
        {
            moveDown = false;
        }

        if (moveDown)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y - velocidade * Time.deltaTime);
            
        }
        else
        {
            transform.position =
                new Vector2(transform.position.x, transform.position.y + velocidade * Time.deltaTime);
        }
    }
}
