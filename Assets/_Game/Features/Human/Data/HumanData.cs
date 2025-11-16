using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HumanData", menuName = "ScriptableObjects/HumanData", order = 1)]
public class HumanData : ScriptableObject
{
    public List<HumanHealthLevelData> humanHealthLevelData = new List<HumanHealthLevelData>();
    public List<HumanMovementSpeedLevelData> humanMovementSpeedLevelData = new List<HumanMovementSpeedLevelData>();
    public List<HumanAttackIntervalLevelData> humanAttackIntervalLevelData = new List<HumanAttackIntervalLevelData>();
    public List<HumanDamageLevelData> humanDamageLevelData = new List<HumanDamageLevelData>();

    [Serializable]
    public class HumanHealthLevelData : LevelData
    {
        public int health;
    }

    [Serializable]
    public class HumanMovementSpeedLevelData : LevelData
    {
        public float movementSpeed;
    }

    [Serializable]
    public class HumanAttackIntervalLevelData : LevelData
    {
        public float attackInterval;
    }

    [Serializable]
    public class HumanDamageLevelData : LevelData
    {
        public int damage;
    }

    public class LevelData
    {
        public int cost;
    }
}