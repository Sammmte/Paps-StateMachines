using System;

namespace Paps.StateMachines
{
    public interface IEventDispatcherStateMachine<TState, TTrigger> : IStateMachine<TState, TTrigger>
    {
        void SubscribeEventHandlerTo(TState stateId, IStateEventHandler eventHandler);
        bool UnsubscribeEventHandlerFrom(TState stateId, IStateEventHandler eventHandler);

        bool HasEventHandlerOn(TState stateId, IStateEventHandler eventHandler);

        IStateEventHandler[] GetEventHandlersOf(TState stateId);

        void SendEvent(IEvent ev, Action<bool> callback = null);
    }
}