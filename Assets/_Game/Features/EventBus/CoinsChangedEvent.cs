using UnityEngine;
using _Game.Infrastructure;

namespace _Game.Features.PlayerWallet
{
    public struct CoinsChangedEvent : IGameEvent
    {
        public readonly int delta;
        public readonly int newTotal;
        public readonly Vector3 worldPosition;

        public CoinsChangedEvent(int delta, int newTotal, Vector3 worldPosition)
        {
            this.delta = delta;
            this.newTotal = newTotal;
            this.worldPosition = worldPosition;
        }
    }
}