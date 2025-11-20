using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class EfeitoDigitador : MonoBehaviour
{
   private TextMeshProUGUI componentTexto;
   private AudioSource _audioSource;
   private string mensagemOriginal;
   public bool imprimindo;
   public float tempoEntreLetras = 0.08f;

   private void Awake()
   {
      TryGetComponent(out componentTexto);
      TryGetComponent(out _audioSource);
      mensagemOriginal = componentTexto.text;
      componentTexto.text = "";
   }

   private void OnEnable()
   {
      ImprimirMensagem(mensagemOriginal);
   }

   private void OnDisabable()
   {
      componentTexto.text = mensagemOriginal;
      StopAllCoroutines();
      
   }

   public void ImprimirMensagem(string mensagem)
   {
      if (gameObject.activeInHierarchy)
      {
         if (imprimindo) return;
         imprimindo = true;
         StartCoroutine(LetraPorLetra(mensagem));
      }
   }

   IEnumerator LetraPorLetra(string mensagem)
   {
      string msg = "";
      foreach (var letra in mensagem)
      {
         msg += letra;
         componentTexto.text = msg;
         _audioSource.Play();
         yield return new WaitForSeconds(tempoEntreLetras);
      }
      
      imprimindo = false;
      StopAllCoroutines();
      
   }
   
}
