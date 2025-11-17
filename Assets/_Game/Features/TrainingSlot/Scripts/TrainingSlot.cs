using System.Collections.Generic;
using _Game;
using _Game.Features.HumansState.Scripts.Core;
using _Game.Features.PlayerWallet;
using _Game.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

public class TrainingSlot : MonoBehaviour
{
    [SerializeField] private GameObject upgradeButtonsContainer;
    [SerializeField] private GameManager _controller;
    [SerializeField] private List<UpgradeButton> _buttons;
    [SerializeField] private GameEvents _gameEvents;

    private bool upgradesAreShown;
    private readonly Dictionary<HumanStatType, int> _levels = new();
    private EventBus _bus;


    public void Initialize()
    {
        print("awake"+ "," + (_bus == null));
        _bus = _controller.EventBus;

        foreach (var btn in _buttons)
        {
            btn.SetName(btn.type.ToString());
            var button = btn.GetComponent<Button>();
            button.onClick.AddListener(() => OnUpgrade(btn.type));
            _levels[btn.type] = 0;
        }
        
        _bus.Subscribe<CoinsChangedEvent>(HandleCoinsChanged);

    }

    private void OnEnable()
    {
        //print("enable" + "," + (_bus == null));
    }

    private void OnDisable()
    {
        print("disable");
        _bus.Unsubscribe<CoinsChangedEvent>(HandleCoinsChanged);
    }

    public void ArrowButtonClicked()
    {
        if (upgradesAreShown)
        {
            upgradeButtonsContainer.SetActive(false);
        }
        else
        {
            upgradeButtonsContainer.SetActive(true);
            RefreshAll();
        }

        upgradesAreShown = !upgradesAreShown;
    }

    void RefreshAll()
    {
        foreach (var btn in _buttons)
            Refresh(btn);
    }

    private void Refresh(UpgradeButton upgradeButton)
    {
        int level = _levels[upgradeButton.type];
        HumanData data = _controller.GetHumanData();

        switch (upgradeButton.type)
        {
            case HumanStatType.Health:
                RefreshGeneric(upgradeButton, level, data.humanMaximumHealthLevelData.Count - 1,
                    data.humanMaximumHealthLevelData[level].maximumHealth,
                    level + 1 < data.humanMaximumHealthLevelData.Count
                        ? data.humanMaximumHealthLevelData[level + 1].maximumHealth
                        : -1,
                    level + 1 < data.humanMaximumHealthLevelData.Count
                        ? data.humanMaximumHealthLevelData[level + 1].cost
                        : -1);
                break;

            case HumanStatType.MoveSpeed:
                RefreshGeneric(upgradeButton, level, data.humanMovementSpeedLevelData.Count - 1,
                    data.humanMovementSpeedLevelData[level].movementSpeed.ToString("0.0"),
                    level + 1 < data.humanMovementSpeedLevelData.Count
                        ? data.humanMovementSpeedLevelData[level + 1].movementSpeed.ToString("0.0")
                        : "MAX",
                    level + 1 < data.humanMovementSpeedLevelData.Count
                        ? data.humanMovementSpeedLevelData[level + 1].cost
                        : -1);
                break;

            case HumanStatType.AttackInterval:
                RefreshGeneric(upgradeButton, level, data.humanAttackIntervalLevelData.Count - 1,
                    data.humanAttackIntervalLevelData[level].attackInterval.ToString("0.00"),
                    level + 1 < data.humanAttackIntervalLevelData.Count
                        ? data.humanAttackIntervalLevelData[level + 1].attackInterval.ToString("0.00")
                        : "MAX",
                    level + 1 < data.humanAttackIntervalLevelData.Count
                        ? data.humanAttackIntervalLevelData[level + 1].cost
                        : -1);
                break;

            case HumanStatType.Damage:
                RefreshGeneric(upgradeButton, level, data.humanDamageLevelData.Count - 1,
                    data.humanDamageLevelData[level].damage,
                    level + 1 < data.humanDamageLevelData.Count
                        ? data.humanDamageLevelData[level + 1].damage
                        : -1,
                    level + 1 < data.humanDamageLevelData.Count
                        ? data.humanDamageLevelData[level + 1].cost
                        : -1);
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
        HumanData data = _controller.GetHumanData();

        int cost = type switch
        {
            HumanStatType.Health => NextCost(data.humanMaximumHealthLevelData, level),
            HumanStatType.MoveSpeed => NextCost(data.humanMovementSpeedLevelData, level),
            HumanStatType.AttackInterval => NextCost(data.humanAttackIntervalLevelData, level),
            HumanStatType.Damage => NextCost(data.humanDamageLevelData, level),
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
        HumanData data = _controller.GetHumanData();

        foreach (var h in _controller.Humans)
        {
            switch (type)
            {
                case HumanStatType.Health:
                    _controller.trainingData.trainingMaximumHealth =
                        data.humanMaximumHealthLevelData[_levels[type]].maximumHealth;
                    break;

                case HumanStatType.MoveSpeed:
                    _controller.trainingData.trainingMovementSpeed =
                        data.humanMovementSpeedLevelData[_levels[type]].movementSpeed;
                    break;

                case HumanStatType.AttackInterval:
                    _controller.trainingData.trainingAttackingInterval =
                        data.humanAttackIntervalLevelData[_levels[type]].attackInterval;
                    break;

                case HumanStatType.Damage:
                    _controller.trainingData.trainingDamage =
                        data.humanDamageLevelData[_levels[type]].damage;
                    break;
            }
        }
    }

    void HandleCoinsChanged(CoinsChangedEvent e)
    {
        if (upgradesAreShown)
            RefreshAll();
    }
}