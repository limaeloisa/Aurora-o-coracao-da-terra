using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configurações de Ataque")]
    public GameObject projetilPrefab;
    public Transform pontoDeDisparo;
    public float velocidadeDoTiro = 10f;
    public float tempoEntreTiros = 0.8f;

    [Header("Configurações Opcionais")]
    public float tempoDestruicaoProjetil = 3f; // Destrói o projétil após X segundos

    private Animator anim;
    private float ultimoTiro;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && Time.time > ultimoTiro + tempoEntreTiros)
        {
            ultimoTiro = Time.time;
            Atacar();
        }
    }

    void Atacar()
    {
        // 1️⃣ Executa animação de ataque
        if (anim != null)
        {
            anim.SetBool("IsAttacking", true);
            Invoke(nameof(ResetarAnimacao), 0.4f);
        }

        // 2️⃣ Cria o projetil no ponto de disparo
        GameObject projetil = Instantiate(projetilPrefab, pontoDeDisparo.position, Quaternion.identity);

        // 3️⃣ Detecta direção baseada na rotação do player (0 = direita, 180 = esquerda)
        float direcao = (transform.eulerAngles.y == 180f) ? -1f : 1f;

        // 4️⃣ Define velocidade do tiro
        Rigidbody2D rb = projetil.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(direcao * velocidadeDoTiro, 0f);
        }

        // 5️⃣ Vira o sprite do projetil pro lado certo
        Vector3 escala = projetil.transform.localScale;
        escala.x = Mathf.Abs(escala.x) * direcao;
        projetil.transform.localScale = escala;

        // 6️⃣ Destrói o projétil após um tempo
        Destroy(projetil, tempoDestruicaoProjetil);
    }

    void ResetarAnimacao()
    {
        if (anim != null)
        {
            anim.SetBool("IsAttacking", false);
        }
    }
}