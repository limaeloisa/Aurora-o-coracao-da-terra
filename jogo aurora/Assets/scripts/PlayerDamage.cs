using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerDamage : MonoBehaviour
{
    [Header("Sistema de Vida")]
    public LifeSystem lifeSystem;   // referência ao script de corações

    [Header("Respawn")]
    public Transform respawnPoint;  // ponto onde o player volta após morrer

    private bool isDead = false;
    private SpriteRenderer sr;

    void Start()
    {
        // se o campo LifeSystem não for preenchido manualmente, encontra automaticamente
        if (lifeSystem == null)
            lifeSystem = FindObjectOfType<LifeSystem>();

        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 🔹 Espinhos continuam funcionando com respawn
        if (collision.CompareTag("Spikes"))
        {
            TakeDamage(1);
        }
    }

    // 🔹 Dano comum (espinhos, armadilhas, etc.) → reaparece no respawn
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

    // 🔹 Dano da explosão → NÃO faz respawn, só perde vida
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