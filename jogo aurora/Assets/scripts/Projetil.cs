using UnityEngine;

public class Projetil : MonoBehaviour
{
    public int dano = 20;
    public float tempoDeVida = 2f;

    void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        // Só acerta inimigos
        if (outro.CompareTag("Inimigo"))
        {
            Inimigo inimigo = outro.GetComponent<Inimigo>();
            if (inimigo != null)
            {
                inimigo.TomarDano(dano);
            }

            // Aqui você pode tocar um efeito de impacto
            Destroy(gameObject);
        }
    }
}