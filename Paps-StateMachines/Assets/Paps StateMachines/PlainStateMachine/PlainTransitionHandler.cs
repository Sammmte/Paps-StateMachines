using System.Collections.Generic;
using System.Linq;

namespace Paps.StateMachines
{
    internal class PlainTransitionHandler<TState, TTrigger>
    {
        public int TransitionCount => _transitions.Count;

        private readonly IPlainStateMachine<TState, TTrigger> _stateMachine;
        private IEqualityComparer<TState> _stateComparer;
        private IEqualityComparer<TTrigger> _triggerComparer;

        private IPlainTransitionValidator<TState, TTrigger> _transitionValidator;

        private HashSet<Transition<TState, TTrigger>> _transitions = new HashSet<Transition<TState, TTrigger>>();

        private PlainStateBehaviourScheduler<TState, TTrigger> _stateBehaviourScheduler;

        public event StateChanged<TState, TTrigger> OnBeforeStateChanges;
        public event StateChanged<TState, TTrigger> OnStateChanged;

        public PlainTransitionHandler(IPlainStateMachine<TState, TTrigger> stateMachine, IEqualityComparer<TState> stateComparer, IEqualityComparer<TTrigger> triggerComparer,
            IEqualityComparer<Transition<TState, TTrigger>> transitionComparer,
            PlainStateBehaviourScheduler<TState, TTrigger> stateBehaviourScheduler, IPlainTransitionValidator<TState, TTrigger> transitionValidator)
        {
            _stateMachine = stateMachine;
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
                var stateFrom = _stateBehaviourScheduler.CurrentState.Value;
                NotifyBeforeStateChangesEvent(stateFrom, trigger, stateTo);
                _stateBehaviourScheduler.SwitchTo(stateTo,
                    () => NotifyStateChangedEvent(stateFrom, trigger, stateTo));

                return true;
            }

            return false;
        }

        private void NotifyBeforeStateChangesEvent(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            OnBeforeStateChanges?.Invoke(stateFrom, trigger, stateTo);
        }

        private void NotifyStateChangedEvent(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            if(OnStateChanged != null)
                OnStateChanged.Invoke(stateFrom, trigger, stateTo);
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
                        throw new MultipleValidTransitionsFromSameStateException(_stateMachine,
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