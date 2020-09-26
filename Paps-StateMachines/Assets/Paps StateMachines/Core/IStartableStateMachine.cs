using System;

namespace Paps.StateMachines
{
    public interface IStartableStateMachine<TState, TTrigger> : IStateMachine<TState, TTrigger>
    {
        bool IsRunning { get; }

        void Start(Action callback = null);
        void Stop(Action callback = null);
    }
}