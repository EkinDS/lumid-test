using TMPro;
using UnityEngine;

namespace _Game.Features.PlayerWallet
{
    public class CoinsDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _coinsText;

        private void Update()
        {
            _coinsText.text = $"{Wallet.GetCoins().ToString()}";
        }
    }
}