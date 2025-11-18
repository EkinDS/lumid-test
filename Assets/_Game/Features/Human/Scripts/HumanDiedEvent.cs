using _Game.Infrastructure;
using UnityEngine;

namespace _Game.Features.Humans
{
    public readonly struct HumanDiedEvent : IGameEvent
    {
        public readonly HumanPresenter Human;

        public HumanDiedEvent(HumanPresenter human)
        {
            Human = human;
        }
    }

}