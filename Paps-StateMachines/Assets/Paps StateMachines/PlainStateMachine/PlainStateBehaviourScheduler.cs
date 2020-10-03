using Paps.Maybe;
using System.Collections.Generic;
using System.Linq;

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

        public event StateChanged<TState, TTrigger> OnBeforeStateChanges;
        public event StateChanged<TState, TTrigger> OnStateChanged;

        private readonly IPlainStateMachine<TState, TTrigger> _stateMachine;
        private readonly PlainStateCollection<TState, TTrigger> _states;
        private readonly PlainTransitionCollection<TState, TTrigger> _transitions;
        private readonly PlainTransitionValidator<TState, TTrigger> _transitionValidator;
        private readonly IEqualityComparer<TState> _stateComparer;
        private readonly IEqualityComparer<TTrigger> _triggerComparer;

        private IState _currentStateObject;
        private TState _currentState { get; set; }

        public PlainStateBehaviourScheduler(IPlainStateMachine<TState, TTrigger> stateMachine,
            PlainStateCollection<TState, TTrigger> stateCollection, PlainTransitionCollection<TState, TTrigger> transitionCollection,
            PlainTransitionValidator<TState, TTrigger> transitionValidator,
            IEqualityComparer<TState> stateComparer, IEqualityComparer<TTrigger> triggerComparer)
        {
            _stateComparer = stateComparer;
            _triggerComparer = triggerComparer;
            _stateMachine = stateMachine;
            _states = stateCollection;
            _transitions = transitionCollection;
            _transitionValidator = transitionValidator;
        }

        public void Start()
        {
            ValidateCanStart();

            _currentState = _states.InitialState.Value;
            _currentStateObject = _states.GetStateObjectById(_states.InitialState.Value);

            IsRunning = true;

            _states.ProtectState(CurrentState.Value);

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
                _states.UnprotectState(CurrentState.Value);

                var lastStateObject = _currentStateObject;

                IsRunning = false;
                _currentState = default;
                _currentStateObject = default;

                lastStateObject.Exit();
            }
        }

        public bool CanSwitch() => IsRunning;

        public void SwitchTo(TState stateId, TTrigger trigger)
        {
            var previousState = CurrentState.Value;
            var nextState = stateId;
            var nextStateObj = _states.GetStateObjectById(stateId);

            _states.ProtectState(nextState);

            NotifyBeforeStateChangesEvent(previousState, trigger, nextState);

            _currentStateObject.Exit();

            _currentState = nextState;
            _currentStateObject = nextStateObj;

            _states.UnprotectState(previousState);

            NotifyStateChangedEvent(previousState, trigger, nextState);

            _currentStateObject.Enter();
        }

        public bool IsInState(TState stateId)
        {
            if (CurrentState.HasValue)
                return _stateComparer.Equals(CurrentState.Value, stateId);

            return false;
        }

        public bool Trigger(TTrigger trigger)
        {
            if (!CanSwitch())
                return false;

            _states.LockRemove();
            _transitions.Lock();
            _transitionValidator.Lock();

            if (TryGetStateTo(trigger, out TState stateTo))
            {
                _states.Unlock();
                _transitions.Unlock();
                _transitionValidator.Unlock();
                SwitchTo(stateTo, trigger);

                return true;
            }

            _transitionValidator.Unlock();
            _transitions.Unlock();
            _states.Unlock();

            return false;
        }

        private void NotifyBeforeStateChangesEvent(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            OnBeforeStateChanges?.Invoke(stateFrom, trigger, stateTo);
        }

        private void NotifyStateChangedEvent(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            OnStateChanged?.Invoke(stateFrom, trigger, stateTo);
        }

        private bool TryGetStateTo(TTrigger trigger, out TState stateTo)
        {
            stateTo = default;

            Transition<TState, TTrigger> validTransition = default;
            bool modifiedFlag = false;
            bool multipleValidGuardsFlag = false;

            foreach (var transition in _transitions)
            {
                if (_stateComparer.Equals(transition.StateFrom, CurrentState.Value)
                    && _triggerComparer.Equals(transition.Trigger, trigger)
                    && _transitionValidator.IsValid(transition))
                {
                    if (multipleValidGuardsFlag)
                    {
                        throw new MultipleValidTransitionsException(_stateMachine,
                            validTransition, transition);
                    }

                    validTransition = transition;
                    stateTo = transition.StateTo;

                    modifiedFlag = true;
                    multipleValidGuardsFlag = true;
                }
            }

            return modifiedFlag;
        }
    }
}