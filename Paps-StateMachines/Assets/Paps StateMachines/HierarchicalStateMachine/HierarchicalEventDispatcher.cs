using System;
using System.Collections.Generic;

namespace Paps.StateMachines
{
    internal class HierarchicalEventDispatcher<TState, TTrigger>
    {
        private IEqualityComparer<TState> _stateComparer;
        private Dictionary<TState, List<IStateEventHandler>> _eventHandlers;

        private StateHierarchyBehaviourScheduler<TState, TTrigger> _stateHierarchyBehaviourScheduler;

        public HierarchicalEventDispatcher(IEqualityComparer<TState> stateComparer, StateHierarchyBehaviourScheduler<TState, TTrigger> stateHierarchyBehaviourScheduler)
        {
            _stateComparer = stateComparer ?? throw new ArgumentNullException(nameof(stateComparer));

            _eventHandlers = new Dictionary<TState, List<IStateEventHandler>>(_stateComparer);
            _stateHierarchyBehaviourScheduler = stateHierarchyBehaviourScheduler;
        }

        public void AddEventHandlerTo(TState stateId, IStateEventHandler eventHandler)
        {
            ValidateIsNotNull(eventHandler);

            if (_eventHandlers.ContainsKey(stateId) == false)
                _eventHandlers.Add(stateId, new List<IStateEventHandler>());

            _eventHandlers[stateId].Add(eventHandler);
        }

        private void ValidateIsNotNull(IStateEventHandler eventHandler)
        {
            if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));
        }

        public bool RemoveEventHandlerFrom(TState stateId, IStateEventHandler eventHandler)
        {
            if (_eventHandlers.ContainsKey(stateId))
            {
                bool removed = _eventHandlers[stateId].Remove(eventHandler);

                if (_eventHandlers[stateId].Count == 0)
                    _eventHandlers.Remove(stateId);

                return removed;
            }

            return false;
        }

        public void RemoveEventHandlersFrom(TState stateId)
        {
            _eventHandlers.Remove(stateId);
        }

        public bool ContainsEventHandlerOn(TState stateId, IStateEventHandler eventHandler)
        {
            if (_eventHandlers.ContainsKey(stateId))
                return _eventHandlers[stateId].Contains(eventHandler);

            return false;
        }

        public IStateEventHandler[] GetEventHandlersOf(TState stateId)
        {
            if (_eventHandlers.ContainsKey(stateId))
                return _eventHandlers[stateId].ToArray();
            else
                return null;
        }

        public bool SendEvent(IEvent ev)
        {
            var activeHierarchyPath = _stateHierarchyBehaviourScheduler.GetActiveHierarchyPath();

            foreach(var stateIdWithObject in activeHierarchyPath)
            {
                if (_eventHandlers.ContainsKey(stateIdWithObject.Key))
                {
                    var eventHandlers = _eventHandlers[stateIdWithObject.Key];

                    foreach (var eventHandler in _eventHandlers[stateIdWithObject.Key])
                    {
                        if (eventHandler.HandleEvent(ev))
                            return true;
                    }
                }
            }

            return false;
        }
    }
}