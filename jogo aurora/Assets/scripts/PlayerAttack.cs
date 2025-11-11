using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configurações de Ataque")]
    public GameObject projetilPrefab;      // Prefab da bola de energia
    public Transform pontoDeDisparo;       // Onde o tiro sai
    public float velocidadeDoTiro = 10f;   // Velocidade do tiro
    public float tempoEntreTiros = 0.5f;   // Delay entre tiros

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
        anim.SetTrigger("Attack");

        // 2️⃣ Cria o projetil
        GameObject projetil = Instantiate(projetilPrefab, pontoDeDisparo.position, Quaternion.identity);

        // 3️⃣ Verifica direção com base no ângulo Y da rotação do Player
        float direcao = (transform.eulerAngles.y == 180f) ? -1f : 1f;

        // 4️⃣ Define velocidade do tiro
        Rigidbody2D rb = projetil.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(direcao * velocidadeDoTiro, 0f);

        // 5️⃣ Vira o sprite do projetil se estiver indo pra esquerda
        Vector3 escala = projetil.transform.localScale;
        escala.x = Mathf.Abs(escala.x) * direcao;
        projetil.transform.localScale = escala;
    }
}