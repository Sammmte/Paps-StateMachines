using System.Collections.Generic;

namespace Paps.StateMachines
{
    internal class PlainTransitionValidator<TState, TTrigger>
    {
        private readonly Dictionary<Transition<TState, TTrigger>, List<IGuardCondition>> _guardConditions;
        private readonly IPlainStateMachine<TState, TTrigger> _stateMachine;

        private bool _isAddLocked;
        private bool _isRemoveLocked;

        public PlainTransitionValidator(IPlainStateMachine<TState, TTrigger> stateMachine, IEqualityComparer<Transition<TState, TTrigger>> transitionComparer)
        {
            _guardConditions = new Dictionary<Transition<TState, TTrigger>, List<IGuardCondition>>(transitionComparer);
            _stateMachine = stateMachine;
        }

        public void AddGuardConditionTo(Transition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            ValidateCanAddGuardCondition(transition, guardCondition);

            if (_guardConditions.ContainsKey(transition) == false)
                _guardConditions.Add(transition, new List<IGuardCondition>());

            _guardConditions[transition].Add(guardCondition);
        }

        private void ValidateCanAddGuardCondition(Transition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            if (_guardConditions.ContainsKey(transition) && _isAddLocked)
                throw new UnableToAddStateMachineElementException(_stateMachine, guardCondition);
        }

        public bool RemoveGuardConditionFrom(Transition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            ValidateCanRemoveGuardCondition(transition, guardCondition);

            if (_guardConditions.ContainsKey(transition))
            {
                bool removed = _guardConditions[transition].Remove(guardCondition);

                if (_guardConditions[transition].Count == 0)
                    _guardConditions.Remove(transition);

                return removed;
            }

            return false;
        }

        private void ValidateCanRemoveGuardCondition(Transition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            if (_guardConditions.ContainsKey(transition) && 
                _guardConditions[transition].Contains(guardCondition) && 
                _isRemoveLocked)
                throw new UnableToRemoveStateMachineElementException(_stateMachine, guardCondition);
        }

        public void RemoveAllGuardConditionsFrom(Transition<TState, TTrigger> transition)
        {
            _guardConditions.Remove(transition);
        }

        public void RemoveAllGuardConditionsFrom(List<Transition<TState, TTrigger>> transitions)
        {
            foreach (var transition in transitions)
                _guardConditions.Remove(transition);
        }

        public bool ContainsGuardConditionOn(Transition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            if (_guardConditions.ContainsKey(transition))
                return _guardConditions[transition].Contains(guardCondition);
            else
                return false;
        }

        public IGuardCondition[] GetGuardConditionsOf(Transition<TState, TTrigger> transition)
        {
            return _guardConditions[transition].ToArray();
        }

        public bool IsValid(Transition<TState, TTrigger> transition)
        {
            if (_guardConditions.ContainsKey(transition))
            {
                var guardConditionsList = _guardConditions[transition];

                for (int i = 0; i < guardConditionsList.Count; i++)
                {
                    if (guardConditionsList[i].IsValid() == false)
                        return false;
                }
            }

            return true;
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
    }
}