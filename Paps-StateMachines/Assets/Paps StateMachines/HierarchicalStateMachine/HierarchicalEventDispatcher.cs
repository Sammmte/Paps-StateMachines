using System;
using System.Collections.Generic;

namespace Paps.StateMachines
{
    internal class HierarchicalEventDispatcher<TState, TTrigger>
    {
        private IEqualityComparer<TState> _stateComparer;
        private readonly HierarchicalStateEventHandlerCollection<TState, TTrigger> _eventHandlers;
        private StateHierarchyBehaviourScheduler<TState, TTrigger> _stateHierarchyBehaviourScheduler;

        public HierarchicalEventDispatcher(IEqualityComparer<TState> stateComparer,
            HierarchicalStateEventHandlerCollection<TState, TTrigger> eventHandlers,
            StateHierarchyBehaviourScheduler<TState, TTrigger> stateHierarchyBehaviourScheduler)
        {
            _stateComparer = stateComparer ?? throw new ArgumentNullException(nameof(stateComparer));

            _eventHandlers = eventHandlers;
            _stateHierarchyBehaviourScheduler = stateHierarchyBehaviourScheduler;
        }

        public bool SendEvent(IEvent ev)
        {
            var activeHierarchyPath = _stateHierarchyBehaviourScheduler.GetActiveHierarchyPath();

            foreach(var stateIdWithObject in activeHierarchyPath)
            {
                var eventHandlers = _eventHandlers.GetEventHandlersOf(stateIdWithObject.Key);

                foreach (var eventHandler in eventHandlers)
                {
                    if (eventHandler.HandleEvent(ev))
                        return true;
                }
            }

            return false;
        }
    }
}