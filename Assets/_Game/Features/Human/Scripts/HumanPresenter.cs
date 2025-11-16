using System;
using System.Collections;
using _Game.Features.Bosses;
using _Game.Features.PlayerWallet;
using UnityEngine;

namespace _Game.Features.Humans
{
    [RequireComponent(typeof(HumanView))]
    public class HumanPresenter : MonoBehaviour
    {
        [SerializeField] private int _baseHealth;
        [SerializeField] private int _baseDamage;

        private HumanModel _model;
        private HumanView _view;
        private BossPresenter _boss;
        private Coroutine _attackRoutine;

        public bool IsDead => _model.IsDead;

        private void Awake()
        {
            _view = GetComponent<HumanView>();
        }

        public void Initialize()
        {
            _model = new HumanModel(_baseHealth, _baseDamage);

            _model.OnHealthChanged += HandleHealthChanged;
            _model.OnDied += HandleDied;

            _view.ArrangeHealthBar(_model.Health, _model.MaximumHealth);
        }
        
        public void MoveTo(Vector3 targetPosition, System.Action<HumanPresenter> onReached)
        {
            _view.MoveTo(targetPosition, 1F, () => onReached?.Invoke(this));
        }

        public void Train()
        {
            _model.Train(_model.MaximumHealth, _model.Damage, _model.MovementSpeed, _model.AttackInterval);
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
        
        private void HandleHealthChanged(int current, int max)
        {
            _view.ArrangeHealthBar(current, max);
        }
        
        private void HandleDamageChanged(int current, int max)
        {
            _view.ArrangeHealthBar(current, max);
        }
        
        private void HandleMovementSpeedChanged(int current, int max)
        {
            _view.ArrangeHealthBar(current, max);
        }
        
        private void HandleAttackIntervalChanged(int current, int max)
        {
            _view.ArrangeHealthBar(current, max);
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


        public void SetMaxHealth(int newMax)
        {
            _model.SetMaxHealth(newMax);
        }

        public void SetMoveSpeed(float newSpeed)
        {
            _model.SetMovementSpeed(newSpeed);
        }

        public void SetAttackInterval(float newInterval)
        {
            _model.SetAttackInterval(newInterval);
        }

        public void SetDamage(int newDamage)
        {
            _model.SetDamage(newDamage);
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
