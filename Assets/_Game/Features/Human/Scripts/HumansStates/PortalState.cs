using System;
using _Game.Features.Humans;
using _Game.Features.HumansState.Scripts.Core;
using _Game.Features.HumansState.Scripts.Waiting;
using UniRx;
using UnityEngine;

namespace _Game.Features.HumansState.Scripts.Portal
{
    public class PortalState : HumanState
    {
        private readonly CompositeDisposable _disposable = new();
        private readonly Vector3 _startingPosition = new(0, -4.80f, 0);

        private bool _freeSlot = true;

        public override bool HasFreeSlot() => _freeSlot;

        public PortalState(HumanStateController humanStateController) : base(humanStateController)
        {
        }

        protected override void Enter(HumanPresenter humanView)
        {
            _freeSlot = false;

            Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(2))
                .Where(_ => humanStateController.FreeSlotIn<WaitingState>())
                .Subscribe(_ => MoveToWaiting(humanView))
                .AddTo(_disposable);
        }

        private void MoveToWaiting(HumanPresenter humanView)
        {
            _disposable.Clear();
            humanStateController.TransitionTo<WaitingState>(humanView);
            _freeSlot = true;
        }
    }
}