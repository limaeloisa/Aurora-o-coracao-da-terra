using UnityEngine;

public class pvida : MonoBehaviour
{
    private SpriteRenderer sr;           // Controla a imagem da poção
    private CircleCollider2D circle;     // Detecta colisões do player

    [Header("Efeito visual da coleta")]
    public GameObject collected;         // Objeto com animação ou efeito padrão
    public GameObject particulaRoxaPrefab; // Prefab da partícula roxa

    [Header("Configuração da cura")]
    public int healAmount = 1;           // Quantidade de corações que a poção cura

    void Start()
    {
        // Guarda referências ao SpriteRenderer e Collider da poção
        sr = GetComponent<SpriteRenderer>();
        circle = GetComponent<CircleCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // Só executa se quem encostou tiver a tag "Player"
        if (collider.CompareTag("Player"))
        {
            // --- 🔹 Recupera vida do jogador ---
            LifeSystem life = FindObjectOfType<LifeSystem>();
            if (life != null)
            {
                life.vida += healAmount;
                if (life.vida > life.vidaMaxima)
                    life.vida = life.vidaMaxima;
            }

            // --- 🔹 Efeito visual da poção sumindo ---
            sr.enabled = false;            // esconde o sprite
            circle.enabled = false;        // desativa colisão
            collected.SetActive(true);     // ativa o efeito de coleta (ex: brilho)

            // --- 🔹 Cria a partícula roxa ---
            if (particulaRoxaPrefab != null)
            {
                // cria a partícula na posição da poção
                GameObject efeito = Instantiate(particulaRoxaPrefab, transform.position, Quaternion.identity);

                // destrói o efeito depois de 2 segundos (pra não acumular objetos)
                Destroy(efeito, 2f);
            }

            // --- 🔹 Destroi a poção após 0.3 segundos ---
            Destroy(gameObject, 0.3f);
        }
    }
}