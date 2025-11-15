using System.Collections.Generic;
using _Game.Features.Bosses;
using _Game.Features.Humans;
using _Game.Features.HumansState.Scripts.Core;
using UnityEngine;

namespace _Game.Features.HumansState.Scripts.Combat
{
    public class CombatState : HumanState
    {
        private readonly BossView _bossPrefab;

        private BossView _currentBossView;

        public override bool HasFreeSlot() => true;

        public CombatState(HumanStateController humanStateController, BossView bossPrefab) : base(
            humanStateController)
        {
            _bossPrefab = bossPrefab;

            SpawnBoss(new List<HumanView>());
        }

        protected override void Enter(HumanView humanView)
        {
            var position = new Vector3(Random.Range(-1f, 1f), 3, 0);
            humanView.MoveTo(position, StartToCombat);
        }

        private void StartToCombat(HumanView humanView)
        {
            _currentBossView.RegisterAttacker(humanView);
            humanView.StartAttacking(_currentBossView);
            _currentBossView.RegisterAttacker(humanView);
        }

        private void SpawnBoss(List<HumanView> attackers)
        {
            var clone = GameObject.Instantiate(_bossPrefab, new Vector3(0, 3.93f, 0), Quaternion.identity);
            clone.Initialize(100, 1.05f, 1, 5);
            _currentBossView = clone;
            attackers.ForEach(StartToCombat);
            _currentBossView.BossDefeatedCallback += HandleBossDefeated;
        }

        private void HandleBossDefeated(List<HumanView> attackers)
        {
            _currentBossView = null;
            SpawnBoss(attackers);
        }
    }
}