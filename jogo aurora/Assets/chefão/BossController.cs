using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class BossController : MonoBehaviour
{
    [Header("Vida")]
    public int maxVida = 200;
    [SerializeField] private int vida;

    [Header("Movimentação")]
    public float moveSpeed = 2f;
    public float chaseRange = 20f;         // distância para perseguir o player
    public float stopDistance = 1.5f;      // distância mínima para não colidir com o player

    [Header("Fases")]
    [Tooltip("Percentual de vida para entrar na fase 2 (0..1)")]
    [Range(0f, 1f)] public float fase2Threshold = 0.5f;

    [Header("Ataque Distância")]
    public GameObject projectilePrefab;
    public Transform[] projectileSpawnPoints;
    public float shootCooldown = 1.2f;
    public int shootBurst = 3;             // quantos projéteis por rajada
    public float shootBurstDelay = 0.25f;  // delay entre projéteis na rajada
    public float projectileSpeed = 6f;

    [Header("Ataque Corpo-a-Corpo")]
    public int meleeDamage = 10;
    public float meleeRange = 1.2f;
    public float meleeCooldown = 2f;

    [Header("Dash / Teleport")]
    public float dashDistance = 6f;
    public float dashSpeed = 10f;
    public float dashCooldown = 5f;

    [Header("Feedback (partículas / som)")]
    public ParticleSystem hitParticles;
    public ParticleSystem dieParticles;
    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip shootSound;
    public AudioClip dashSound;
    public AudioClip dieSound;

    [Header("Referências e ajustes")]
    public LayerMask obstacleLayer;
    public Transform playerTransform;
    public BossHealthUI healthUI; // arraste o script da UI (Slider) aqui

    // estado interno
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private bool olhandoDireita = true;

    private float lastShootTime = -999f;
    private float lastMeleeTime = -999f;
    private float lastDashTime = -999f;
    private bool fase2 = false;
    private bool isDead = false;
    private Vector2 originalScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        vida = maxVida;
        originalScale = transform.localScale;

        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (healthUI != null)
            healthUI.SetMaxHealth(maxVida);
    }

    void Update()
    {
        if (isDead || playerTransform == null) return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);

        // cheque de fase
        if (!fase2 && vida <= maxVida * fase2Threshold)
        {
            EnterFase2();
        }

        // comportamento simples:
        if (dist <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            // idle patrol simples (pode implementar)
            rb.velocity = new Vector2(0, rb.velocity.y);
            if (anim) anim.SetBool("walk", false);
        }

        // tentar atacar à distância (se estiver apto)
        if (Time.time >= lastShootTime + shootCooldown)
        {
            StartCoroutine(ShootBurst());
            lastShootTime = Time.time;
        }

        // dash ocasional em fase2
        if (fase2 && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(DashAttack());
            lastDashTime = Time.time;
        }
    }

    // Move em direção ao player mantendo distância mínima
    void ChasePlayer()
    {
        Vector2 dir = (playerTransform.position - transform.position);
        float horizontal = Mathf.Sign(dir.x);

        // virar sprite
        if ((horizontal > 0 && !olhandoDireita) || (horizontal < 0 && olhandoDireita))
            Flip(horizontal > 0);

        // se estiver perto demais, para e tenta melee
        float dist = dir.magnitude;
        if (dist <= stopDistance)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            if (Time.time >= lastMeleeTime + meleeCooldown)
            {
                MeleeAttack();
                lastMeleeTime = Time.time;
            }
            if (anim) anim.SetBool("walk", false);
        }
        else
        {
            // mover-se horizontalmente em direção ao player
            Vector2 vel = new Vector2(horizontal * moveSpeed, rb.velocity.y);
            rb.velocity = vel;
            if (anim) anim.SetBool("walk", true);
        }
    }

    IEnumerator ShootBurst()
    {
        if (isDead) yield break;

        if (projectilePrefab == null || projectileSpawnPoints == null || projectileSpawnPoints.Length == 0) yield break;

        if (audioSource != null && shootSound != null) audioSource.PlayOneShot(shootSound);
        if (anim) anim.SetTrigger("shoot");

        for (int i = 0; i < shootBurst; i++)
        {
            foreach (Transform sp in projectileSpawnPoints)
            {
                SpawnProjectile(sp);
            }
            yield return new WaitForSeconds(shootBurstDelay);
        }
    }

    void SpawnProjectile(Transform spawnPoint)
    {
        if (projectilePrefab == null) return;

        GameObject proj = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        BossProjectile bp = proj.GetComponent<BossProjectile>();
        if (bp != null)
        {
            float dir = olhandoDireita ? 1f : -1f;
            bp.Initialize(new Vector2(dir * projectileSpeed, 0f), meleeDamage / 2, gameObject);
        }
        else
        {
            Rigidbody2D pRb = proj.GetComponent<Rigidbody2D>();
            if (pRb != null)
            {
                pRb.velocity = new Vector2((olhandoDireita ? 1f : -1f) * projectileSpeed, 0f);
            }
        }
        Destroy(proj, 6f);
    }

    void MeleeAttack()
    {
        if (anim) anim.SetTrigger("melee");
        // dano ao player com overlap circle
        Collider2D hit = Physics2D.OverlapCircle(transform.position + (Vector3.right * (olhandoDireita ? meleeRange : -meleeRange)), meleeRange, LayerMask.GetMask("Default", "Player"));
        if (hit != null && hit.CompareTag("Player"))
        {
            PlayerDamage pd = hit.GetComponent<PlayerDamage>();
            pd?.TakeDamage(meleeDamage);
        }
    }

    IEnumerator DashAttack()
    {
        if (audioSource != null && dashSound != null) audioSource.PlayOneShot(dashSound);
        if (anim) anim.SetTrigger("dash");

        // direção para o player
        Vector2 target = playerTransform.position;
        Vector2 start = transform.position;
        Vector2 dir = (target - start).normalized;
        float traveled = 0f;

        // durante o dash ignora física lateral control?
        while (traveled < dashDistance)
        {
            rb.velocity = dir * dashSpeed;
            traveled += dashSpeed * Time.deltaTime;
            yield return null;
        }

        // reset velocity
        rb.velocity = Vector2.zero;
    }

    void EnterFase2()
    {
        fase2 = true;
        // aumenta agressividade
        moveSpeed *= 1.25f;
        shootCooldown *= 0.8f;
        shootBurst = Mathf.Max(1, shootBurst + 1);
        if (anim) anim.SetTrigger("fase2");
        // efeito visual
        if (hitParticles != null) hitParticles.Play();
    }

    // chama quando recebe dano
    public void TomarDano(int dano, Vector2 hitPoint)
    {
        if (isDead) return;

        vida -= dano;
        if (healthUI != null) healthUI.SetHealth(vida);

        if (hitParticles != null)
        {
            ParticleSystem p = Instantiate(hitParticles, hitPoint, Quaternion.identity);
            p.Play();
            Destroy(p.gameObject, 1f);
        }

        if (audioSource != null && hitSound != null) audioSource.PlayOneShot(hitSound);

        if (vida <= 0)
        {
            StartCoroutine(DieRoutine());
        }
    }

    private IEnumerator DieRoutine()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        if (anim) anim.SetTrigger("die");
        if (audioSource != null && dieSound != null) audioSource.PlayOneShot(dieSound);
        if (dieParticles != null)
        {
            ParticleSystem p = Instantiate(dieParticles, transform.position, Quaternion.identity);
            p.Play();
            Destroy(p.gameObject, 2f);
        }
        // esperar animação de morte
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    void Flip(bool faceRight)
    {
        olhandoDireita = faceRight;
        Vector3 s = originalScale;
        s.x = Mathf.Abs(originalScale.x) * (faceRight ? 1f : -1f);
        transform.localScale = s;
    }

    // debug gizmos para melee range
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (Vector3.right * meleeRange), meleeRange);
    }
}