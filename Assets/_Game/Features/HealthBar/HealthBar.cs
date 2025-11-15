using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillerSprite;
    

    public void SetFillAmount(float currentHealth, float maximumHealth)
    {
        float percentage = currentHealth / maximumHealth;
        fillerSprite.fillAmount = percentage;
    }
}