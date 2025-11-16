using System;
using System.Collections;
using _Game.Features.Bosses;
using _Game.Features.HumansState.Scripts.Core;
using _Game.Features.PlayerWallet;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Game.Features.Humans
{
    [RequireComponent(typeof(HumanView))]
    public class HumanPresenter : MonoBehaviour
    {
        private HumanModel _model;
        private HumanView _view;
        private BossPresenter _boss;
        private Coroutine _attackRoutine;

        public bool IsDead => _model.IsDead;

        private void Awake()
        {
            _view = GetComponent<HumanView>();
        }

        public void Initialize(HumanStateController humanStateController)
        {
            _model = new HumanModel(humanStateController. GetHumanData());

            _model.OnDied += HandleDied;
        }
        
        public void MoveTo(Vector3 targetPosition, System.Action<HumanPresenter> onReached)
        {
            _view.MoveTo(targetPosition, _model.MovementSpeed, () => onReached?.Invoke(this));
        }

        public void Train(HumanStateController.TrainingData trainingData)
        {
            _model.Train(trainingData);
            _view.ArrangeHealthBar(_model.Health, _model.MaximumHealth);
        }

        public void StartAttacking(BossPresenter bossView)
        {
            _boss = bossView;

            if (_attackRoutine != null)
                StopCoroutine(_attackRoutine);

            _view.SetFightingPosition();
            _attackRoutine = StartCoroutine(AttackLoop());
        }

        public void TakeDamage(int damage)
        {
            _model.TakeDamage(damage);
            _view.PlayHitAnimationDown();
            _view.ArrangeHealthBar(_model.Health, _model.MaximumHealth);
        }

        
        private IEnumerator AttackLoop()
        {
            var wait = new WaitForSeconds(_model.AttackInterval + Random.Range(-0.1F, 0F));

            while (_boss != null && _boss.IsAlive && !_model.IsDead)
            {
                Wallet.AddCoins(_model.Damage);
                _boss.TakeDamage(_model.Damage);
                _view.PlayAttackAnimationUp();

                yield return wait;
            }
        }
        
        public event Action<HumanPresenter> OnHumanDied;


        private void HandleDied()
        {
            if (_attackRoutine != null)
                StopCoroutine(_attackRoutine);

            OnHumanDied?.Invoke(this);
            
            _view.PerformDeathAnimation(() => Destroy(gameObject));
        }
    }
}
