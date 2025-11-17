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
        [SerializeField] int _coinsPerBurst = 3;
        [SerializeField] float _duration = 0.6f;
        [SerializeField] float _spawnDelay = 0.05f;
        [SerializeField] GameEvents _gameEvents;

        EventBus _bus;

        int _displayedCoins;
        int _targetCoins;
        int _activeBursts;

        private List<Ease> _eases = new List<Ease>() { Ease.OutCubic, Ease.InCubic, Ease.Linear };

        void Awake()
        {
            _bus = _gameEvents.Bus;
            if (_coinTarget == null) _coinTarget = transform as RectTransform;

            _targetCoins = _displayedCoins = Wallet.GetCoins();
            _coinsText.text = _displayedCoins.ToString();
        }

        void OnEnable()
        {
            _bus.Subscribe<CoinsChangedEvent>(OnCoinsChanged);
        }

        void OnDisable()
        {
            _bus.Unsubscribe<CoinsChangedEvent>(OnCoinsChanged);
        }

        void OnCoinsChanged(CoinsChangedEvent e)
        {
            _targetCoins = e.NewTotal;

            if (e.Delta > 0)
            {
                int previousTotal = e.NewTotal - e.Delta;
                _displayedCoins = previousTotal;
                _coinsText.text = _displayedCoins.ToString();

                StartCoroutine(SpawnCoins(e.WorldPosition, _coinsPerBurst, e.Delta));
            }
            else
            {
                _displayedCoins = e.NewTotal;
                _coinsText.text = e.NewTotal.ToString();
            }
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

                
                coin.transform.DOLocalMoveX(target.x, _duration).SetEase(_eases[Random.Range(0, _eases.Count)]);
                coin.transform.DOLocalMoveY(target.y, _duration).SetEase(_eases[Random.Range(0, _eases.Count)]).OnComplete((() =>
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

        Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float t)
        {
            float u = 1f - t;
            return u * u * a + 2f * u * t * b + t * t * c;
        }

        void Arrive(int gain)
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
