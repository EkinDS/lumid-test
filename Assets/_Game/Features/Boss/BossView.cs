using System;
using System.Collections.Generic;
using _Game.Features.Humans;
using DG.Tweening;
using UnityEngine;

namespace _Game.Features.Bosses
{
    public class BossView : MonoBehaviour
    {
        public Action<List<HumanPresenter>> BossDefeatedCallback { get; set; }

        private readonly List<HumanPresenter> _attackers = new();

        public bool IsAlive() => _currentHp > 0;

        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private float _lastAttackTime;
        private double _currentHp;
        private float _attackInterval;
        private int _targetsPerAttack;
        private int _damage;
        private int _maximumHealth;
        private Vector2 _originalPosition;
        private Sequence _takeDamageMovementSequence;
        private Sequence _takeDamageColorSequence;
        private Sequence _attackMovementSequence;

        public void Initialize(double hp, float attackInterval, int targetsPerAttack, double damage)
        {
            _originalPosition = transform.position;
            _currentHp = hp;
            _attackInterval = attackInterval;
            _targetsPerAttack = targetsPerAttack;
            _damage = (int)damage;

            _maximumHealth = (int)_currentHp;

            _healthBar.SetFillAmount((float)_currentHp, _maximumHealth);
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
                var defeatedHumans = new List<HumanPresenter>();
                for (var i = 0; i < Mathf.Min(_targetsPerAttack, _attackers.Count); i++)
                {
                    var target = _attackers[i];
                    target.TakeDamage(_damage);

                   AnimateAttack();

                    if (target.IsDead)
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

            _healthBar.SetFillAmount((float)_currentHp, _maximumHealth);

            AnimateTakingDamage();

            if (!(_currentHp <= 0))
                return;

            BossDefeatedCallback.Invoke(_attackers);
            Destroy(gameObject);
        }

        public void RegisterAttacker(HumanPresenter humanView)
        {
            if (!_attackers.Contains(humanView))
            {
                _attackers.Add(humanView);
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
            _attackMovementSequence.Append(transform.DOLocalMoveY(_originalPosition.y - 0.2F, 0.05F));
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
            _takeDamageColorSequence.Append(transform.DOLocalMoveY(_originalPosition.y + 0.2F, 0.05F));
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