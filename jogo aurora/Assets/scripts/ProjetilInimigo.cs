using UnityEngine;

public class ProjetilInimigo : MonoBehaviour
{
    public int dano = 10;
    public float tempoDeVida = 3f;

    void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("Player"))
        {
            PlayerDamage pd = outro.GetComponent<PlayerDamage>();
            if (pd != null)
            {
                pd.TakeDamage(dano);
            }
            Destroy(gameObject);
        }
        else if (!outro.CompareTag("Inimigo")) // não destrói se colidir com o próprio inimigo
        {
            Destroy(gameObject);
        }
    }
}