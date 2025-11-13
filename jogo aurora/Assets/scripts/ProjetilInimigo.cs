using UnityEngine;

public class ProjetilInimigo : MonoBehaviour
{
    public int dano = 1;
    public float tempoDeVida = 3f;

    void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("Player"))
        {
            PlayerDamage player = outro.GetComponent<PlayerDamage>();
            if (player != null)
            {
                player.TakeExplosionDamage(dano);
            }

            Destroy(gameObject);
        }
        else if (outro.CompareTag("chão") || outro.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}