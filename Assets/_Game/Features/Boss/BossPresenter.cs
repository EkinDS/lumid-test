using System.Collections.Generic;
using _Game.Features.Humans;
using _Game.Infrastructure;
using UnityEngine;

namespace _Game.Features.Bosses
{
    [RequireComponent(typeof(BossView))]
    public class BossPresenter : MonoBehaviour
    {
        [Header("Config")] 
        [SerializeField] private int _startingHp = 100;
        [SerializeField] private int _damage = 5;
        [SerializeField] private float _attackInterval = 1f;
        [SerializeField] private int _targetsPerAttack = 1;

        private BossModel _model;
        private BossView _view;
        private EventBus _bus;
        private float _lastAttackTime;

        private readonly List<HumanPresenter> _attackers = new();

        public bool IsAlive => _model != null && _model.IsAlive;

        void Awake()
        {
            _view = GetComponent<BossView>();
        }

        void OnDisable()
        {
            if (_bus == null) return;
            _bus.Unsubscribe<BossHealthChangedEvent>(OnBossHealthChanged);
            _bus.Unsubscribe<BossDiedEvent>(OnBossDied);
        }

        void Start()
        {
            if (_model == null && _bus != null)
                _model = new BossModel(_startingHp, _damage, _bus);

            if (_model != null)
                _view.SetHealthBar(_model.CurrentHp, _model.MaxHp);
        }

        public void Initialize(int hp, float attackInterval, int targetsPerAttack, double damage, EventBus eventBus)
        {
            _attackInterval = attackInterval;
            _targetsPerAttack = targetsPerAttack;
            _damage = (int)damage;

            _bus = eventBus;

            _model = new BossModel(hp, _damage, _bus);
            _view.SetHealthBar(_model.CurrentHp, _model.MaxHp);

            _bus.Subscribe<BossHealthChangedEvent>(OnBossHealthChanged);
            _bus.Subscribe<BossDiedEvent>(OnBossDied);
        }

        void Update()
        {
            if (!IsAlive) return;
            if (_attackers.Count == 0) return;
            if (Time.time - _lastAttackTime < _attackInterval) return;

            AttackHumans();
            _lastAttackTime = Time.time;
        }

        public void TakeDamage(int damage)
        {
            if (!IsAlive) return;
            _model.TakeDamage(damage);
            _view.PlayHitAnimation();
        }

        public void RegisterAttacker(HumanPresenter humanPresenter)
        {
            if (!_attackers.Contains(humanPresenter))
                _attackers.Add(humanPresenter);
        }

        public void UnregisterAttacker(HumanPresenter humanPresenter)
        {
            _attackers.Remove(humanPresenter);
        }

        void OnBossHealthChanged(BossHealthChangedEvent e)
        {
            _view.SetHealthBar(e.CurrentHp, e.MaxHp);
        }

        void OnBossDied(BossDiedEvent e)
        {
            _bus.Publish(new BossDefeatedEvent(new List<HumanPresenter>(_attackers)));

            _attackers.Clear();

            _view.PerformDeathAnimation(() => Destroy(gameObject));
        }

        void AttackHumans()
        {
            var defeatedHumans = new List<HumanPresenter>();
            int count = Mathf.Min(_targetsPerAttack, _attackers.Count);

            for (int i = 0; i < count; i++)
            {
                var target = _attackers[i];
                target.TakeDamage(_model.Damage);
                _view.PlayAttackAnimation();

                if (target.IsDead)
                    defeatedHumans.Add(target);
            }

            foreach (var h in defeatedHumans)
                _attackers.Remove(h);
        }
    }
}
