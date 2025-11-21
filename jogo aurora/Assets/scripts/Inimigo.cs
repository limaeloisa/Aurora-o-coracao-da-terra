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
    private bool olhandoDireita = true;

    private SpriteRenderer sprite;
    private Color corOriginal;
    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;

    private enum Estado { Idle, Andando }
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

        MudarEstado(Estado.Andando);
        Perseguir();
    }

    void MudarEstado(Estado novoEstado)
    {
        if (estadoAtual == novoEstado) return;
        estadoAtual = novoEstado;

        anim.SetBool("idle", estadoAtual == Estado.Idle);
        anim.SetBool("walk", estadoAtual == Estado.Andando);
    }

    void Perseguir()
    {
        float direcao = Mathf.Sign(player.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(direcao * velocidade, rb.linearVelocity.y);

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

    // --- COLISÃO COM O PLAYER (ATAQUE CORPO-A-CORPO SIMPLES) ---
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
        {
            // Aqui você pode colocar animação de dano do player
            // col.collider.GetComponent<PlayerDamage>()?.TomarDano(1);

            // Não existe mais animação de ataque do inimigo
        }
    }

    // --- SISTEMA DE DANO DO INIMIGO ---
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