using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyChase : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 2f;             // velocidade de perseguição
    public float chaseRange = 8f;        // distância máxima para começar a perseguir
    public float stopDistance = 0.4f;    // distância para parar perto do player

    [Header("Dano")]
    public int damage = 1;               // quanto tira de vida ao tocar
    public float damageCooldown = 1f;    // tempo entre danos consecutivos
    public float knockbackForce = 6f;    // força do empurrão aplicado ao player

    [Header("Referências/Opcional")]
    public Transform target;             // se vazio, encontra com tag "Player"
    public GameObject hitEffectPrefab;   // efeito spawn ao acertar (opcional)
    public Animator animator;            // animações do inimigo (opcional)

    Rigidbody2D rb;
    float nextDamageTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Recomendo: congelar rotação para evitar girar ao colidir
        rb.freezeRotation = true;

        // se não atribuído, tenta achar o player pela tag
        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) target = p.transform;
        }

        // se tiver animator e ele usar parâmetros, pode configurar aqui
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position);
        float dist = direction.magnitude;

        // ativa perseguição apenas dentro do alcance
        if (dist <= chaseRange && dist > stopDistance)
        {
            Vector2 move = direction.normalized * speed;
            // mover usando velocity (mais simples para colisões)
            rb.linearVelocity = new Vector2(move.x, rb.linearVelocity.y);
            FlipSprite(move.x);
            if (animator != null) animator.SetBool("walk", true);
        }
        else
        {
            // para o deslocamento horizontal quando fora de alcance ou muito perto
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (animator != null) animator.SetBool("walk", false);
        }
    }

    void FlipSprite(float moveX)
    {
        if (moveX > 0.01f) transform.eulerAngles = new Vector3(0f, 0f, 0f);
        else if (moveX < -0.01f) transform.eulerAngles = new Vector3(0f, 180f, 0f);
    }

    // Quando colidir fisicamente com o player
    void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamage(collision.collider);
    }

    // Também tenta durante a permanência no contato (respeitando cooldown)
    void OnCollisionStay2D(Collision2D collision)
    {
        TryDamage(collision.collider);
    }

    void TryDamage(Collider2D col)
    {
        if (Time.time < nextDamageTime) return;

        // busca componente PlayerDamage no objeto colidido ou em seus pais
        PlayerDamage pd = col.GetComponent<PlayerDamage>();
        if (pd == null)
            pd = col.GetComponentInParent<PlayerDamage>();

        if (pd != null)
        {
            // aplica dano
            pd.TakeDamage(damage);
            nextDamageTime = Time.time + damageCooldown;

            // efeito de hit opcional
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, col.transform.position, Quaternion.identity);
            }

            // aplica knockback ao Rigidbody2D do player (se existir)
            Rigidbody2D playerRb = col.GetComponent<Rigidbody2D>() ?? col.GetComponentInParent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockDir = (col.transform.position - transform.position).normalized;
                playerRb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);
            }

            // animação de ataque (opcional)
            if (animator != null)
            {
                animator.SetTrigger("attack");
            }
        }
    }

    // (Opcional) gizmo para visualizar alcance no editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}