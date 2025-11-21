using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerDamage : MonoBehaviour
{
    [Header("Sistema de Vida")]
    public LifeSystem lifeSystem;

    [Header("Respawn")]
    public Transform respawnPoint;

    private bool isDead = false;
    private SpriteRenderer sr;

    void Start()
    {
        if (lifeSystem == null)
            lifeSystem = FindObjectOfType<LifeSystem>();

        sr = GetComponent<SpriteRenderer>();

        // Caso não tenha respawn inicial definido, cria um
        if (respawnPoint == null)
        {
            GameObject startPoint = new GameObject("RespawnInicial");
            startPoint.transform.position = transform.position;
            respawnPoint = startPoint.transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Dano de espinho
        if (collision.CompareTag("Spikes"))
        {
            TakeDamage(1);
        }

        // Detecta novo respawnpoint
        if (collision.CompareTag("Respawn"))
        {
            SetRespawnPoint(collision.transform);
        }
    }

    // Atualiza o respawn para o checkpoint tocado
    public void SetRespawnPoint(Transform newPoint)
    {
        respawnPoint = newPoint;
        Debug.Log("Novo respawn definido: " + newPoint.position);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        lifeSystem.vida -= amount;
        StartCoroutine(DamageFlash());

        if (lifeSystem.vida > 0)
        {
            StartCoroutine(Respawn());
        }
        else
        {
            Die();
        }
    }

    public void TakeExplosionDamage(int amount)
    {
        if (isDead) return;

        lifeSystem.vida -= amount;
        StartCoroutine(DamageFlash());

        if (lifeSystem.vida <= 0)
        {
            Die();
        }
    }

    IEnumerator DamageFlash()
    {
        if (sr != null)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            sr.color = Color.white;
        }
    }

    IEnumerator Respawn()
    {
        isDead = true;

        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(0.5f);

        // Faz o respawn corretamente
        transform.position = respawnPoint.position;

        GetComponent<Collider2D>().enabled = true;
        isDead = false;
    }

    void Die()
    {
        Debug.Log("Player morreu sem vidas!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}