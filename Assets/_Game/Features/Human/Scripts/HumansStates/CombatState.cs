using System.Collections.Generic;
using _Game.Features.Bosses;
using _Game.Features.Humans;
using _Game.Features.HumansState.Scripts.Core;
using _Game.Infrastructure;
using UnityEngine;

namespace _Game.Features.HumansState.Scripts.Combat
{
    public class CombatState : HumanState
    {
        private BossPresenter _currentBossView;
        private readonly BossPresenter _bossPrefab;
        private int _lastBossHp = 100;
        private readonly EventBus _bus;

        public override bool HasFreeSlot() => true;

        public CombatState(GameManager gameManager, BossPresenter bossPrefab) : base(gameManager)
        {
            _bossPrefab = bossPrefab;
            _bus = gameManager.EventBus;

            _bus.Subscribe<BossDefeatedEvent>(OnBossDefeated);

            SpawnBoss(new List<HumanPresenter>());
        }

        protected override void Enter(HumanPresenter humanView)
        {
            var position = new Vector3(Random.Range(-1f, 1f), 3f, 0f);
            humanView.MoveTo(position, StartToCombat);
        }

        private void StartToCombat(HumanPresenter humanView)
        {
            if (_currentBossView == null) return;

            _currentBossView.RegisterAttacker(humanView);
            humanView.StartAttacking(_currentBossView);
        }

        private void SpawnBoss(List<HumanPresenter> attackers)
        {
            var clone = GameObject.Instantiate(_bossPrefab, new Vector3(0f, 2.93f, 0f), Quaternion.identity);
            clone.Initialize(_lastBossHp, 1.05f, 1, 5, gameManager.EventBus);

            _lastBossHp *= 2;
            _currentBossView = clone;

            attackers.ForEach(StartToCombat);
        }

        private void OnBossDefeated(BossDefeatedEvent e)
        {
            _currentBossView = null;

            var attackers = new List<HumanPresenter>(e.Attackers);
            SpawnBoss(attackers);
        }
    }
}