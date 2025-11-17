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
        private readonly HumanPresenter _humanPrefab;

        private int _spawnedHumansCount;

        public override bool HasFreeSlot() => true;

        public SpawnState(GameManager gameManager, HumanPresenter humanPrefab) : base(
            gameManager)
        {
            _humanPrefab = humanPrefab;
        }

        protected override void Enter(HumanPresenter humanView)
        {
            SpawnHuman();
        }

        private void SpawnHuman()
        {
            var human = GameObject.Instantiate(_humanPrefab, new Vector3(0F, -4.8F, 0F), Quaternion.identity);
            human.Initialize(gameManager);

            gameManager.RegisterHuman(human);

            human.OnHumanDied += OnHumanDied;

            gameManager.TransitionTo<PortalState>(human);


            Observable.Interval(TimeSpan.FromMilliseconds(1500))
                .Where(_ => gameManager.FreeSlotIn<PortalState>())
                .Subscribe(_ => SpawnHuman());
        }

        private void OnHumanDied(HumanPresenter human)
        {
            human.OnHumanDied -= OnHumanDied;
            gameManager.UnregisterHuman(human);
        }
    }
}