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

    private void OnEnable()
    {
        Wallet.OnCoinsChanged += HandleCoinsChanged;
    }

    private void OnDisable()
    {
        Wallet.OnCoinsChanged -= HandleCoinsChanged;
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

            RefreshAll();
        }

        upgradesAreShown = !upgradesAreShown;
    }

    private void RefreshAll()
    {
        foreach (var btn in _buttons)
            Refresh(btn);
    }

    private void Refresh(UpgradeButton upgradeButton)
    {
        int level = _levels[upgradeButton.type];

        switch (upgradeButton.type)
        {
            case HumanStatType.Health:
                RefreshGeneric(upgradeButton, level, _data.humanHealthLevelData.Count,
                    _data.humanHealthLevelData[level].health,
                    level + 1 < _data.humanHealthLevelData.Count ? _data.humanHealthLevelData[level + 1].health : -1,
                    level + 1 < _data.humanHealthLevelData.Count ? _data.humanHealthLevelData[level + 1].cost : -1);
                break;
            case HumanStatType.MoveSpeed:
                RefreshGeneric(upgradeButton, level, _data.humanMovementSpeedLevelData.Count,
                    _data.humanMovementSpeedLevelData[level].movementSpeed.ToString("0.0"),
                    level + 1 < _data.humanMovementSpeedLevelData.Count
                        ? _data.humanMovementSpeedLevelData[level + 1].movementSpeed.ToString("0.0")
                        : "MAX",
                    level + 1 < _data.humanMovementSpeedLevelData.Count
                        ? _data.humanMovementSpeedLevelData[level + 1].cost
                        : -1);
                break;
            case HumanStatType.AttackInterval:
                RefreshGeneric(upgradeButton, level, _data.humanAttackIntervalLevelData.Count,
                    _data.humanAttackIntervalLevelData[level].attackInterval.ToString("0.00"),
                    level + 1 < _data.humanAttackIntervalLevelData.Count
                        ? _data.humanAttackIntervalLevelData[level + 1].attackInterval.ToString("0.00")
                        : "MAX",
                    level + 1 < _data.humanAttackIntervalLevelData.Count
                        ? _data.humanAttackIntervalLevelData[level + 1].cost
                        : -1);
                break;
            case HumanStatType.Damage:
                RefreshGeneric(upgradeButton, level, _data.humanDamageLevelData.Count,
                    _data.humanDamageLevelData[level].damage,
                    level + 1 < _data.humanDamageLevelData.Count ? _data.humanDamageLevelData[level + 1].damage : -1,
                    level + 1 < _data.humanDamageLevelData.Count ? _data.humanDamageLevelData[level + 1].cost : -1);
                break;
        }
    }

    private void RefreshGeneric(UpgradeButton btn, int level, int maxLevel,
        object current, object next, int cost)
    {
        string nextVal = next.ToString() == "-1" ? "MAX" : next.ToString();
        string costText = cost < 0 ? "MAX" : cost.ToString();

        btn.UpdateDisplay(current.ToString(), level, nextVal, costText, level >= maxLevel);

        var button = btn.GetComponent<Button>();
        button.interactable = cost >= 0 && Wallet.GetCoins() >= cost;
    }

    private void OnUpgrade(HumanStatType type)
    {
        var level = _levels[type];

        int cost = type switch
        {
            HumanStatType.Health => NextCost(_data.humanHealthLevelData, level),
            HumanStatType.MoveSpeed => NextCost(_data.humanMovementSpeedLevelData, level),
            HumanStatType.AttackInterval => NextCost(_data.humanAttackIntervalLevelData, level),
            HumanStatType.Damage => NextCost(_data.humanDamageLevelData, level),
            _ => -1
        };

        if (cost < 0 || Wallet.GetCoins() < cost)
            return;

        Wallet.AddCoins(-cost);
        _levels[type]++;

        ApplyStat(type);
        RefreshAll();
    }

    private int NextCost<T>(List<T> list, int level) where T : HumanData.LevelData
        => level + 1 < list.Count ? list[level + 1].cost : -1;

    private void ApplyStat(HumanStatType type)
    {
        foreach (var h in _controller.Humans)
        {
            switch (type)
            {
                case HumanStatType.Health:
                    h.SetMaxHealth(_data.humanHealthLevelData[_levels[type]].health);
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

    private void HandleCoinsChanged(int newCoins)
    {
        if (upgradesAreShown)
            RefreshAll();
    }
}