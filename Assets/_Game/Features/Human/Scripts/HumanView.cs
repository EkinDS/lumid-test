using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace _Game.Features.Humans
{
    public class HumanView : MonoBehaviour
    {
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private Vector3 _fightingPosition;
        private Sequence _takeDamageColorSequence;
        private Sequence _attackMovementSequence;
        private bool _fightingPositionIsSet;

        private readonly CompositeDisposable _moveDisposables = new();


        public void ArrangeHealthBar(int current, int max)
        {
            _healthBar.SetFillAmount(current, max);
        }

        public void MoveTo(Vector3 targetPosition, float speed, Action onReached)
        {
            _moveDisposables.Clear();

            var startPosition = transform.position;
            var distance = Vector3.Distance(startPosition, targetPosition);
            var duration = distance / speed;
            var elapsedTime = 0.0f;

            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    elapsedTime += Time.deltaTime;
                    transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);

                    if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                        return;

                    onReached?.Invoke();
                    _moveDisposables.Clear();
                })
                .AddTo(_moveDisposables);
        }

        public void SetFightingPosition()
        {
            if (_fightingPositionIsSet)
            {
                return;
            }

            _fightingPositionIsSet = true;
            
            _fightingPosition = transform.position;
        }

        public void PlayAttackAnimationUp()
        {
            transform.position = _fightingPosition;
            _attackMovementSequence.Kill();
            _attackMovementSequence = DOTween.Sequence();
            _attackMovementSequence.Append(transform.DOMoveY(_fightingPosition.y + 0.2f, 0.05f));
            _attackMovementSequence.Append(transform.DOMoveY(_fightingPosition.y, 0.05f));
        }

        public void PlayHitAnimationDown()
        {
            _spriteRenderer.color = Color.white;
            _takeDamageColorSequence.Kill();
            _takeDamageColorSequence = DOTween.Sequence();
            _takeDamageColorSequence.Append(_spriteRenderer.DOColor(Color.red, 0.05f));
            _takeDamageColorSequence.Append(_spriteRenderer.DOColor(Color.white, 0.05f));
        }


        public void PerformDeathAnimation(Action deathAction)
        {
            transform.DORotate(new Vector3(0F, 0F, 90F), 0.3F);
            _spriteRenderer.DOFade(0F, 1F).OnComplete((() =>
            {
                deathAction?.Invoke();
            }));
        }

        private void KillSequences()
        {
            _attackMovementSequence?.Kill();
            _takeDamageColorSequence?.Kill();
        }

        private void OnDestroy()
        {
            KillSequences();
            _moveDisposables.Clear();
        }
    }
}