using System.Collections.Generic;
using System;

namespace Paps.StateMachines
{
    internal class StateHierarchyBehaviourScheduler<TState, TTrigger>
    {
        private readonly IHierarchicalStateMachine<TState, TTrigger> _stateMachine;
        private readonly StateHierarchy<TState, TTrigger> _stateHierarchy;
        private readonly HierarchicalTransitionCollection<TState, TTrigger> _transitions;
        private readonly HierarchicalTransitionValidator<TState, TTrigger> _transitionValidator;
        private List<KeyValuePair<TState, IState>> _activeHierarchyPath;

        private IEqualityComparer<TState> _stateComparer;
        private readonly IEqualityComparer<TTrigger> _triggerComparer;

        public event HierarchyPathChanged<TTrigger> OnBeforeActiveHierarchyPathChanges;
        public event HierarchyPathChanged<TTrigger> OnActiveHierarchyPathChanged;

        public bool IsRunning { get; private set; }

        public StateHierarchyBehaviourScheduler(IHierarchicalStateMachine<TState, TTrigger> stateMachine,
            StateHierarchy<TState, TTrigger> stateHierarchy,
            HierarchicalTransitionCollection<TState, TTrigger> transitions, 
            HierarchicalTransitionValidator<TState, TTrigger> transitionValidator,
            IEqualityComparer<TState> stateComparer,
            IEqualityComparer<TTrigger> triggerComparer)
        {
            _stateMachine = stateMachine;
            _stateHierarchy = stateHierarchy;
            _transitions = transitions;
            _transitionValidator = transitionValidator;
            _stateComparer = stateComparer;
            _triggerComparer = triggerComparer;
            _activeHierarchyPath = new List<KeyValuePair<TState, IState>>();
        }

        public void Start()
        {
            AddToActivesFrom(_stateHierarchy.InitialState.Value);

            IsRunning = true;

            EnterActivesFrom(_stateHierarchy.InitialState.Value);
        }

        private void AddToActivesFrom(TState stateId)
        {
            var previous = stateId;
            var current = _stateHierarchy.GetInitialStateOf(previous);

            _activeHierarchyPath.Add(NewKeyValueFor(previous));

            while (_stateHierarchy.AreImmediateParentAndChild(previous, current))
            {
                _activeHierarchyPath.Add(NewKeyValueFor(current));

                previous = current;
                current = _stateHierarchy.GetInitialStateOf(current);
            }
        }

        private void EnterActivesFrom(TState stateId)
        {
            for(int i = 0; i < _activeHierarchyPath.Count; i++)
            {
                if(AreEquals(_activeHierarchyPath[i].Key, stateId))
                {
                    for(int j = i; j < _activeHierarchyPath.Count; j++)
                    {
                        _activeHierarchyPath[j].Value.Enter();
                    }

                    return;
                }
            }
        }

        public void Stop()
        {
            if(IsRunning)
            {
                IsRunning = false;

                ExitActivesUntil(_activeHierarchyPath[0].Key);

                _activeHierarchyPath.Clear();
            }
        }

        private void RemoveFromActivesUntil(TState stateId)
        {
            for(int i = _activeHierarchyPath.Count - 1; i >= 0; i--)
            {
                var current = _activeHierarchyPath[i];

                _activeHierarchyPath.RemoveAt(i);

                if (AreEquals(current.Key, stateId)) return;
            }
        }

        private void ExitActivesUntil(TState stateId)
        {
            for(int i = _activeHierarchyPath.Count - 1; i >= 0; i--)
            {
                _activeHierarchyPath[i].Value.Exit();

                if (AreEquals(_activeHierarchyPath[i].Key, stateId)) return;
            }
        }

        public void Update()
        {
            if (!IsRunning)
                return;

            for(int i = 0; i < _activeHierarchyPath.Count; i++)
            {
                _activeHierarchyPath[i].Value.Update();
            }
        }

        private KeyValuePair<TState, IState> NewKeyValueFor(TState stateId)
        {
            return new KeyValuePair<TState, IState>(stateId, _stateHierarchy.GetStateObjectById(stateId));
        }

        private bool AreEquals(TState stateId1, TState stateId2)
        {
            return _stateComparer.Equals(stateId1, stateId2);
        }

        public HierarchyPath<TState> GetActiveHierarchyPath()
        {
            return new HierarchyPath<TState>(_activeHierarchyPath);
        }

        public bool IsInState(TState stateId)
        {
            for(int i = 0; i < _activeHierarchyPath.Count; i++)
            {
                if (AreEquals(_activeHierarchyPath[i].Key, stateId))
                    return true;
            }

            return false;
        }

        private void SwitchTo(TTrigger trigger, TState sourceState, TState targetState)
        {
            OnBeforeActiveHierarchyPathChanges?.Invoke(trigger);

            ExitActivesUntil(sourceState);

            RemoveFromActivesUntil(sourceState);

            AddToActivesFrom(targetState);

            OnActiveHierarchyPathChanged?.Invoke(trigger);

            EnterActivesFrom(targetState);
        }

        public bool Trigger(TTrigger trigger)
        {
            if (!IsRunning)
                return false;

            if(TryGetTargetState(trigger, out TState sourceState, out TState targetState))
            {
                SwitchTo(trigger, sourceState, targetState);

                return true;
            }

            return false;
        }

        private bool TryGetTargetState(TTrigger trigger, out TState sourceState, out TState targetState)
        {
            sourceState = default;
            targetState = default;
            Transition<TState, TTrigger> validTransition = default;
            bool hasOneValid = false;

            foreach (var transition in _transitions)
            {
                if (HasTrigger(transition, trigger) && _transitionValidator.IsValid(transition))
                {
                    if (hasOneValid)
                    {
                        throw new MultipleValidTransitionsException(_stateMachine, validTransition, transition);
                    }
                    else
                    {
                        hasOneValid = true;
                        validTransition = transition;
                        sourceState = transition.StateFrom;
                        targetState = transition.StateTo;
                    }

                }
            }

            return hasOneValid;
        }

        private bool HasTrigger(Transition<TState, TTrigger> transition, TTrigger trigger)
        {
            return _triggerComparer.Equals(transition.Trigger, trigger);
        }
    }
}