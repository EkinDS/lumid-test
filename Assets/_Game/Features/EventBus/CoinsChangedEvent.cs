using UnityEngine;
using _Game.Infrastructure;

namespace _Game.Features.PlayerWallet
{
    public readonly struct CoinsChangedEvent : IGameEvent
    {
        public readonly int Delta;
        public readonly int NewTotal;
        public readonly Vector3 WorldPosition;

        public CoinsChangedEvent(int delta, int newTotal, Vector3 worldPosition)
        {
            Delta = delta;
            NewTotal = newTotal;
            WorldPosition = worldPosition;
        }
    }
}