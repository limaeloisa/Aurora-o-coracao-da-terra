using UnityEngine;

public class Coletavel : MonoBehaviour
{
    public LevelProgression gerenciador; // Referência ao script principal

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gerenciador.AdicionarColetavel();
            Destroy(gameObject); // Remove o item da cena
        }
    }
}
