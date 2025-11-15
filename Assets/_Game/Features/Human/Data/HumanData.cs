using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HumanData", menuName = "ScriptableObjects/HumanData", order = 1)]
public class HumanData : ScriptableObject
{
    public List<HumanHealthLevelData> humanLevelData = new List<HumanHealthLevelData>();
    public List<HumanMovementSpeedLevelData> humanMovementSpeedLevelData = new List<HumanMovementSpeedLevelData>();
    public List<HumanAttackIntervalLevelData> humanAttackIntervalLevelData = new List<HumanAttackIntervalLevelData>();
    public List<HumanDamageLevelData> humanDamageLevelData = new List<HumanDamageLevelData>();

    [Serializable]
    public class HumanHealthLevelData : ILevelData
    {
        public int health;
        public int Cost { get; }
    }

    [Serializable]
    public class HumanMovementSpeedLevelData : ILevelData
    {
        public float movementSpeed;
        public int Cost { get; }
    }

    [Serializable]
    public class HumanAttackIntervalLevelData : ILevelData
    {
        public float attackInterval;
        public int Cost { get; }
    }

    [Serializable]
    public class HumanDamageLevelData : ILevelData
    {
        public int damage;
        public int Cost { get; }
    }
}