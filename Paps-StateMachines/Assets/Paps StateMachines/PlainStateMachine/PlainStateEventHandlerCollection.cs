using System;
using System.Collections.Generic;

namespace Paps.StateMachines
{
    internal class PlainStateEventHandlerCollection<TState, TTrigger>
    {
        private readonly IPlainStateMachine<TState, TTrigger> _stateMachine;
        private readonly IEqualityComparer<TState> _stateComparer;
        private readonly Dictionary<TState, List<IStateEventHandler>> _eventHandlers;
        private readonly List<TState> _lockedStates = new List<TState>();

        public PlainStateEventHandlerCollection(IPlainStateMachine<TState, TTrigger> stateMachine, IEqualityComparer<TState> stateComparer)
        {
            _stateComparer = stateComparer ?? throw new ArgumentNullException(nameof(stateComparer));
            _stateMachine = stateMachine;
            _eventHandlers = new Dictionary<TState, List<IStateEventHandler>>(_stateComparer);
        }

        public void AddEventHandlerTo(TState stateId, IStateEventHandler eventHandler)
        {
            ValidateIsNotNull(eventHandler);
            ValidateCanAddEventHandler(stateId, eventHandler);

            if (_eventHandlers.ContainsKey(stateId) == false)
                _eventHandlers.Add(stateId, new List<IStateEventHandler>());

            _eventHandlers[stateId].Add(eventHandler);
        }

        private void ValidateCanAddEventHandler(TState stateId, IStateEventHandler eventHandler)
        {
            if (IsLocked(stateId))
                throw new UnableToAddStateMachineElementException(_stateMachine, eventHandler);
        }

        private void ValidateIsNotNull(IStateEventHandler eventHandler)
        {
            if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));
        }

        public bool RemoveEventHandlerFrom(TState stateId, IStateEventHandler eventHandler)
        {
            ValidateCanRemoveEventHandler(stateId, eventHandler);

            if (_eventHandlers.ContainsKey(stateId))
            {
                bool removed = _eventHandlers[stateId].Remove(eventHandler);

                if (_eventHandlers[stateId].Count == 0)
                    _eventHandlers.Remove(stateId);

                return removed;
            }

            return false;
        }

        private void ValidateCanRemoveEventHandler(TState stateId, IStateEventHandler eventHandler)
        {
            if (IsLocked(stateId))
                throw new UnableToRemoveStateMachineElementException(_stateMachine, eventHandler);
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

        public void LockEventHandlersOf(TState stateId)
        {
            if (!IsLocked(stateId))
                _lockedStates.Add(stateId);
        }

        public void UnlockEventHandlersOf(TState stateId)
        {
            _lockedStates.Remove(stateId);
        }

        private bool IsLocked(TState stateId)
        {
            return _lockedStates.Contains(stateId);
        }
    }
}