using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillerSprite;
    [SerializeField] private TextMeshProUGUI healthText;

    private bool healhDroppedBelowZero;

    public void SetFillAmount(int currentHealth, int maximumHealth)
    {
        float percentage = (float)currentHealth / maximumHealth;
        fillerSprite.fillAmount = percentage;
        healthText.text = currentHealth +  "/" + maximumHealth;

        if (currentHealth <= 0)
        {
            healhDroppedBelowZero = true;
        }
        
        SetVisibility(currentHealth > 0);
    }

    private void SetVisibility(bool isVisible)
    {
        if (healhDroppedBelowZero)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(isVisible);
        }
    }
}