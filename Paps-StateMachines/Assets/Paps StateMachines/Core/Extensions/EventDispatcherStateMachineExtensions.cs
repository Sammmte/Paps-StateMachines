using System;

namespace Paps.StateMachines.Extensions
{
    public static class EventDispatcherStateMachineExtensions
    {
        public static void SubscribeEventHandlerTo<TState, TTrigger>(this IEventDispatcherStateMachine<TState, TTrigger> fsm, TState stateId, Func<IEvent, bool> method)
        {
            fsm.AddEventHandlerTo(stateId, new DelegateStateEventHandler(method));
        }

        public static void UnsubscribeEventHandlerFrom<TState, TTrigger>(this IEventDispatcherStateMachine<TState, TTrigger> fsm, TState stateId, Func<IEvent, bool> method)
        {
            fsm.RemoveEventHandlerFrom(stateId, new DelegateStateEventHandler(method));
        }

        public static bool HasEventHandler<TState, TTrigger>(this IEventDispatcherStateMachine<TState, TTrigger> fsm, TState stateId, Func<IEvent, bool> method)
        {
            return fsm.ContainsEventHandlerOn(stateId, new DelegateStateEventHandler(method));
        }

        public static void SubscribeEventHandlersTo<TState, TTrigger>(this IEventDispatcherStateMachine<TState, TTrigger> fsm, TState stateId, params IStateEventHandler[] eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                fsm.AddEventHandlerTo(stateId, eventHandler);
            }
        }

        public static void SubscribeEventHandlersTo<TState, TTrigger>(this IEventDispatcherStateMachine<TState, TTrigger> fsm, TState stateId, params Func<IEvent, bool>[] methods)
        {
            foreach(var method in methods)
            {
                fsm.AddEventHandlerTo(stateId, new DelegateStateEventHandler(method));
            }
        }
    }
}