using System;

namespace _Game.Features.Humans
{
    public class HumanModel
    {
        public event Action<int, int> OnHealthChanged;
        public event Action<int> OnDamageChanged;
        public event Action OnDied;

        public int Health { get; private set; }
        
        public int MaximumHealth { get; private set; }
        
        public int Damage { get; private set; }
        
        public bool IsDead => Health <= 0;

        public HumanModel(int initialHealth, int initialDamage)
        {
            MaximumHealth = initialHealth;
            Health = initialHealth;        
            Damage = initialDamage;
        }

        public void Train(int healthIncrease, int damageIncrease)
        {
            MaximumHealth += healthIncrease;
            Health += healthIncrease;
            Damage += damageIncrease;

            OnHealthChanged?.Invoke(Health, MaximumHealth);
            OnDamageChanged?.Invoke(Damage);
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
    }
}