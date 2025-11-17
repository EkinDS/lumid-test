using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillerSprite;
    [SerializeField] private TextMeshProUGUI healthText;


    public void SetFillAmount(int currentHealth, int maximumHealth)
    {
        float percentage = (float)currentHealth / maximumHealth;
        fillerSprite.fillAmount = percentage;
        healthText.text = currentHealth +  "/" + maximumHealth;
        SetVisibility(currentHealth > 0);
    }


    private void SetVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }
}