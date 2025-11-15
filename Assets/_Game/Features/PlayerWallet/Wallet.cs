namespace _Game.Features.PlayerWallet
{
    public static class Wallet
    {
        private static int _coins;

        static Wallet()
        {
            _coins = 0;
        }

        public static void AddCoins(int amount)
        {
            _coins += amount;
        }

        public static int GetCoins()
        {
            return _coins;
        }
    }
}