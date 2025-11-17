using System;
using UnityEngine;

namespace _Game.Features.PlayerWallet
{
    public static class Wallet
    {
        public static event Action<int> OnCoinsChanged;

        public static event Action<int, Vector3> OnCoinsGained;

        private static int _coins;

        static Wallet()
        {
            _coins = 0;
        }

        public static int GetCoins()
        {
            return _coins;
        }

        public static void AddCoins(int amount)
        {
            _coins += amount;
            OnCoinsChanged?.Invoke(_coins);
        }

        public static void AddCoins(int amount, Vector3 worldPosition)
        {
            _coins += amount;
            OnCoinsChanged?.Invoke(_coins);
            OnCoinsGained?.Invoke(amount, worldPosition);
        }
    }
}