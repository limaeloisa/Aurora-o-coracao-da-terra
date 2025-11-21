using UnityEngine;

public class FireBall : MonoBehaviour
{
    public int dano = 1;
    public float velocidade = -6f;
    public float destruirApos = 3f;

    void Start()
    {
        Destroy(gameObject, destruirApos);
    }

    void Update()
    {
        transform.position += new Vector3(0, velocidade * Time.deltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerDamage pd = collision.GetComponent<PlayerDamage>();
            if (pd != null)
            {
                pd.TakeDamage(dano);
            }
            Destroy(gameObject);
        }
    }
}