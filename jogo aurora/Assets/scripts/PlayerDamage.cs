using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (collision.CompareTag("Spikes"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        lifeSystem.vida -= amount; // tira 1 coração
        StartCoroutine(DamageFlash()); // pisca vermelho

        if (lifeSystem.vida > 0)
        {
            // reaparece no ponto salvo
            StartCoroutine(Respawn());
        }
        else
        {
            // se acabou as vidas, "morre de vez"
            Die();
        }
    }

    System.Collections.IEnumerator DamageFlash()
    {
        if (sr != null)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            sr.color = Color.white;
        }
    }

    System.Collections.IEnumerator Respawn()
    {
        isDead = true;

        // desativa temporariamente o player
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

        // espera meio segundo pra dar tempo de ver o flash
        yield return new WaitForSeconds(0.5f);

        // move o player para o ponto de respawn
        transform.position = respawnPoint.position;

        // reativa o collider
        GetComponent<Collider2D>().enabled = true;
        isDead = false;
    }

    void Die()
    {
        Debug.Log("Player morreu sem vidas!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}