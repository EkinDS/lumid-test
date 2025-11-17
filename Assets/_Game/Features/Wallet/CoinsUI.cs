using System.Collections;
using System.Collections.Generic;
using _Game.Infrastructure;
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
        [SerializeField] GameEvents _gameEvents;

        private float _duration = 0.6f;
        private int _coinsPerBurst = 3;
        private float _spawnDelay = 0.05f;
        private EventBus _bus;
        private int _displayedCoins;
        private int _targetCoins;
        private int _activeBursts;

        private readonly List<Ease> _eases = new List<Ease>() { Ease.OutCubic, Ease.InCubic, Ease.Linear };

        private void Awake()
        {
            _bus = _gameEvents.Bus;
            if (_coinTarget == null) _coinTarget = transform as RectTransform;

            _targetCoins = _displayedCoins = Wallet.GetCoins();
            _coinsText.text = _displayedCoins.ToString();
        }

        private void OnEnable()
        {
            _bus.Subscribe<CoinsChangedEvent>(OnCoinsChanged);
        }

        private void OnDisable()
        {
            _bus.Unsubscribe<CoinsChangedEvent>(OnCoinsChanged);
        }

        private void OnCoinsChanged(CoinsChangedEvent e)
        {
            _targetCoins = e.newTotal;

            if (e.delta > 0)
            {
                int previousTotal = e.newTotal - e.delta;
                _displayedCoins = previousTotal;
                _coinsText.text = _displayedCoins.ToString();

                StartCoroutine(SpawnCoins(e.worldPosition, _coinsPerBurst, e.delta));
            }
            else
            {
                _displayedCoins = e.newTotal;
                _coinsText.text = e.newTotal.ToString();
            }
        }

        private IEnumerator SpawnCoins(Vector3 pos, int count, int total)
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


                coin.transform.DOLocalMoveX(target.x, _duration).SetEase(_eases[Random.Range(0, _eases.Count)]);
                coin.transform.DOLocalMoveY(target.y, _duration).SetEase(_eases[Random.Range(0, _eases.Count)])
                    .OnComplete((() =>
                    {
                        Destroy(coin.gameObject);
                        Arrive(gain);
                    }));

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

        private void Arrive(int gain)
        {
            _displayedCoins += gain;
            if (_displayedCoins > _targetCoins)
                _displayedCoins = _targetCoins;

            _coinsText.text = _displayedCoins.ToString();

            _coinTarget.DOKill();
            _coinTarget.localScale = Vector3.one;
            _coinTarget.DOPunchScale(Vector3.one * 0.15f, 0.2f, 1, 0.5f);
        }
    }
}