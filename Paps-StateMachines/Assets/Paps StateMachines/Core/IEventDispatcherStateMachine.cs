using System;

namespace Paps.StateMachines
{
    public interface IEventDispatcherStateMachine<TState, TTrigger> : IStateMachine<TState, TTrigger>
    {
        void AddEventHandlerTo(TState stateId, IStateEventHandler eventHandler);
        bool RemoveEventHandlerFrom(TState stateId, IStateEventHandler eventHandler);

        bool ContainsEventHandlerOn(TState stateId, IStateEventHandler eventHandler);

        IStateEventHandler[] GetEventHandlersOf(TState stateId);

        void SendEvent(IEvent ev, Action<bool> callback = null);
    }
}