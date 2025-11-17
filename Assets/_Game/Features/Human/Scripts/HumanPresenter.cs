using System;
using System.Collections;
using _Game.Features.Bosses;
using _Game.Features.HumansState.Scripts.Core;
using _Game.Features.PlayerWallet;
using UnityEngine;

namespace _Game.Features.Humans
{
    [RequireComponent(typeof(HumanView))]
    public class HumanPresenter : MonoBehaviour
    {
        private HumanModel _model;
        private HumanView _view;
        private BossPresenter _boss;
        private Coroutine _attackRoutine;
        public event Action<HumanPresenter> OnHumanDied;

        
        public bool IsDead => _model.IsDead;

        private void Awake()
        {
            _view = GetComponent<HumanView>();
        }

        public void Initialize(GameManager gameManager)
        {
            _model = new HumanModel(gameManager.GetHumanData());
            name = "Human " + Time.time;

            _model.OnDied += HandleDied;
        }

        public void MoveTo(Vector3 targetPosition, System.Action<HumanPresenter> onReached)
        {
            _view.MoveTo(targetPosition, _model.movementSpeed, () => onReached?.Invoke(this));
        }

        public void Train(GameManager.TrainingData trainingData)
        {
            _model.Train(trainingData);
            _view.ArrangeHealthBar(_model.health, _model.maximumHealth);
        }

        public void StartAttacking(BossPresenter bossView)
        {
            if (_attackRoutine != null)
            {
                StopCoroutine(_attackRoutine);
            }

            _boss = bossView;

            _view.SetFightingPosition();
            _attackRoutine = StartCoroutine(AttackLoop());
        }

        public void TakeDamage(int damage)
        {
            _model.TakeDamage(damage);
            _view.PlayHitAnimationDown();
            _view.ArrangeHealthBar(_model.health, _model.maximumHealth);
        }


        private IEnumerator AttackLoop()
        {
            var wait = new WaitForSeconds(_model.attackInterval);
            
            yield return wait;
            while (_boss != null && _boss.IsAlive && !_model.IsDead)
            {
                RewardPlayer(_model.damage, transform.position);
                _boss.TakeDamage(_model.damage);
                _view.PlayAttackAnimationUp();

                yield return wait;
            }
        }

        private void HandleDied()
        {
            if (_attackRoutine != null)
            {
                StopCoroutine(_attackRoutine);
            }

            OnHumanDied?.Invoke(this);

            _view.PerformDeathAnimation(() => Destroy(gameObject));
        }
        
        private void RewardPlayer(int amount, Vector3 worldPos)
        {
            Wallet.AddCoins(amount, worldPos); 
        }
    }
}