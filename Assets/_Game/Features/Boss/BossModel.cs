using System;

namespace _Game.Features.Bosses
{
    public class BossModel
    {
        public event Action<int, int> OnHealthChanged;
        public event Action OnDied;

        public int CurrentHp { get; private set; }

        public int MaxHp { get; private set; }

        public int Damage { get; private set; }

        public bool IsAlive => CurrentHp > 0;

        public BossModel(int hp, int damage)
        {
            CurrentHp = hp;
            MaxHp = hp;
            Damage = damage;

            OnHealthChanged?.Invoke(CurrentHp, MaxHp);
        }

        public void TakeDamage(int amount)
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