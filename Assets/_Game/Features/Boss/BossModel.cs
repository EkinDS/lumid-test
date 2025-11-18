using System;
using _Game.Infrastructure;

namespace _Game.Features.Bosses
{
    public class BossModel
    {
        public int CurrentHp { get; private set; }
        public int MaxHp { get; private set; }
        public int Damage { get; private set; }
        public bool IsAlive => CurrentHp > 0;

        readonly EventBus _bus;

        public BossModel(int hp, int damage, EventBus bus)
        {
            _bus = bus;
            CurrentHp = hp;
            MaxHp = hp;
            Damage = damage;

            _bus?.Publish(new BossHealthChangedEvent(CurrentHp, MaxHp));
        }

        public void TakeDamage(int amount)
        {
            if (!IsAlive) return;

            CurrentHp = Math.Max(0, CurrentHp - amount);
            _bus?.Publish(new BossHealthChangedEvent(CurrentHp, MaxHp));

            if (CurrentHp <= 0)
                _bus?.Publish(new BossDiedEvent());
        }
    }
}