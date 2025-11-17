using System;
using _Game.Features.HumansState.Scripts.Core;

namespace _Game.Features.Humans
{
    [Serializable]
    public class HumanModel
    {
        public event Action OnDied;

        public int Health;
        public int MaximumHealth;
        public int Damage;
        public float AttackInterval;
        public float MovementSpeed;


        public bool IsDead => Health <= 0;

        public HumanModel(HumanData humanData)
        {
            MovementSpeed = humanData.humanMovementSpeedLevelData[0].movementSpeed;
            MaximumHealth = humanData.humanMaximumHealthLevelData[0].maximumHealth;
            AttackInterval = humanData.humanAttackIntervalLevelData[0].attackInterval;
            Damage = humanData.humanDamageLevelData[0].damage;
            Health = MaximumHealth;
        }

        public void Train(HumanStateController.TrainingData  trainingData)
        {
            MaximumHealth = trainingData.trainingMaximumHealth;
            Health = MaximumHealth;
            Damage = trainingData.trainingDamage;
            AttackInterval = trainingData.trainingAttackingInterval;
            MovementSpeed = trainingData.trainingMovementSpeed;
        }

        public void TakeDamage(int amount)
        {
            if (IsDead) return;

            Health -= amount;
            if (Health < 0) Health = 0;
            
            if (IsDead)
            {
                OnDied?.Invoke();
            }
        }
    }
}