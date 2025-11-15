using System;
using System.Collections.Generic;
using _Game.Features.Humans;
using _Game.Features.HumansState.Scripts.Core;
using _Game.Features.HumansState.Scripts.Training;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Game.Features.HumansState.Scripts.Waiting
{
    public class WaitingState : HumanState
    {
        private readonly Dictionary<int, CompositeDisposable> _disposables = new();

        public override bool HasFreeSlot() => true;

        public WaitingState(HumanStateController humanStateController) : base(humanStateController)
        {
        }

        protected override void Enter(HumanView humanView)
        {
            var position = new Vector3(Random.Range(-1f, 1f), -4, 0);
            humanView.MoveTo(position, OnReachedWaitingSlot);
        }

        private void OnReachedWaitingSlot(HumanView humanView)
        {
            _disposables.Add(humanView.GetInstanceID(), new CompositeDisposable());
            Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0.5f))
                .Where(_ => humanStateController.FreeSlotIn<TrainingState>())
                .Subscribe(_ => FreeGridSlotAndMoveHumanToCombatPreparation(humanView))
                .AddTo(_disposables[humanView.GetInstanceID()]);
        }

        private void FreeGridSlotAndMoveHumanToCombatPreparation(HumanView humanView)
        {
            _disposables[humanView.GetInstanceID()].Dispose();
            _disposables.Remove(humanView.GetInstanceID());
            humanStateController.TransitionTo<TrainingState>(humanView);
        }
    }
}