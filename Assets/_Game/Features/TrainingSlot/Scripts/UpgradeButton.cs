using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public HumanStatType type;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelTex;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Image moneyImage;

    
    public void SetName(string statName)
    {
        nameText.text = statName;
    }

    public void UpdateDisplay(string currentValue, int level, string nextValue, string price, bool isMaxLevel)
    {
        levelTex.text = $"Level {level + 1}";

        if (isMaxLevel)
        {
            valueText.text = currentValue;
            priceText.text = "MAX";
            moneyImage.gameObject.SetActive(false);
        }
        else
        {
            valueText.text = $"{currentValue} > {nextValue}";
            priceText.text = price;
            moneyImage.gameObject.SetActive(true);
        }
    }
}