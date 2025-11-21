using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject container; // painel que contém a barra

    private int maxHealth;

    private void Start()
    {
        if (container != null) container.SetActive(false);
    }

    public void SetMaxHealth(int hp)
    {
        maxHealth = hp;
        if (slider != null)
        {
            slider.maxValue = hp;
            slider.value = hp;
        }
        if (container != null) container.SetActive(true);
    }

    public void SetHealth(int hp)
    {
        if (slider != null)
            slider.value = hp;
    }

    public void Hide()
    {
        if (container != null) container.SetActive(false);
    }
}