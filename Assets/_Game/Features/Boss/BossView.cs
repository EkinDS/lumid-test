using DG.Tweening;
using UnityEngine;

namespace _Game.Features.Bosses
{
    public class BossView : MonoBehaviour
    {
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private Vector2 _originalPosition;
        private Sequence _takeDamageMovementSequence;
        private Sequence _takeDamageColorSequence;
        private Sequence _attackMovementSequence;

        private void Awake()
        {
            _originalPosition = transform.position;
        }

        public void SetHealthBar(double currentHp, int maxHp)
        {
            _healthBar.SetFillAmount((float)currentHp, maxHp);
        }

        public void PlayAttackAnimation()
        {
            KillSequences();
            ResetVisual();

            _attackMovementSequence = DOTween.Sequence();
            _attackMovementSequence.Append(transform.DOLocalMoveY(_originalPosition.y - 0.2f, 0.05f));
            _attackMovementSequence.Append(transform.DOLocalMoveY(_originalPosition.y, 0.05f));
        }

        public void PlayHitAnimation()
        {
            KillSequences();
            ResetVisual();

            _takeDamageColorSequence = DOTween.Sequence();
            _takeDamageColorSequence.Append(_spriteRenderer.DOColor(Color.red, 0.05f));
            _takeDamageColorSequence.Append(_spriteRenderer.DOColor(Color.white, 0.05f));

            _takeDamageMovementSequence = DOTween.Sequence();
            _takeDamageMovementSequence.Append(transform.DOLocalMoveY(_originalPosition.y + 0.2f, 0.05f));
            _takeDamageMovementSequence.Append(transform.DOLocalMoveY(_originalPosition.y, 0.05f));
        }

        private void ResetVisual()
        {
            transform.position = _originalPosition;
            _spriteRenderer.color = Color.white;
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
        }
    }
}