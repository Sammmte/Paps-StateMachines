using Paps.Maybe;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Paps.StateMachines
{
    internal class PlainStateBehaviourScheduler<TState>
    {
        public Maybe<TState> CurrentState
        {
            get
            {
                if (IsStarted)
                    return _currentState.ToMaybe();
                else
                    return Maybe<TState>.Nothing;
            }
        }
        public bool IsStarted { get; private set; }

        private readonly PlainStateCollection<TState> _states;

        private IState _currentStateObject;
        private TState _currentState { get; set; }

        private IEqualityComparer<TState> _stateComparer;

        public PlainStateBehaviourScheduler(PlainStateCollection<TState> stateCollection, IEqualityComparer<TState> stateComparer)
        {
            _stateComparer = stateComparer;
            _states = stateCollection;
        }

        public void Start()
        {
            ValidateCanStart();

            _currentState = _states.InitialState.Value;
            _currentStateObject = _states.GetStateById(_states.InitialState.Value);

            IsStarted = true;

            _currentStateObject.Enter();
        }

        private void ValidateCanStart()
        {
            ValidateIsNotStarted();
            ValidateIsNotEmpty();
            ValidateInitialState();
        }

        private void ValidateIsNotStarted()
        {
            if (IsStarted)
                throw new StateMachineStartedException();
        }

        private void ValidateIsNotEmpty()
        {
            if (_states.StateCount == 0)
                throw new EmptyStateMachineException("State machine has no states");
        }

        private void ValidateInitialState()
        {
            if (_states.InitialState.HasValue == false)
                throw new InvalidInitialStateException("Initial state is not set");
        }

        public void Update()
        {
            if(IsStarted)
            {
                _currentStateObject.Update();
            }
        }

        public void Stop()
        {
            if(IsStarted)
            {
                var lastStateObject = _currentStateObject;

                IsStarted = false;
                _currentState = default;
                _currentStateObject = default;

                lastStateObject.Exit();
            }
        }

        public bool CanSwitch() => IsStarted;

        public void SwitchTo(TState stateId, Action onStateChanged)
        {
            var nextState = stateId;
            var nextStateObj = _states.GetStateById(stateId);

            _currentStateObject.Exit();

            _currentState = nextState;
            _currentStateObject = nextStateObj;

            onStateChanged();

            _currentStateObject.Enter();
        }

        public bool IsInState(TState stateId)
        {
            if (CurrentState.HasValue)
                return _stateComparer.Equals(CurrentState.Value, stateId);

            return false;
        }
    }
}