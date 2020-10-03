using System.Collections.Generic;

namespace Paps.StateMachines
{
    internal class HierarchicalTransitionValidator<TState, TTrigger>
    {
        private Dictionary<Transition<TState, TTrigger>, List<IGuardCondition>> _guardConditions;

        private readonly StateHierarchyBehaviourScheduler<TState, TTrigger> _stateHierarchyBehaviourScheduler;
        private readonly StateHierarchy<TState, TTrigger> _stateHierarchy;
        private readonly IEqualityComparer<TState> _stateComparer;
        private IEqualityComparer<Transition<TState, TTrigger>> _transitionEqualityComparer;

        public HierarchicalTransitionValidator(IEqualityComparer<TState> stateComparer, IEqualityComparer<Transition<TState, TTrigger>> transitionEqualityComparer, 
            StateHierarchyBehaviourScheduler<TState, TTrigger> stateHierarchyBehaviourScheduler, 
            StateHierarchy<TState, TTrigger> stateHierarchy)
        {
            _stateHierarchyBehaviourScheduler = stateHierarchyBehaviourScheduler;
            _stateHierarchy = stateHierarchy;
            _stateComparer = stateComparer;
            _transitionEqualityComparer = transitionEqualityComparer;

            _guardConditions = new Dictionary<Transition<TState, TTrigger>, List<IGuardCondition>>(_transitionEqualityComparer);
        }

        public void AddGuardConditionTo(Transition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            if (_guardConditions.ContainsKey(transition) == false)
                _guardConditions.Add(transition, new List<IGuardCondition>());

            _guardConditions[transition].Add(guardCondition);
        }

        public bool RemoveGuardConditionFrom(Transition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            if (_guardConditions.ContainsKey(transition))
            {
                bool removed = _guardConditions[transition].Remove(guardCondition);

                if (_guardConditions[transition].Count == 0)
                    _guardConditions.Remove(transition);

                return removed;
            }

            return false;
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
            if (_guardConditions.ContainsKey(transition))
                return _guardConditions[transition].ToArray();
            else
                return null;
        }

        public bool IsValid(Transition<TState, TTrigger> transition)
        {
            if(!IsValidTarget(transition))
                return false;

            if (_guardConditions.ContainsKey(transition))
            {
                var guardConditionsList = _guardConditions[transition];

                for(int i = 0; i < guardConditionsList.Count; i++)
                {
                    if (guardConditionsList[i].IsValid() == false)
                        return false;
                }
            }

            return true;
        }

        private bool IsValidTarget(Transition<TState, TTrigger> transition)
        {
            return SourceStateIsInActiveHierarchy(transition.SourceState) && 
                (TargetStateIsEqualsToSource(transition.SourceState, transition.TargetState) || 
                TargetIsSiblingOfSource(transition.SourceState, transition.TargetState));
        }

        private bool SourceStateIsInActiveHierarchy(TState sourceState)
        {
            return _stateHierarchyBehaviourScheduler.IsInState(sourceState);
        }

        private bool TargetStateIsEqualsToSource(TState sourceState, TState targetState)
        {
            return _stateComparer.Equals(sourceState, targetState);
        }

        private bool TargetIsSiblingOfSource(TState sourceState, TState targetState)
        {
            return _stateHierarchy.AreSiblings(sourceState, targetState);
        }
    }

}