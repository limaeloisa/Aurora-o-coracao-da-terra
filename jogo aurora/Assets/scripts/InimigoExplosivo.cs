using UnityEngine;
using System.Collections;

public class InimigoExplosivo : MonoBehaviour
{
    [Header("Movimentação")]
    public float velocidade = 3f;            // velocidade de voo
    public float distanciaExplosao = 1.5f;   // distância mínima para explodir
    public float alturaFlutuar = 1.2f;       // variação de altura no voo
    public float raioPerseguicao = 6f;       // distância máxima de perseguição

    [Header("Explosão")]
    public int dano = 1;                     // quanto dano dá (1 coração)
    public float raioExplosao = 2f;          // área de dano
    public float delayAntesExplodir = 0.3f;  // tempo piscando antes da explosão
    public GameObject efeitoExplosaoPrefab;  // partícula de explosão

    private Transform jogador;
    private bool explodindo = false;
    private SpriteRenderer sprite;
    private Color corOriginal;
    private float tempoSeno;                 // para efeito de flutuação
    private Vector2 posicaoInicial;          // posição original para retornar

    void Start()
    {
        jogador = GameObject.FindGameObjectWithTag("Player")?.transform;
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
            corOriginal = sprite.color;

        posicaoInicial = transform.position; // guarda posição inicial
    }

    void Update()
    {
        if (explodindo || jogador == null) return;

        float distanciaDoJogador = Vector2.Distance(transform.position, jogador.position);
        float distanciaDaBase = Vector2.Distance(transform.position, posicaoInicial);

        // --- Se o jogador está dentro do raio de perseguição ---
        if (distanciaDoJogador <= raioPerseguicao)
        {
            // Persegue o jogador
            Vector2 direcao = (jogador.position - transform.position).normalized;
            transform.position += (Vector3)direcao * velocidade * Time.deltaTime;
        }
        else
        {
            // Volta para a posição inicial se o jogador sair do raio
            if (distanciaDaBase > 0.1f)
            {
                Vector2 direcao = (posicaoInicial - (Vector2)transform.position).normalized;
                transform.position += (Vector3)direcao * (velocidade * 0.5f) * Time.deltaTime;
            }
        }

        // Efeito de flutuação (sobe e desce)
        tempoSeno += Time.deltaTime * 3f;
        transform.position += new Vector3(0, Mathf.Sin(tempoSeno) * alturaFlutuar * Time.deltaTime, 0);

        // --- Explode se o jogador estiver perto ---
        if (distanciaDoJogador <= distanciaExplosao)
        {
            StartCoroutine(Explodir());
        }
    }

    IEnumerator Explodir()
    {
        explodindo = true;

        // Pisca vermelho antes de explodir
        float t = 0;
        while (t < delayAntesExplodir)
        {
            if (sprite != null)
                sprite.color = sprite.color == Color.red ? corOriginal : Color.red;

            yield return new WaitForSeconds(0.1f);
            t += 0.1f;
        }

        // Instancia o efeito visual da explosão
        if (efeitoExplosaoPrefab != null)
            Instantiate(efeitoExplosaoPrefab, transform.position, Quaternion.identity);

        // Dano em tudo dentro do raio
        Collider2D[] atingidos = Physics2D.OverlapCircleAll(transform.position, raioExplosao);
        foreach (Collider2D col in atingidos)
        {
            if (col.CompareTag("Player"))
            {
                PlayerDamage player = col.GetComponent<PlayerDamage>();
                if (player != null)
                {
                    // 🔹 Agora usa o dano que NÃO faz respawn
                    player.TakeExplosionDamage(dano);
                }
            }
        }

        Destroy(gameObject); // remove o inimigo
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; // raio de perseguição
        Gizmos.DrawWireSphere(transform.position, raioPerseguicao);

        Gizmos.color = Color.red; // raio da explosão
        Gizmos.DrawWireSphere(transform.position, raioExplosao);
    }
}