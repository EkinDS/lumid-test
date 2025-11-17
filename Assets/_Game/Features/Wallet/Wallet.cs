using UnityEngine;
using _Game.Infrastructure;

namespace _Game.Features.PlayerWallet
{
    public static class Wallet
    {
        static EventBus _bus;
        static int _coins;

        public static void Initialize(EventBus bus)
        {
            _bus = bus;
        } 

        public static int GetCoins() => _coins;

        public static void AddCoins(int amount) => AddCoins(amount, Vector3.zero);

        public static void AddCoins(int amount, Vector3 worldPosition)
        {
            _coins += amount;
            
            _bus?.Publish(new CoinsChangedEvent(amount, _coins, worldPosition));
        }
    }
}