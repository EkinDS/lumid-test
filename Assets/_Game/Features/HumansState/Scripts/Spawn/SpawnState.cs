using System;
using _Game.Features.Humans;
using _Game.Features.HumansState.Scripts.Core;
using _Game.Features.HumansState.Scripts.Portal;
using UniRx;
using UnityEngine;

namespace _Game.Features.HumansState.Scripts.Spawn
{
    public class SpawnState : HumanState
    {
        private readonly HumanView _humanPrefab;

        private int _spawnedHumansCount;

        public override bool HasFreeSlot() => true;

        public SpawnState(HumanStateController humanStateController, HumanView humanPrefab) : base(
            humanStateController)
        {
            _humanPrefab = humanPrefab;
        }

        protected override void Enter(HumanView humanView)
        {
            SpawnHuman();
        }

        private void SpawnHuman()
        {
            var humanView = GameObject.Instantiate(_humanPrefab, Vector3.zero, Quaternion.identity);
            humanView.Initialize(_spawnedHumansCount++);
            humanStateController.TransitionTo<PortalState>(humanView);

            Observable.Interval(TimeSpan.FromMilliseconds(1000))
                .Where(_ => humanStateController.FreeSlotIn<PortalState>())
                .Subscribe(_ => SpawnHuman());
        }
    }
}