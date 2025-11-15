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
}

[Serializable]
public class HumanHealthLevelData
{
    public int health;
    public int cost;
}

[Serializable]
public class HumanMovementSpeedLevelData
{
    public float movementSpeed;
    public int cost;
}

[Serializable]
public class HumanAttackIntervalLevelData
{
    public float attackInterval;
    public int cost;
}

[Serializable]
public class HumanDamageLevelData
{
    public int damage;
    public int cost;
}