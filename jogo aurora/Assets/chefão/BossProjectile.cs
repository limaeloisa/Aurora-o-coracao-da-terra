using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BossProjectile : MonoBehaviour
{
    public int damage = 5;
    private Rigidbody2D rb;
    private GameObject owner;

    public void Initialize(Vector2 velocity, int dmg, GameObject ownerObj)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = velocity;
        damage = dmg;
        owner = ownerObj;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == owner) return; // ignora colidir com o boss

        if (other.CompareTag("Player"))
        {
            PlayerDamage pd = other.GetComponent<PlayerDamage>();
            pd?.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            // bateu em algo do cenário
            Destroy(gameObject);
        }
    }
}