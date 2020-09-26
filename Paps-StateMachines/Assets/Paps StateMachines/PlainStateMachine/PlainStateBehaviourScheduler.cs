using Paps.Maybe;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace Paps.StateMachines
{
    internal class PlainStateBehaviourScheduler<TState, TTrigger>
    {
        public Maybe<TState> CurrentState
        {
            get
            {
                if (IsRunning)
                    return _currentState.ToMaybe();
                else
                    return Maybe<TState>.Nothing;
            }
        }
        public bool IsRunning { get; private set; }

        private readonly IPlainStateMachine<TState, TTrigger> _stateMachine;
        private readonly PlainStateCollection<TState, TTrigger> _states;

        private IState _currentStateObject;
        private TState _currentState { get; set; }

        private IEqualityComparer<TState> _stateComparer;

        public PlainStateBehaviourScheduler(IPlainStateMachine<TState, TTrigger> stateMachine, PlainStateCollection<TState, TTrigger> stateCollection, IEqualityComparer<TState> stateComparer)
        {
            _stateComparer = stateComparer;
            _stateMachine = stateMachine;
            _states = stateCollection;
        }

        public void Start()
        {
            ValidateCanStart();

            _currentState = _states.InitialState.Value;
            _currentStateObject = _states.GetStateObjectById(_states.InitialState.Value);

            IsRunning = true;

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
            if (IsRunning)
                throw new StateMachineRunningException(_stateMachine);
        }

        private void ValidateIsNotEmpty()
        {
            if (_states.StateCount == 0)
                throw new EmptyStateMachineException(_stateMachine);
        }

        private void ValidateInitialState()
        {
            if (_states.InitialState.HasValue == false)
                throw new InvalidInitialStateException(_stateMachine);
        }

        public void Update()
        {
            if(IsRunning)
            {
                _currentStateObject.Update();
            }
        }

        public void Stop()
        {
            if(IsRunning)
            {
                var lastStateObject = _currentStateObject;

                IsRunning = false;
                _currentState = default;
                _currentStateObject = default;

                lastStateObject.Exit();
            }
        }

        public bool CanSwitch() => IsRunning;

        public void SwitchTo(TState stateId, Action onStateChanged)
        {
            var nextState = stateId;
            var nextStateObj = _states.GetStateObjectById(stateId);

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