using Paps.Maybe;
using System;

namespace Paps.StateMachines
{
    public interface IStateMachine<TState, TTrigger>
    {
        int StateCount { get; }
        int TransitionCount { get; }

        Maybe<TState> InitialState { get; }

        void SetInitialState(TState stateId);

        void AddState(TState stateId, IState stateObject);
        bool RemoveState(TState stateId);

        bool ContainsState(TState stateId);

        TState[] GetStates();

        void AddTransition(Transition<TState, TTrigger> transition);
        bool RemoveTransition(Transition<TState, TTrigger> transition);

        bool ContainsTransition(Transition<TState, TTrigger> transition);

        Transition<TState, TTrigger>[] GetTransitions();

        IState GetStateObjectById(TState stateId);

        void Trigger(TTrigger trigger, Action<bool> callback = null);

        bool IsInState(TState stateId);
    }
}