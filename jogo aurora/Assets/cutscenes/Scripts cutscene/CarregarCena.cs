using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class CarregarCena : MonoBehaviour
{

    public string cenaParaCarregar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.LoadScene(cenaParaCarregar);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
