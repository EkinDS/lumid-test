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
        private Sequence _takeDamageMovementSequence;
        private Sequence _takeDamageColorSequence;
        private Sequence _attackMovementSequence;
        
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
            _fightingPosition = transform.position;
        }
        
        public void PlayAttackAnimationUp()
        {
            KillSequences();

            _spriteRenderer.color = Color.white;
            transform.position = _fightingPosition;

            _attackMovementSequence = DOTween.Sequence();
            _attackMovementSequence.Append(transform.DOLocalMoveY(_fightingPosition.y + 0.2f, 0.05f));
            _attackMovementSequence.Append(transform.DOLocalMoveY(_fightingPosition.y, 0.05f));
        }

        public void PlayHitAnimationDown()
        {
            KillSequences();

            transform.position = _fightingPosition;
            _spriteRenderer.color = Color.white;

            _takeDamageColorSequence = DOTween.Sequence();
            _takeDamageColorSequence.Append(_spriteRenderer.DOColor(Color.red, 0.05f));
            _takeDamageColorSequence.Append(_spriteRenderer.DOColor(Color.white, 0.05f));

            _takeDamageMovementSequence = DOTween.Sequence();
            _takeDamageMovementSequence.Append(transform.DOLocalMoveY(_fightingPosition.y - 0.2f, 0.05f));
            _takeDamageMovementSequence.Append(transform.DOLocalMoveY(_fightingPosition.y, 0.05f));
        }

        private void KillSequences()
        {
            _attackMovementSequence?.Kill();
            _takeDamageMovementSequence?.Kill();
            _takeDamageColorSequence?.Kill();
        }

        private void OnDestroy()
        {
            KillSequences();
            _moveDisposables.Clear();
        }
    }
}
