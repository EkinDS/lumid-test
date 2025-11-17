using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Game.Features.PlayerWallet
{
    public class CoinsUI : MonoBehaviour
    {
        [SerializeField] TMP_Text _coinsText;
        [SerializeField] Canvas _canvas;
        [SerializeField] RectTransform _coinTarget;
        [SerializeField] RectTransform _coinPrefab;
        [SerializeField] RectTransform _coinParent;
        [SerializeField] int _coinsPerBurst = 3;
        [SerializeField] float _duration = 0.6f;
        [SerializeField] float _spawnDelay = 0.05f;

        int _displayedCoins;
        int _targetCoins;
        int _activeBursts;

        void Awake()
        {
            if (_coinTarget == null) _coinTarget = transform as RectTransform;
        }


        void OnEnable()
        {
            Wallet.OnCoinsChanged += OnChanged;
            Wallet.OnCoinsGained += OnGained;
            _targetCoins = _displayedCoins = Wallet.GetCoins();
            _coinsText.text = _displayedCoins.ToString();
        }

        void OnDisable()
        {
            Wallet.OnCoinsChanged -= OnChanged;
            Wallet.OnCoinsGained -= OnGained;
        }

        void OnChanged(int v)
        {
            bool gain = v > _targetCoins;
            _targetCoins = v;

            if (!gain)
            {
                _displayedCoins = v;
                _coinsText.text = v.ToString();
            }
        }

        void OnGained(int amount, Vector3 pos)
        {
            StartCoroutine(SpawnCoins(pos, _coinsPerBurst, amount));
        }

        IEnumerator SpawnCoins(Vector3 pos, int count, int total)
        {
            var cam = Camera.main;
            var canvasRT = _canvas.transform as RectTransform;

            _activeBursts++;

            Vector3 sp = cam.WorldToScreenPoint(pos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, sp, null, out var start);
            Vector2 target = canvasRT.InverseTransformPoint(_coinTarget.position);

            int baseGain = total / count;
            int rem = total % count;

            for (int i = 0; i < count; i++)
            {
                int gain = baseGain + (i == count - 1 ? rem : 0);

                var coin = Instantiate(_coinPrefab, _coinParent);
                coin.anchoredPosition = start;

                Vector2 control = (start + target) * 0.5f;
                control.y += Random.Range(60f, 120f);
                control.x += Random.Range(-40f, 40f);

                DOTween.To(() => 0f, t =>
                    {
                        coin.anchoredPosition = Bezier(start, control, target, t);
                    }, 1f, _duration)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(() =>
                    {
                        Destroy(coin.gameObject);
                        Arrive(gain);
                    });

                yield return new WaitForSeconds(_spawnDelay);
            }

            yield return new WaitForSeconds(_duration + 0.05f);

            _activeBursts--;
            if (_activeBursts <= 0 && _displayedCoins != _targetCoins)
            {
                _displayedCoins = _targetCoins;
                _coinsText.text = _displayedCoins.ToString();
            }
        }

        Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float t)
        {
            float u = 1f - t;
            return u * u * a + 2f * u * t * b + t * t * c;
        }

        void Arrive(int gain)
        {
            _displayedCoins += gain;
            if (_displayedCoins > _targetCoins) _displayedCoins = _targetCoins;
            _coinsText.text = _displayedCoins.ToString();

            _coinTarget.DOKill();
            _coinTarget.localScale = Vector3.one;
            _coinTarget.DOPunchScale(Vector3.one * 0.15f, 0.2f, 1, 0.5f);
        }
    }
}
