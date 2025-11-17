using System;
using _Game.Features.Humans;
using _Game.Features.HumansState.Scripts.Core;
using _Game.Features.HumansState.Scripts.Waiting;
using UniRx;

namespace _Game.Features.HumansState.Scripts.Portal
{
    public class PortalState : HumanState
    {
        private readonly CompositeDisposable _disposable = new();

        private bool _freeSlot = true;

        public override bool HasFreeSlot() => _freeSlot;

        public PortalState(GameManager gameManager) : base(gameManager)
        {
        }

        protected override void Enter(HumanPresenter humanView)
        {
            _freeSlot = false;

            Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(2))
                .Where(_ => gameManager.FreeSlotIn<WaitingState>())
                .Subscribe(_ => MoveToWaiting(humanView))
                .AddTo(_disposable);
        }

        private void MoveToWaiting(HumanPresenter humanView)
        {
            _disposable.Clear();
            gameManager.TransitionTo<WaitingState>(humanView);
            _freeSlot = true;
        }
    }
}