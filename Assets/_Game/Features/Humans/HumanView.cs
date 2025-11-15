using System;
using _Game.Features.Bosses;
using _Game.Features.PlayerWallet;
using UniRx;
using UnityEngine;

namespace _Game.Features.Humans
{
    public class HumanView : MonoBehaviour
    {
        private readonly CompositeDisposable _disposables = new();

        private int _health;
        private int _damage;

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
        }

        public void StartAttacking(BossView bossView)
        {
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
            }
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;
            if (IsDead())
            {
                Debug.Log("Human Dead!");
                Destroy(gameObject);
            }
        }
    }
}