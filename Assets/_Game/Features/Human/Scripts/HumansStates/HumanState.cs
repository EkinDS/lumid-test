using _Game.Features.Humans;

namespace _Game.Features.HumansState.Scripts.Core
{
    public abstract class HumanState
    {
        protected readonly GameManager gameManager;

        protected abstract void Enter(HumanPresenter humanView);
        public abstract bool HasFreeSlot();

        protected HumanState(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void OnEnter(HumanPresenter humanView)
        {
            Enter(humanView);
        }
    }
}