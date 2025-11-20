using System;
using DG.Tweening;
using UnityEngine;

namespace _Game.Features.Bosses
{
    public class BossView : MonoBehaviour
    {
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private SpriteRenderer _mainSpriteRenderer;
        [SerializeField] private SpriteRenderer _clubSpriteRenderer;

        private Vector2 _originalPosition;
        private Sequence _takeDamageColorSequence;
        private Sequence _attackMovementSequence;
        private float _lastHitAnimationTime;

        private void Awake()
        {
            _originalPosition = transform.position;
        }

        public void SetHealthBar(int currentHp, int maxHp)
        {
            _healthBar.SetFillAmount(currentHp, maxHp);
        }

        public void PlayAttackAnimation()
        {
            transform.position = _originalPosition;
            _attackMovementSequence.Kill();
            _attackMovementSequence = DOTween.Sequence();
            _attackMovementSequence.Append(transform.DOMoveY(_originalPosition.y - 0.2f, 0.05f));
            _attackMovementSequence.Append(transform.DOMoveY(_originalPosition.y, 0.05f));
        }

        public void PlayHitAnimation()
        {
            float currentTime = Time.time;
            
            if (_lastHitAnimationTime > currentTime - 1F)
            {
                return;
            }

            _lastHitAnimationTime = currentTime;
            
            _mainSpriteRenderer.color = Color.white;
            _takeDamageColorSequence.Kill();
            _takeDamageColorSequence = DOTween.Sequence();
            _takeDamageColorSequence.Append(_mainSpriteRenderer.DOColor(Color.red, 0.05f));
            _takeDamageColorSequence.Append(_mainSpriteRenderer.DOColor(Color.white, 0.05f));
        }
        
        public void PerformDeathAnimation(Action deathAction)
        {
            transform.DORotate(new Vector3(0F, 0F, -90F), 0.3F);
            _clubSpriteRenderer.DOFade(0F, 0.5F);
            _mainSpriteRenderer.DOFade(0F, 1F).OnComplete((() =>
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
        }
    }
}