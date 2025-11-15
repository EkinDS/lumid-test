using System;
using _Game.Features.Bosses;
using _Game.Features.PlayerWallet;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace _Game.Features.Humans
{
    public class HumanView : MonoBehaviour
    {
        private readonly CompositeDisposable _disposables = new();

        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private int _health;
        private int _damage;
        private int _maximumHealth;
        private Vector2 _originalPosition;
        private Sequence _takeDamageMovementSequence;
        private Sequence _takeDamageColorSequence;
        private Sequence _attackMovementSequence;
        
        public bool IsDead() => _health <= 0;

        public void Initialize(int count)
        {
            gameObject.name = $"Human_{count}";
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void MoveTo(Vector3 targetPosition, Action<HumanView> onReached)
        {
            var speed = 5;
            var startPosition = transform.position;
            var distance = Vector3.Distance(startPosition, targetPosition);
            var duration = distance / speed;
            var elapsedTime = 0.0f;

            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    elapsedTime += Time.deltaTime;
                    transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);

                    if (!(Vector3.Distance(transform.position, targetPosition) <= 0.1f))
                        return;

                    onReached(this);
                    _disposables.Clear();
                })
                .AddTo(_disposables);
        }

        public void Train()
        {
            _health += 10;
            _damage += 10;

            _maximumHealth = _health;
            
            _healthBar.SetFillValues(_health, _maximumHealth);
        }

        public void StartAttacking(BossView bossView)
        {
            _originalPosition = transform.position;
            Observable.Interval(TimeSpan.FromMilliseconds(1000))
                .Subscribe(_ => AttackBoss(bossView))
                .AddTo(this) //Stop attacking if human dead
                .AddTo(bossView); //Stop attacking if boss dead
        }

        private void AttackBoss(BossView bossView)
        {
            if (bossView.IsAlive())
            {
                Wallet.AddCoins(_damage);
                bossView.TakeDamage(_damage);
                
                AnimateAttack();
            }
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;
            
            _healthBar.SetFillValues(_health, _maximumHealth);

            AnimateTakingDamage();
            
            if (IsDead())
            {
                Debug.Log("Human Dead!");
                Destroy(gameObject);
            }
        }
        
        
        private void AnimateAttack()
        {
            _attackMovementSequence.Kill();
            _takeDamageMovementSequence.Kill();
            _takeDamageColorSequence.Kill();        
            
            _spriteRenderer.color = Color.white;
            transform.position = _originalPosition;
            
            _attackMovementSequence = DOTween.Sequence();
            _attackMovementSequence.Append(transform.DOLocalMoveY(_originalPosition.y + 0.2F, 0.05F));
            _attackMovementSequence.Append(transform.DOLocalMoveY(_originalPosition.y, 0.05F));
        }


        private void AnimateTakingDamage()
        {
            _attackMovementSequence.Kill();
            _takeDamageMovementSequence.Kill();
            _takeDamageColorSequence.Kill();        
            
            transform.position = _originalPosition;
            _spriteRenderer.color = Color.white;
            
            _takeDamageMovementSequence = DOTween.Sequence();
            _takeDamageMovementSequence.Append(_spriteRenderer.DOColor(Color.red, 0.05f));
            _takeDamageMovementSequence.Append(_spriteRenderer.DOColor(Color.white, 0.05f));

            transform.position = _originalPosition;
            _takeDamageColorSequence = DOTween.Sequence();
            _takeDamageColorSequence.Append(transform.DOLocalMoveY(_originalPosition.y - 0.2F, 0.05F));
            _takeDamageColorSequence.Append(transform.DOLocalMoveY(_originalPosition.y, 0.05F));
        }
        
        
        private void OnDestroy()
        {
            _takeDamageMovementSequence.Kill();
            _takeDamageColorSequence.Kill();
            _attackMovementSequence.Kill();
        }
    }
}