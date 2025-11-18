using System;
using _Game.Features.Humans;
using _Game.Features.HumansState.Scripts.Core;
using _Game.Features.HumansState.Scripts.Portal;
using _Game.Infrastructure;
using UniRx;
using UnityEngine;

namespace _Game.Features.HumansState.Scripts.Spawn
{
    public class SpawnState : HumanState
    {
        readonly HumanPresenter _humanPrefab;
        readonly EventBus _bus;

        int _spawnedHumansCount;
        IDisposable _spawnSubscription;

        public override bool HasFreeSlot() => true;

        public SpawnState(GameManager gameManager, HumanPresenter humanPrefab) : base(gameManager)
        {
            _humanPrefab = humanPrefab;
            _bus = gameManager.EventBus;

            _bus.Subscribe<HumanDiedEvent>(OnHumanDied);

            _spawnSubscription = Observable.Interval(TimeSpan.FromMilliseconds(1500))
                .Where(_ => gameManager.FreeSlotIn<PortalState>())
                .Subscribe(_ => SpawnHuman());
        }

        protected override void Enter(HumanPresenter humanView)
        {
            SpawnHuman();
        }

        void SpawnHuman()
        {
            var human = GameObject.Instantiate(_humanPrefab, new Vector3(0f, -4.8f, 0f), Quaternion.identity);
            human.Initialize(gameManager);

            gameManager.RegisterHuman(human);
            gameManager.TransitionTo<PortalState>(human);
        }

        void OnHumanDied(HumanDiedEvent e)
        {
            gameManager.UnregisterHuman(e.Human);
        }
    }
}