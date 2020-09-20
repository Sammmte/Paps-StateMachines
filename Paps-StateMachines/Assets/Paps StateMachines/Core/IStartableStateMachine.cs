using System;

namespace Paps.StateMachines
{
    public interface IStartableStateMachine<TState, TTrigger> : IStateMachine<TState, TTrigger>
    {
        bool IsStarted { get; }

        void Start(Action callback = null);
        void Stop(Action callback = null);
    }
}