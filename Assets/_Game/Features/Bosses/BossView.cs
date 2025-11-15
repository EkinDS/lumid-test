using System;
using System.Collections.Generic;
using _Game.Features.Humans;
using UnityEngine;

namespace _Game.Features.Bosses
{
    public class BossView : MonoBehaviour
    {
        public Action<List<HumanView>> BossDefeatedCallback { get; set; }

        private readonly List<HumanView> _attackers = new();

        public bool IsAlive() => _currentHp > 0;

        private float _lastAttackTime;
        private double _currentHp;
        private float _attackInterval;
        private int _targetsPerAttack;
        private int _damage;

        public void Initialize(double hp, float attackInterval, int targetsPerAttack, double damage)
        {
            _currentHp = hp;
            _attackInterval = attackInterval;
            _targetsPerAttack = targetsPerAttack;
            _damage = (int)damage;
        }

        private void Update()
        {
            if (!IsAlive())
                return;

            AttackHumans();
        }

        private void AttackHumans()
        {
            if (Time.time - _lastAttackTime >= _attackInterval && _attackers.Count > 0)
            {
                var defeatedHumans = new List<HumanView>();
                for (var i = 0; i < Mathf.Min(_targetsPerAttack, _attackers.Count); i++)
                {
                    var target = _attackers[i];
                    target.TakeDamage(_damage);
                    if (target.IsDead())
                    {
                        defeatedHumans.Add(target);
                    }
                }

                foreach (var defeatedHuman in defeatedHumans)
                {
                    _attackers.Remove(defeatedHuman);
                }

                _lastAttackTime = Time.time;
            }
        }

        public void TakeDamage(double damage)
        {
            _currentHp = Math.Max(0, _currentHp - damage);
            if (!(_currentHp <= 0))
                return;

            Debug.Log("Boss Defeated!");
            BossDefeatedCallback.Invoke(_attackers);
            Destroy(gameObject);
        }

        public void RegisterAttacker(HumanView humanView)
        {
            if (!_attackers.Contains(humanView))
            {
                _attackers.Add(humanView);
            }
        }
    }
}