using System.Collections.Generic;
using System.Linq;

namespace Paps.StateMachines
{
    internal class PlainTransitionHandler<TState, TTrigger>
    {
        public int TransitionCount => _transitions.Count;

        private IEqualityComparer<TState> _stateComparer;
        private IEqualityComparer<TTrigger> _triggerComparer;

        private PlainTransitionValidator<TState, TTrigger> _transitionValidator;

        private HashSet<Transition<TState, TTrigger>> _transitions = new HashSet<Transition<TState, TTrigger>>();

        private PlainStateBehaviourScheduler<TState> _stateBehaviourScheduler;

        public StateChanged<TState, TTrigger> OnBeforeStateChanges;
        public StateChanged<TState, TTrigger> OnStateChanged;

        public PlainTransitionHandler(IEqualityComparer<TState> stateComparer, IEqualityComparer<TTrigger> triggerComparer,
            IEqualityComparer<Transition<TState, TTrigger>> transitionComparer,
            PlainStateBehaviourScheduler<TState> stateBehaviourScheduler)
        {
            _stateComparer = stateComparer;
            _triggerComparer = triggerComparer;
            _transitionValidator = new PlainTransitionValidator<TState, TTrigger>(transitionComparer);
            _stateBehaviourScheduler = stateBehaviourScheduler;
        }

        public void AddTransition(Transition<TState, TTrigger> transition)
        {
            _transitions.Add(transition);
        }

        public bool RemoveTransition(Transition<TState, TTrigger> transition)
        {
            if(_transitions.Remove(transition))
            {
                _transitionValidator.RemoveAllGuardConditionsFrom(transition);
                return true;
            }

            return false;
        }

        public bool ContainsTransition(Transition<TState, TTrigger> transition)
        {
            return _transitions.Contains(transition);
        }

        public Transition<TState, TTrigger>[] GetTransitions()
        {
            return _transitions.ToArray();
        }

        public void Trigger(TTrigger trigger)
        {
            if (!_stateBehaviourScheduler.CanSwitch())
                return;

            if (TryGetStateTo(trigger, out TState stateTo))
            {
                OnBeforeStateChanges?.Invoke(_stateBehaviourScheduler.CurrentState.Value, trigger, stateTo);
                _stateBehaviourScheduler.SwitchTo(stateTo, 
                    () => OnStateChanged?.Invoke(_stateBehaviourScheduler.CurrentState.Value, trigger, stateTo));
            }
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

        public void AddGuardConditionTo(Transition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            ValidateContainsTransition(transition);

            _transitionValidator.AddGuardConditionTo(transition, guardCondition);
        }

        private void ValidateContainsTransition(Transition<TState, TTrigger> transition)
        {
            if (!ContainsTransition(transition))
                throw new TransitionNotAddedException(transition.StateFrom, transition.Trigger, transition.StateTo);
        }

        public bool RemoveGuardConditionFrom(Transition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            ValidateContainsTransition(transition);

            return _transitionValidator.RemoveGuardConditionFrom(transition, guardCondition);
        }

        public bool ContainsGuardConditionOn(Transition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            return _transitionValidator.ContainsGuardConditionOn(transition, guardCondition);
        }

        public IGuardCondition[] GetGuardConditionsOf(Transition<TState, TTrigger> transition)
        {
            ValidateContainsTransition(transition);

            return _transitionValidator.GetGuardConditionsOf(transition);
        }
    }
}