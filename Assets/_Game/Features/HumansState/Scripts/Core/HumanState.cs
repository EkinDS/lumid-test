using _Game.Features.Humans;

namespace _Game.Features.HumansState.Scripts.Core
{
    public abstract class HumanState
    {
        protected readonly HumanStateController humanStateController;

        protected abstract void Enter(HumanView humanView);
        public abstract bool HasFreeSlot();

        protected HumanState(HumanStateController humanStateController)
        {
            this.humanStateController = humanStateController;
        }

        public void OnEnter(HumanView humanView)
        {
            Enter(humanView);
        }
    }
}