using TMPro;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    public HumanStatType type;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI valueText;

    public void SetName(string name)
    {
        nameText.text = name;
    }

    public void UpdateDisplay(string currentValue, string nextValue, string price)
    {
        valueText.text = $"{currentValue} > {nextValue}";
        priceText.text = price;
    }
}
