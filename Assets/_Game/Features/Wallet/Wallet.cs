namespace _Game.Features.PlayerWallet
{
    public static class Wallet
    {
        public static event System.Action<int> OnCoinsChanged;

        private static int _coins;

        static Wallet()
        {
            _coins = 0;
        }

        public static void AddCoins(int amount)
        {
            _coins += amount;
            
            OnCoinsChanged?.Invoke(_coins);
        }

        public static int GetCoins()
        {
            return _coins;
        }
    }
}