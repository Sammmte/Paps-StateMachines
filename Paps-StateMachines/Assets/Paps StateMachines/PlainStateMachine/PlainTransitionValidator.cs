using System;
using System.Collections.Generic;

namespace Paps.StateMachines
{
    internal class PlainTransitionValidator<TState, TTrigger> : IPlainTransitionValidator<TState, TTrigger>
    {
        private Dictionary<Transition<TState, TTrigger>, List<IGuardCondition>> _guardConditions;

        public PlainTransitionValidator(IEqualityComparer<Transition<TState, TTrigger>> transitionComparer)
        {
            _guardConditions = new Dictionary<Transition<TState, TTrigger>, List<IGuardCondition>>(transitionComparer);
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
    }
}