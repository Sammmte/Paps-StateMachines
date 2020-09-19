using System;

namespace Paps.StateMachines.Extensions
{
    public class DelegateState : IState
    {
        protected Action onEnter;
        protected Action onUpdate;
        protected Action onExit;

        public DelegateState(Action onEnter, Action onUpdate, Action onExit)
        {
            this.onEnter = onEnter;
            this.onUpdate = onUpdate;
            this.onExit = onExit;
        }

        public void Enter()
        {
            onEnter?.Invoke();
        }

        public void Update()
        {
            onUpdate?.Invoke();
        }

        public void Exit()
        {
            onExit?.Invoke();
        }
    }
}
