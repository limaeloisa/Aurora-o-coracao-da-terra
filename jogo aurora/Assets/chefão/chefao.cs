using UnityEngine;

public class chefao : MonoBehaviour
{
    [Header("Vida do Inimigo")]
    public int vida = 50;

    [Header("Movimentação")]
    public float velocidade = 2f;
    public float distanciaDePerseguir = 10f;  // distancia para começar a perseguir
    public float distanciaDeAtaque = 8f;      // agora ele atira de longe

    [Header("Ataque Especial (Chuva de Fogo)")]
    public GameObject fireRainSpawnerPrefab;
    public float tempoEntreAtaques = 3f;
    private float ultimoAtaque = 0f;

    private Transform player;
    private Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Encontra o jogador automático
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
        else
            Debug.LogError("⚠ ERRO: Player não tem tag 'Player'");
    }

    void Update()
    {
        if (player == null) return;

        float distancia = Vector2.Distance(transform.position, player.position);

        // Se muito longe = idle
        if (distancia > distanciaDePerseguir)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("walk", false);
            return;
        }

        // Se perto o suficiente para atacar
        if (distancia <= distanciaDeAtaque)
        {
            Atacar();
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("walk", false);

            // Virar para o player
            if (player.position.x > transform.position.x)
                transform.eulerAngles = Vector3.zero;
            else
                transform.eulerAngles = new Vector3(0, 180, 0);

            return;
        }

        // Caso contrário → perseguir
        Perseguir();
    }

    void Perseguir()
    {
        anim.SetBool("walk", true);

        Vector2 direcao = (player.position - transform.position).normalized;

        rb.linearVelocity = new Vector2(direcao.x * velocidade, rb.linearVelocity.y);

        if (direcao.x > 0)
            transform.eulerAngles = new Vector3(0, 0, 0);
        else
            transform.eulerAngles = new Vector3(0, 180, 0);
    }

    void Atacar()
    {
        if (Time.time < ultimoAtaque + tempoEntreAtaques) return;

        ultimoAtaque = Time.time;

        anim.SetTrigger("attack");

        // Instancia chuva de fogo mirando no player
        Instantiate(fireRainSpawnerPrefab, player.position, Quaternion.identity);
    }

    public void TomarDano(int dano)
    {
        vida -= dano;

        if (vida <= 0)
            Destroy(gameObject);
    }
}