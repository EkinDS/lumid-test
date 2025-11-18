using System;
using _Game.Features.HumansState.Scripts.Core;
using UnityEngine;

namespace _Game.Features.Humans
{
    [Serializable]
    public class HumanModel
    {
        public event Action OnDied;

        public int health;
        public int maximumHealth;
        public int damage;
        public float attackInterval;
        public float movementSpeed;

        public bool IsDead => health <= 0;

        public HumanModel(HumanData humanData)
        {
            movementSpeed   = humanData.humanMovementSpeedLevelData[0].movementSpeed;
            maximumHealth   = humanData.humanMaximumHealthLevelData[0].maximumHealth;
            attackInterval  = humanData.humanAttackIntervalLevelData[0].attackInterval;
            damage          = humanData.humanDamageLevelData[0].damage;
            health          = maximumHealth;
        }

        public void Train(GameManager.TrainingData trainingData)
        {
            maximumHealth  = trainingData.trainingMaximumHealth;
            health         = maximumHealth;
            damage         = trainingData.trainingDamage;
            attackInterval = trainingData.trainingAttackingInterval;
            movementSpeed  = trainingData.trainingMovementSpeed;
        }

        public void TakeDamage(int amount)
        {
            if (IsDead) return;

            health = Mathf.Max(health - amount, 0);

            if (IsDead)
                OnDied?.Invoke();
        }
    }
}