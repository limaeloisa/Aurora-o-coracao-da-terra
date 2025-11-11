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

    private SpriteRenderer sprite;
    private Color corOriginal;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
            corOriginal = sprite.color;
    }

    public void TomarDano(int dano)
    {
        vida -= dano;
        Debug.Log("Inimigo tomou dano. Vida restante: " + vida);

        //  Pisca em vermelho ao levar dano
        StartCoroutine(PiscarVermelho());

        //  Aplica empurrão leve na direção oposta ao Player
        AplicarEmpurrao();

        if (vida <= 0)
        {
            Morrer();
        }
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
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        PlayerAttack player = FindObjectOfType<PlayerAttack>();

        if (rb != null && player != null)
        {
            Vector2 direcao = (transform.position - player.transform.position).normalized;
            rb.AddForce(direcao * forcaEmpurrao, ForceMode2D.Impulse);
        }
    }

    void Morrer()
    {
        // Aqui você pode adicionar animação de morte ou partícula
        Destroy(gameObject);
    }
}