using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuprincipalManager : MonoBehaviour
{
   [SerializeField] private string nomeDoLevelDoJogador;
   [SerializeField] private GameObject painelMenuInicial;

   public void AbrirOpcoes()
   {
      painelMenuInicial.SetActive(false);
   }

   public void FecharOpcoes()
   {
      painelMenuInicial.SetActive(true);
   }
   
   
   
   public void Jogar()
   {
      SceneManager.LoadScene(nomeDoLevelDoJogador);
   }

   public void SairJogo()
   {
      Debug.Log("sair do jogo");
      Application.Quit();
   }
}
