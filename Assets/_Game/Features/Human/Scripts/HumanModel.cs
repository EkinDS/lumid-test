using System;

namespace _Game.Features.Humans
{
    public class HumanModel
    {
        public event Action<int, int> OnHealthChanged;
        public event Action<int> OnDamageChanged;
        public event Action<float> OnMovementSpeedChanged;
        public event Action<float> OnAttackIntervalChanged;
        public event Action OnDied;

        public int Health { get; set; }

        public int MaximumHealth { get; set; }

        public int Damage { get; set; }
        public float AttackInterval { get; set; }
        public float MovementSpeed { get; set; }

        public bool IsDead => Health <= 0;

        public HumanModel(int initialHealth, int initialDamage)
        {
            MaximumHealth = initialHealth;
            Health = initialHealth;
            Damage = initialDamage;
        }

        public void Train(int healthIncrease, int damageIncrease, float movementSpeedIncrease, float attackIntervalChange)
        {
            MaximumHealth += healthIncrease;
            Health += healthIncrease;
            Damage += damageIncrease;
            AttackInterval += attackIntervalChange;
            MovementSpeed += movementSpeedIncrease;

            OnHealthChanged?.Invoke(Health, MaximumHealth);
            OnDamageChanged?.Invoke(Damage);
            OnAttackIntervalChanged?.Invoke(AttackInterval);
            OnMovementSpeedChanged?.Invoke(MovementSpeed);
        }

        public void TakeDamage(int amount)
        {
            if (IsDead) return;

            Health -= amount;
            if (Health < 0) Health = 0;

            OnHealthChanged?.Invoke(Health, MaximumHealth);

            if (IsDead)
            {
                OnDied?.Invoke();
            }
        }
        
        
        public void SetMaxHealth(int value)
        {
            float currentPercentage = (float)Health / (float)MaximumHealth;
            
            MaximumHealth = value;
            Health = (int)(MaximumHealth * currentPercentage);
            
            if (Health > MaximumHealth)
                Health = MaximumHealth;

            OnHealthChanged?.Invoke(Health, MaximumHealth);
        }

        public void SetDamage(int value)
        {
            Damage = value;
            OnDamageChanged?.Invoke(Damage);
        }

        public void SetMovementSpeed(float value)
        {
            MovementSpeed = value;
            OnMovementSpeedChanged?.Invoke(MovementSpeed);
        }

        public void SetAttackInterval(float value)
        {
            AttackInterval = value;
            OnAttackIntervalChanged?.Invoke(AttackInterval);
        }
    }
}