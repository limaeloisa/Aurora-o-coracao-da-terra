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

    [Header("Movimentação e perseguição")]
    public float velocidade = 2f;
    public float distanciaParaPerseguir = 10f;
    public float distanciaParaAtacar = 5f;
    private bool olhandoDireita = true;

    [Header("Ataque à Distância")]
    public GameObject projetilPrefab;
    public Transform pontoDeDisparo;
    public float velocidadeTiro = 7f;
    public float tempoEntreTiros = 2f;
    private float ultimoTiro;

    private SpriteRenderer sprite;
    private Color corOriginal;
    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;

    private enum Estado { Idle, Andando, Atacando }
    private Estado estadoAtual = Estado.Idle;

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
        if (player == null) return;

        float distancia = Vector2.Distance(transform.position, player.position);

        if (distancia <= distanciaParaPerseguir && distancia > distanciaParaAtacar)
        {
            MudarEstado(Estado.Andando);
            Perseguir();
        }
        else if (distancia <= distanciaParaAtacar)
        {
            MudarEstado(Estado.Atacando);
            rb.velocity = Vector2.zero;
            Atirar();
        }
        else
        {
            MudarEstado(Estado.Idle);
            rb.velocity = Vector2.zero;
        }
    }

    void MudarEstado(Estado novoEstado)
    {
        if (estadoAtual == novoEstado) return;
        estadoAtual = novoEstado;

        // Atualiza parâmetros de animação
        anim.SetBool("idle", estadoAtual == Estado.Idle);
        anim.SetBool("walk", estadoAtual == Estado.Andando);
        anim.SetBool("attack", estadoAtual == Estado.Atacando);
    }

    void Perseguir()
    {
        if (player == null) return;

        float direcao = Mathf.Sign(player.position.x - transform.position.x);

        rb.velocity = new Vector2(direcao * velocidade, rb.velocity.y);

        if ((direcao > 0 && !olhandoDireita) || (direcao < 0 && olhandoDireita))
        {
            Virar();
        }
    }

    void Virar()
    {
        olhandoDireita = !olhandoDireita;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    void Atirar()
    {
        if (Time.time < ultimoTiro + tempoEntreTiros) return;
        ultimoTiro = Time.time;

        anim.SetTrigger("attack");

        GameObject projetil = Instantiate(projetilPrefab, pontoDeDisparo.position, Quaternion.identity);

        float direcao = olhandoDireita ? 1f : -1f;

        Rigidbody2D rbProj = projetil.GetComponent<Rigidbody2D>();
        if (rbProj != null)
            rbProj.velocity = new Vector2(direcao * velocidadeTiro, 0f);

        Vector3 escala = projetil.transform.localScale;
        escala.x = Mathf.Abs(escala.x) * direcao;
        projetil.transform.localScale = escala;

        Destroy(projetil, 3f);
    }

    public void TomarDano(int dano)
    {
        vida -= dano;
        StartCoroutine(PiscarVermelho());
        AplicarEmpurrao();

        if (vida <= 0)
            Morrer();
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
        if (rb == null || player == null) return;
        Vector2 direcao = (transform.position - player.position).normalized;
        rb.AddForce(direcao * forcaEmpurrao, ForceMode2D.Impulse);
    }

    void Morrer()
    {
        // Se existir uma animação de morte, toca ela — senão, só destrói
        if (anim.HasState(0, Animator.StringToHash("die")))
        {
            anim.SetTrigger("die");
            Destroy(gameObject, 0.5f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}