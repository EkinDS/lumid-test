using System;
using System.Collections.Generic;
using _Game.Features.Humans;
using DG.Tweening;
using UnityEngine;

namespace _Game.Features.Bosses
{
    [RequireComponent(typeof(BossView))]
    public class BossPresenter : MonoBehaviour
    {
        [SerializeField] private int _startingHp;
        [SerializeField] private int _damage;
        [SerializeField] private float _attackInterval;
        [SerializeField] private int _targetsPerAttack;

        private BossModel _model;
        private BossView _view;
        private float _lastAttackTime;
        private Action<List<HumanPresenter>> bossDefeatedCallback;

        private readonly List<HumanPresenter> _attackers = new();

        public bool IsAlive => _model != null && _model.IsAlive;

        private void Awake()
        {
            _view = GetComponent<BossView>();

            _model = new BossModel(_startingHp, _damage);
            SubscribeModelEvents();
            _view.SetHealthBar(_model.CurrentHp, _model.MaxHp);
        }

        private void Update()
        {
            if (!IsAlive)
                return;

            if (_attackers.Count == 0)
                return;

            if (Time.time - _lastAttackTime < _attackInterval)
                return;

            AttackHumans();
            _lastAttackTime = Time.time;
        }

        public void Initialize(int hp, float attackInterval, int targetsPerAttack, double damage)
        {
            _attackInterval = attackInterval;
            _targetsPerAttack = targetsPerAttack;
            _damage = (int)damage;

            _model = new BossModel(hp, _damage);
            SubscribeModelEvents();

            _view.SetHealthBar(_model.CurrentHp, _model.MaxHp);
        }

        public void TakeDamage(int damage)
        {
            _model.TakeDamage(damage);
            _view.PlayHitAnimation();
        }

        public void RegisterAttacker(HumanPresenter humanPresenter)
        {
            if (!_attackers.Contains(humanPresenter))
            {
                _attackers.Add(humanPresenter);
            }
        }

        public void SetDeathCallback(Action<List<HumanPresenter>> callback)
        {
            bossDefeatedCallback = callback;
        }

        private void HandleHealthChanged(int current, int max)
        {
            _view.SetHealthBar(current, max);
        }

        private void HandleDied()
        {
            bossDefeatedCallback?.Invoke(new List<HumanPresenter>(_attackers));
           
            _view.PerformDeathAnimation((() => Destroy(gameObject)));
        }

        private void OnDestroy()
        {
            UnsubscribeModelEvents();
        }


        private void SubscribeModelEvents()
        {
            _model.OnHealthChanged += HandleHealthChanged;
            _model.OnDied += HandleDied;
        }

        private void UnsubscribeModelEvents()
        {
            if (_model == null) return;

            _model.OnHealthChanged -= HandleHealthChanged;
            _model.OnDied -= HandleDied;
        }


        private void AttackHumans()
        {
            var defeatedHumans = new List<HumanPresenter>();

            var count = Mathf.Min(_targetsPerAttack, _attackers.Count);
            for (var i = 0; i < count; i++)
            {
                var target = _attackers[i];
                target.TakeDamage(_model.Damage);

                _view.PlayAttackAnimation();

                if (target.IsDead)
                {
                    defeatedHumans.Add(target);
                }
            }

            foreach (var defeatedHuman in defeatedHumans)
            {
                _attackers.Remove(defeatedHuman);
            }
        }
    }
}