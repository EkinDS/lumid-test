using System;

namespace _Game.Features.Bosses
{
    public class BossModel
    {
        public event Action<double, int> OnHealthChanged;
        public event Action OnDied;

        public double CurrentHp { get; private set; }

        public int MaxHp { get; private set; }

        public int Damage { get; private set; }

        public bool IsAlive => CurrentHp > 0;

        public BossModel(double hp, int damage)
        {
            CurrentHp = hp;
            MaxHp = (int)hp;
            Damage = damage;

            OnHealthChanged?.Invoke(CurrentHp, MaxHp);
        }

        public void TakeDamage(double amount)
        {
            if (!IsAlive) return;

            CurrentHp = Math.Max(0, CurrentHp - amount);
            OnHealthChanged?.Invoke(CurrentHp, MaxHp);

            if (CurrentHp <= 0)
            {
                OnDied?.Invoke();
            }
        }
    }
}