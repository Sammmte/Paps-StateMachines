using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Paps.StateMachines
{
    internal class HierarchicalTransitionCollection<TState, TTrigger> : IEnumerable<Transition<TState, TTrigger>>
    {
        public int TransitionCount => _transitions.Count;

        private readonly HashSet<Transition<TState, TTrigger>> _transitions;
        private readonly IHierarchicalStateMachine<TState, TTrigger> _stateMachine;
        private readonly IEqualityComparer<TState> _stateComparer;

        private bool _isAddLocked;
        private bool _isRemoveLocked;

        public HierarchicalTransitionCollection(IHierarchicalStateMachine<TState, TTrigger> stateMachine, 
            IEqualityComparer<TState> stateComparer,
            IEqualityComparer<Transition<TState, TTrigger>> transitionEqualityComparer)
        {
            _transitions = new HashSet<Transition<TState, TTrigger>>(transitionEqualityComparer);
            _stateMachine = stateMachine;
            _stateComparer = stateComparer;
        }

        public void AddTransition(Transition<TState, TTrigger> transition)
        {
            ValidateCanAddTransition(transition);

            _transitions.Add(transition);
        }

        private void ValidateCanAddTransition(Transition<TState, TTrigger> transition)
        {
            if (_isAddLocked)
                throw new UnableToAddStateMachineElementException(_stateMachine, transition);
        }

        public bool RemoveTransition(Transition<TState, TTrigger> transition)
        {
            ValidateCanRemoveTransition(transition);

            return _transitions.Remove(transition);
        }

        private void ValidateCanRemoveTransition(Transition<TState, TTrigger> transition)
        {
            if (_transitions.Contains(transition) && _isRemoveLocked)
                throw new UnableToRemoveStateMachineElementException(_stateMachine, transition);
        }

        public bool ContainsTransition(Transition<TState, TTrigger> transition)
        {
            return _transitions.Contains(transition);
        }

        public Transition<TState, TTrigger>[] GetTransitions()
        {
            return _transitions.ToArray();
        }

        public List<Transition<TState, TTrigger>> RemoveTransitionsRelatedTo(TState stateId)
        {
            var toRemoveTransitions = GetTransitionsRelatedTo(stateId);

            foreach (var transition in toRemoveTransitions)
                _transitions.Remove(transition);

            return toRemoveTransitions;
        }

        private List<Transition<TState, TTrigger>> GetTransitionsRelatedTo(TState stateId)
        {
            var list = new List<Transition<TState, TTrigger>>();

            foreach (var transition in _transitions)
            {
                if (_stateComparer.Equals(transition.StateFrom, stateId) ||
                    _stateComparer.Equals(transition.StateTo, stateId))
                {
                    list.Add(transition);
                }
            }

            return list;
        }

        public void Lock()
        {
            _isAddLocked = true;
            _isRemoveLocked = true;
        }

        public void Unlock()
        {
            _isAddLocked = false;
            _isRemoveLocked = false;
        }

        public IEnumerator<Transition<TState, TTrigger>> GetEnumerator()
        {
            return _transitions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}