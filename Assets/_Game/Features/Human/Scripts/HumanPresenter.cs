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

        public bool IsDead => _model.IsDead;

        private void Awake()
        {
            _view = GetComponent<HumanView>();
        }

        public void Initialize(HumanStateController humanStateController)
        {
            _model = new HumanModel(humanStateController. GetHumanData());

            _model.OnMaximumHealthChanged += HandleMaximumHealthChanged;
            _model.OnDamageChanged += HandleDamageChanged;
            _model.OnMovementSpeedChanged += HandleMovementSpeedChanged;
            _model.OnAttackIntervalChanged += HandleAttackIntervalChanged;
            _model.OnDied += HandleDied;
        }
        
        public void MoveTo(Vector3 targetPosition, System.Action<HumanPresenter> onReached)
        {
            _view.MoveTo(targetPosition, _model.MovementSpeed, () => onReached?.Invoke(this));
        }

        public void Train()
        {
            _model.Train();
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
        }
        
        private void HandleMaximumHealthChanged(int health)
        {
            _model.MaximumHealthToBeAfterTraining = health;
            //_view.ArrangeHealthBar(current, max);
        }
        
        private void HandleDamageChanged(int damage)
        {
            _model.DamageToBeAfterTraining = damage;
        }
        
        private void HandleMovementSpeedChanged(float movementSpeed)
        {
            _model.MovementSpeedToBeAfterTraining = movementSpeed;

        }
        
        private void HandleAttackIntervalChanged(float attackInterval)
        {
            _model.AttackIntervalToBeAfterTraining = attackInterval;

        }
        
        private IEnumerator AttackLoop()
        {
            var wait = new WaitForSeconds(1f);

            while (_boss != null && _boss.IsAlive && !_model.IsDead)
            {
                Wallet.AddCoins(_model.Damage);
                _boss.TakeDamage(_model.Damage);
                _view.PlayAttackAnimationUp();

                yield return wait;
            }
        }
        
        ////
        
        public event Action<HumanPresenter> OnHumanDied;


        public void SetMaxHealthToBeAfterTraining(int newMax)
        {
            _model.SetMaxHealthToBeAfterTraining(newMax);
        }

        public void SetMoveSpeedToBeAfterTraining(float newSpeed)
        {
            _model.SetMovementSpeedToBeAfterTraining(newSpeed);
        }

        public void SetAttackIntervalToBeAfterTraining(float newInterval)
        {
            _model.SetAttackIntervalToBeAfterTraining(newInterval);
        }

        public void SetDamageToBeAfterTraining(int newDamage)
        {
            _model.SetDamageToBeAfterTraining(newDamage);
        }

        private void HandleDied()
        {
            if (_attackRoutine != null)
                StopCoroutine(_attackRoutine);

            OnHumanDied?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
