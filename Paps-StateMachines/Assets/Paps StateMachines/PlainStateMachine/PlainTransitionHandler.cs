using System.Collections.Generic;
using System.Linq;

namespace Paps.StateMachines
{
    internal class PlainTransitionHandler<TState, TTrigger>
    {
        public int TransitionCount => _transitions.Count;

        private IEqualityComparer<TState> _stateComparer;
        private IEqualityComparer<TTrigger> _triggerComparer;

        private IPlainTransitionValidator<TState, TTrigger> _transitionValidator;

        private HashSet<Transition<TState, TTrigger>> _transitions = new HashSet<Transition<TState, TTrigger>>();

        private PlainStateBehaviourScheduler<TState> _stateBehaviourScheduler;

        public StateChanged<TState, TTrigger> OnBeforeStateChanges;
        public StateChanged<TState, TTrigger> OnStateChanged;

        public PlainTransitionHandler(IEqualityComparer<TState> stateComparer, IEqualityComparer<TTrigger> triggerComparer,
            IEqualityComparer<Transition<TState, TTrigger>> transitionComparer,
            PlainStateBehaviourScheduler<TState> stateBehaviourScheduler, IPlainTransitionValidator<TState, TTrigger> transitionValidator)
        {
            _stateComparer = stateComparer;
            _triggerComparer = triggerComparer;
            _transitionValidator = transitionValidator;
            _stateBehaviourScheduler = stateBehaviourScheduler;
        }

        public void AddTransition(Transition<TState, TTrigger> transition)
        {
            _transitions.Add(transition);
        }

        public bool RemoveTransition(Transition<TState, TTrigger> transition)
        {
            return _transitions.Remove(transition);
        }

        public bool ContainsTransition(Transition<TState, TTrigger> transition)
        {
            return _transitions.Contains(transition);
        }

        public Transition<TState, TTrigger>[] GetTransitions()
        {
            return _transitions.ToArray();
        }

        public bool Trigger(TTrigger trigger)
        {
            if (!_stateBehaviourScheduler.CanSwitch())
                return false;

            if (TryGetStateTo(trigger, out TState stateTo))
            {
                OnBeforeStateChanges?.Invoke(_stateBehaviourScheduler.CurrentState.Value, trigger, stateTo);
                _stateBehaviourScheduler.SwitchTo(stateTo, 
                    () => OnStateChanged?.Invoke(_stateBehaviourScheduler.CurrentState.Value, trigger, stateTo));

                return true;
            }

            return false;
        }

        private bool TryGetStateTo(TTrigger trigger, out TState stateTo)
        {
            stateTo = default;

            bool modifiedFlag = false;
            bool multipleValidGuardsFlag = false;

            foreach (Transition<TState, TTrigger> transition in _transitions)
            {
                if (_stateComparer.Equals(transition.StateFrom, _stateBehaviourScheduler.CurrentState.Value)
                    && _triggerComparer.Equals(transition.Trigger, trigger)
                    && _transitionValidator.IsValid(transition))
                {
                    if (multipleValidGuardsFlag)
                    {
                        throw new MultipleValidTransitionsFromSameStateException(
                            _stateBehaviourScheduler.CurrentState.Value, trigger, stateTo, transition.StateTo);
                    }

                    stateTo = transition.StateTo;

                    modifiedFlag = true;
                    multipleValidGuardsFlag = true;
                }
            }

            return modifiedFlag;
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

            foreach(var transition in _transitions)
            {
                if(_stateComparer.Equals(transition.StateFrom, stateId) ||
                    _stateComparer.Equals(transition.StateTo, stateId))
                {
                    list.Add(transition);
                }
            }

            return list;
        }
    }
}