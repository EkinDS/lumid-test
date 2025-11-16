using System;

namespace _Game.Features.Humans
{
    [Serializable]
    public class HumanModel
    {
        public event Action<int> OnMaximumHealthChanged;
        public event Action<int> OnDamageChanged;
        public event Action<float> OnMovementSpeedChanged;
        public event Action<float> OnAttackIntervalChanged;
        public event Action OnDied;

        public int Health;
        public int MaximumHealth;
        public int Damage;
        public float AttackInterval;
        public float MovementSpeed;

        public int MaximumHealthToBeAfterTraining;
        public int DamageToBeAfterTraining;
        public float AttackIntervalToBeAfterTraining;
        public float MovementSpeedToBeAfterTraining;

        public bool IsDead => Health <= 0;

        public HumanModel(HumanData humanData)
        {
            MovementSpeed = humanData.humanMovementSpeedLevelData[0].movementSpeed;
            MaximumHealth = humanData.humanMaximumHealthLevelData[0].maximumHealth;
            AttackInterval = humanData.humanAttackIntervalLevelData[0].attackInterval;
            Damage = humanData.humanDamageLevelData[0].damage;
            Health = MaximumHealth;

            MaximumHealthToBeAfterTraining = MaximumHealth;
            DamageToBeAfterTraining = Damage;
            AttackIntervalToBeAfterTraining = AttackInterval;
            MovementSpeedToBeAfterTraining = MovementSpeed;
        }

        public void Train()
        {
            MaximumHealth = MaximumHealthToBeAfterTraining;
            Health = MaximumHealthToBeAfterTraining;
            Damage = DamageToBeAfterTraining;
            AttackInterval = AttackIntervalToBeAfterTraining;
            MovementSpeed = MovementSpeedToBeAfterTraining;

            OnMaximumHealthChanged?.Invoke(MaximumHealth);
            OnDamageChanged?.Invoke(Damage);
            OnAttackIntervalChanged?.Invoke(AttackInterval);
            OnMovementSpeedChanged?.Invoke(MovementSpeed);
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


        public void SetMaxHealthToBeAfterTraining(int value)
        {
            MaximumHealthToBeAfterTraining = value;
        }

        public void SetDamageToBeAfterTraining(int value)
        {
            DamageToBeAfterTraining = value;
        }

        public void SetMovementSpeedToBeAfterTraining(float value)
        {
            MovementSpeedToBeAfterTraining = value;
        }

        public void SetAttackIntervalToBeAfterTraining(float value)
        {
            AttackIntervalToBeAfterTraining = value;
        }
    }
}