using System.Collections.Generic;
using _Game.Features.Humans;
using _Game.Infrastructure;

namespace _Game.Features.Bosses
{
    public readonly struct BossDefeatedEvent : IGameEvent
    {
        public readonly IReadOnlyList<HumanPresenter> Attackers;

        public BossDefeatedEvent(IReadOnlyList<HumanPresenter> attackers)
        {
            Attackers = attackers;
        }
    }
}