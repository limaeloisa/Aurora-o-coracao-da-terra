using UnityEngine;
using System.Collections;

public class Inimigo : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int vida = 50;

    [Header("Feedback Visual")]
    public float tempoPiscar = 0.1f;

    [Header("Empurrão ao levar dano")]
    public float forcaEmpurrao = 2f;

    [Header("Movimentação")]
    public float velocidade = 2f;
    public Transform pontoEsquerdo;
    public Transform pontoDireito;
    private bool indoDireita = true;

    [Header("Ataque à Distância")]
    public GameObject projetilPrefab;
    public Transform pontoDeDisparo;
    public float velocidadeTiro = 7f;
    public float tempoEntreTiros = 2f;
    public float distanciaParaAtirar = 8f;
    private float ultimoTiro;

    private SpriteRenderer sprite;
    private Color corOriginal;
    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (sprite != null)
            corOriginal = sprite.color;
    }

    void Update()
    {
        Patrulhar();
        VerificarAtirar();
    }

    // 🔹 Movimento de patrulha entre dois pontos
    void Patrulhar()
    {
        if (pontoEsquerdo == null || pontoDireito == null || rb == null) return;

        if (anim != null)
            anim.SetBool("walk", true);

        if (indoDireita)
        {
            rb.velocity = new Vector2(velocidade, rb.velocity.y);
            transform.eulerAngles = new Vector3(0f, 0f, 0f);

            if (transform.position.x >= pontoDireito.position.x)
                indoDireita = false;
        }
        else
        {
            rb.velocity = new Vector2(-velocidade, rb.velocity.y);
            transform.eulerAngles = new Vector3(0f, 180f, 0f);

            if (transform.position.x <= pontoEsquerdo.position.x)
                indoDireita = true;
        }
    }

    // 🔹 Verifica se o player está perto para atirar
    void VerificarAtirar()
    {
        if (player == null || projetilPrefab == null || pontoDeDisparo == null) return;

        float distancia = Vector2.Distance(transform.position, player.position);

        if (distancia <= distanciaParaAtirar && Time.time > ultimoTiro + tempoEntreTiros)
        {
            ultimoTiro = Time.time;
            Atirar();
        }
    }

    // 🔹 Cria o projetil e dispara
    void Atirar()
    {
        if (anim != null)
            anim.SetTrigger("attack");

        GameObject projetil = Instantiate(projetilPrefab, pontoDeDisparo.position, Quaternion.identity);

        float direcao = (transform.eulerAngles.y == 180f) ? -1f : 1f;

        Rigidbody2D rbProj = projetil.GetComponent<Rigidbody2D>();
        if (rbProj != null)
            rbProj.velocity = new Vector2(direcao * velocidadeTiro, 0f);

        // vira o tiro conforme a direção
        Vector3 escala = projetil.transform.localScale;
        escala.x = Mathf.Abs(escala.x) * direcao;
        projetil.transform.localScale = escala;

        Destroy(projetil, 3f);
    }

    // 🔹 Leva dano e reage
    public void TomarDano(int dano)
    {
        vida -= dano;
        Debug.Log("Inimigo tomou dano. Vida restante: " + vida);

        StartCoroutine(PiscarVermelho());
        AplicarEmpurrao();

        if (vida <= 0)
        {
            Morrer();
        }
    }

    private IEnumerator PiscarVermelho()
    {
        if (sprite != null)
        {
            sprite.color = Color.red;
            yield return new WaitForSeconds(tempoPiscar);
            sprite.color = corOriginal;
        }
    }

    private void AplicarEmpurrao()
    {
        PlayerAttack player = FindObjectOfType<PlayerAttack>();
        if (rb != null && player != null)
        {
            Vector2 direcao = (transform.position - player.transform.position).normalized;
            rb.AddForce(direcao * forcaEmpurrao, ForceMode2D.Impulse);
        }
    }

    void Morrer()
    {
        if (anim != null)
            anim.SetTrigger("die");

        Destroy(gameObject, 0.5f);
    }

    // 🚫 Removido o dano por colisão direta
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Nada aqui — agora o player só toma dano de tiros
    }
}