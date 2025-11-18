using _Game.Infrastructure;

namespace _Game.Features.Bosses
{
    public readonly struct BossHealthChangedEvent : IGameEvent
    {
        public readonly int CurrentHp;
        public readonly int MaxHp;

        public BossHealthChangedEvent(int currentHp, int maxHp)
        {
            CurrentHp = currentHp;
            MaxHp = maxHp;
        }
    }

    public readonly struct BossDiedEvent : IGameEvent
    {
    }
}