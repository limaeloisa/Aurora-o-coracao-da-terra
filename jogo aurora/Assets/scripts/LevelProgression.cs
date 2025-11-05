using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // ← Importante para usar TextMeshPro

public class LevelProgression : MonoBehaviour
{
    [Header("Configurações de Coletáveis")]
    public int totalNecessario = 5; // Quantos itens são necessários
    private int coletados = 0; // Quantos o jogador já pegou

    [Header("Próxima Fase")]
    public string proximaFase;

    [Header("Referência de UI")]
    public TextMeshProUGUI textoContador; // Campo para mostrar na tela

    private bool podeAvancar = false;

    void Start()
    {
        AtualizarTexto(); // Mostra o texto inicial na tela
    }

    public void AdicionarColetavel()
    {
        coletados++;
        Debug.Log("Itens coletados: " + coletados);

        if (coletados >= totalNecessario)
        {
            podeAvancar = true;
            Debug.Log("Meta atingida! Pode passar de fase!");
        }

        AtualizarTexto(); // Atualiza o texto toda vez que pegar algo
    }

    private void AtualizarTexto()
    {
        if (textoContador != null)
        {
            textoContador.text = "Coletados: " + coletados + " / " + totalNecessario;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (podeAvancar)
            {
                Debug.Log("Indo para a próxima fase...");
                SceneManager.LoadScene(proximaFase);
            }
            else
            {
                Debug.Log("Você ainda não coletou o suficiente!");
            }
        }
    }
}