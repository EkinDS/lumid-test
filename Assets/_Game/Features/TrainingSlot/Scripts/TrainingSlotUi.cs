using System.Collections.Generic;
using _Game.Features.HumansState.Scripts.Core;
using _Game.Features.PlayerWallet;
using UnityEngine;
using UnityEngine.UI;

public class TrainingSlotUI : MonoBehaviour
{
    [SerializeField] private GameObject upgradeButtonsContainer;
    [SerializeField] private HumanStateController _controller;
    [SerializeField] private HumanData _data;
    [SerializeField] private List<UpgradeButton> _buttons;

    private bool upgradesAreShown;
    
    private readonly Dictionary<HumanStatType, int> _levels = new();

    private void Awake()
    {
        foreach (var btn in _buttons)
        {
            btn.SetName(btn.type.ToString());
            var button = btn.GetComponent<Button>();
            button.onClick.AddListener(() => OnUpgrade(btn.type));
            _levels[btn.type] = 0;
        }
    }

    private void Start()
    {
        RefreshAll();
    }


    public void ArrowButtonClicked()
    {
        if (upgradesAreShown)
        {
            upgradeButtonsContainer.gameObject.SetActive(false);
        }
        else
        {
            upgradeButtonsContainer.gameObject.SetActive(true);
        }
        
        upgradesAreShown = !upgradesAreShown;
    }

    private void RefreshAll()
    {
        foreach (var btn in _buttons)
            Refresh(btn);
    }

    private void Refresh(UpgradeButton btn)
    {
        int level = _levels[btn.type];

        switch (btn.type)
        {
            case HumanStatType.Health:
                RefreshGeneric(btn, level, _data.humanLevelData.Count,
                    _data.humanLevelData[level].health,
                    level + 1 < _data.humanLevelData.Count ? _data.humanLevelData[level + 1].health : -1,
                    level + 1 < _data.humanLevelData.Count ? _data.humanLevelData[level + 1].Cost : -1);
                break;

            case HumanStatType.MoveSpeed:
                RefreshGeneric(btn, level, _data.humanMovementSpeedLevelData.Count,
                    _data.humanMovementSpeedLevelData[level].movementSpeed.ToString("0.0"),
                    level + 1 < _data.humanMovementSpeedLevelData.Count ? _data.humanMovementSpeedLevelData[level + 1].movementSpeed.ToString("0.0") : "MAX",
                    level + 1 < _data.humanMovementSpeedLevelData.Count ? _data.humanMovementSpeedLevelData[level + 1].Cost : -1);
                break;

            case HumanStatType.AttackInterval:
                RefreshGeneric(btn, level, _data.humanAttackIntervalLevelData.Count,
                    _data.humanAttackIntervalLevelData[level].attackInterval.ToString("0.0"),
                    level + 1 < _data.humanAttackIntervalLevelData.Count ? _data.humanAttackIntervalLevelData[level + 1].attackInterval.ToString("0.0") : "MAX",
                    level + 1 < _data.humanAttackIntervalLevelData.Count ? _data.humanAttackIntervalLevelData[level + 1].Cost : -1);
                break;

            case HumanStatType.Damage:
                RefreshGeneric(btn, level, _data.humanDamageLevelData.Count,
                    _data.humanDamageLevelData[level].damage,
                    level + 1 < _data.humanDamageLevelData.Count ? _data.humanDamageLevelData[level + 1].damage : -1,
                    level + 1 < _data.humanDamageLevelData.Count ? _data.humanDamageLevelData[level + 1].Cost : -1);
                break;
        }
    }

    private void RefreshGeneric(UpgradeButton btn, int level, int maxLevel, 
                                object current, object next, int cost)
    {
        string nextVal = next.ToString() == "-1" ? "MAX" : next.ToString();
        string costText = cost < 0 ? "MAX" : cost.ToString();

        btn.UpdateDisplay(current.ToString(), nextVal, costText);

        var button = btn.GetComponent<Button>();
        button.interactable = cost >= 0 && Wallet.GetCoins() >= cost;
    }

    private void OnUpgrade(HumanStatType type)
    {
        var level = _levels[type];

        int cost = type switch
        {
            HumanStatType.Health         => NextCost(_data.humanLevelData, level),
            HumanStatType.MoveSpeed      => NextCost(_data.humanMovementSpeedLevelData, level),
            HumanStatType.AttackInterval => NextCost(_data.humanAttackIntervalLevelData, level),
            HumanStatType.Damage         => NextCost(_data.humanDamageLevelData, level),
            _ => -1
        };

        if (cost < 0 || Wallet.GetCoins() < cost)
            return;

        Wallet.AddCoins(-cost);
        _levels[type]++;

        ApplyStat(type);
        RefreshAll();
    }

    private int NextCost<T>(List<T> list, int level) where T : ILevelData
        => level + 1 < list.Count ? list[level + 1].Cost : -1;

    private void ApplyStat(HumanStatType type)
    {
        foreach (var h in _controller.Humans)
        {
            switch (type)
            {
                case HumanStatType.Health:
                    h.SetMaxHealth(_data.humanLevelData[_levels[type]].health);
                    break;

                case HumanStatType.MoveSpeed:
                    h.SetMoveSpeed(_data.humanMovementSpeedLevelData[_levels[type]].movementSpeed);
                    break;

                case HumanStatType.AttackInterval:
                    h.SetAttackInterval(_data.humanAttackIntervalLevelData[_levels[type]].attackInterval);
                    break;

                case HumanStatType.Damage:
                    h.SetDamage(_data.humanDamageLevelData[_levels[type]].damage);
                    break;
            }
        }
    }
}

public interface ILevelData
{
    int Cost { get; }
}