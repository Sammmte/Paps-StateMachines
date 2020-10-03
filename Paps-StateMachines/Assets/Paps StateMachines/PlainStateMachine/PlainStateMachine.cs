using System;
using System.Collections.Generic;
using Paps.Maybe;

namespace Paps.StateMachines
{
    public class PlainStateMachine<TState, TTrigger> : IPlainStateMachine<TState, TTrigger>
    {
        public event StateChanged<TState, TTrigger> OnBeforeStateChanges
        {
            add
            {
                _stateBehaviourScheduler.OnBeforeStateChanges += value;
            }
            remove
            {
                _stateBehaviourScheduler.OnBeforeStateChanges -= value;
            }
        }

        public event StateChanged<TState, TTrigger> OnStateChanged
        {
            add
            {
                _stateBehaviourScheduler.OnStateChanged += value;
            }
            remove
            {
                _stateBehaviourScheduler.OnStateChanged -= value;
            }
        }

        public Maybe<TState> CurrentState => _stateBehaviourScheduler.CurrentState;

        public bool IsRunning => _stateBehaviourScheduler.IsRunning;

        public int StateCount => _states.StateCount;

        public int TransitionCount => _transitions.TransitionCount;

        public Maybe<TState> InitialState => _states.InitialState;

        private PlainStateCollection<TState, TTrigger> _states;
        private PlainTransitionCollection<TState, TTrigger> _transitions;
        private PlainStateBehaviourScheduler<TState, TTrigger> _stateBehaviourScheduler;
        private PlainTransitionValidator<TState, TTrigger> _transitionValidator;
        private PlainStateEventHandlerCollection<TState, TTrigger> _eventHandlers;
        private PlainEventDispatcher<TState, TTrigger> _eventDispatcher;

        private StateEqualityComparer _stateComparer;
        private TriggerEqualityComparer _triggerComparer;
        private IEqualityComparer<Transition<TState, TTrigger>> _transitionComparer;

        private Queue<Action> _actionQueue = new Queue<Action>();
        private bool _processingActions;

        public PlainStateMachine(IEqualityComparer<TState> stateComparer, IEqualityComparer<TTrigger> triggerComparer)
        {
            if (stateComparer == null) throw new ArgumentNullException(nameof(stateComparer));
            if (triggerComparer == null) throw new ArgumentNullException(nameof(triggerComparer));

            _stateComparer = new StateEqualityComparer(stateComparer);
            _triggerComparer = new TriggerEqualityComparer(triggerComparer);

            _transitionComparer = new TransitionEqualityComparer<TState, TTrigger>(_stateComparer, _triggerComparer);

            _states = new PlainStateCollection<TState, TTrigger>(this, _stateComparer);
            _transitionValidator = new PlainTransitionValidator<TState, TTrigger>(this, _transitionComparer);
            _transitions = new PlainTransitionCollection<TState, TTrigger>(this, _stateComparer, _transitionComparer);
            _stateBehaviourScheduler = new PlainStateBehaviourScheduler<TState, TTrigger>(this, _states, _transitions, _transitionValidator, _stateComparer, _triggerComparer);
            _eventHandlers = new PlainStateEventHandlerCollection<TState, TTrigger>(this, _stateComparer);
            _eventDispatcher = new PlainEventDispatcher<TState, TTrigger>(_eventHandlers, _stateBehaviourScheduler);
        }

        public PlainStateMachine() : this(EqualityComparer<TState>.Default, EqualityComparer<TTrigger>.Default)
        {
            
        }

        public PlainStateMachine(TState initialStateId, IState stateObject, IEqualityComparer<TState> stateComparer, 
            IEqualityComparer<TTrigger> triggerComparer) : this(stateComparer, triggerComparer)
        {
            AddState(initialStateId, stateObject);
        }

        public PlainStateMachine(TState initialStateId, IState stateObject) : this(initialStateId, stateObject,
            EqualityComparer<TState>.Default, EqualityComparer<TTrigger>.Default)
        {

        }

        public void SetStateComparer(IEqualityComparer<TState> stateComparer)
        {
            if (stateComparer == null) throw new ArgumentNullException(nameof(stateComparer));

            _stateComparer.SetEqualityComparer(stateComparer);
        }

        public void SetTriggerComparer(IEqualityComparer<TTrigger> triggerComparer)
        {
            if (triggerComparer == null) throw new ArgumentNullException(nameof(triggerComparer));

            _triggerComparer.SetEqualityComparer(triggerComparer);
        }

        private void ProcessActions()
        {
            if (_processingActions)
                return;

            _processingActions = true;

            while (_actionQueue.Count > 0)
                _actionQueue.Dequeue().Invoke();

            _processingActions = false;
        }

        public void Start(Action callback = null)
        {
            _actionQueue.Enqueue(() =>
            {
                _stateBehaviourScheduler.Start();
                
                callback?.Invoke();
            });

            ProcessActions();
        }

        public void Stop(Action callback = null)
        {
            _actionQueue.Enqueue(() =>
            {
                _stateBehaviourScheduler.Stop();

                callback?.Invoke();
            });

            ProcessActions();
        }

        public void Update(Action callback = null)
        {
            _actionQueue.Enqueue(() =>
            {
                _stateBehaviourScheduler.Update();
                callback?.Invoke();
            });

            ProcessActions();
        }

        public void AddEventHandlerTo(TState stateId, IStateEventHandler eventHandler)
        {
            ValidateContainsState(stateId);

            _eventHandlers.AddEventHandlerTo(stateId, eventHandler);
        }

        public bool RemoveEventHandlerFrom(TState stateId, IStateEventHandler eventHandler)
        {
            return _eventHandlers.RemoveEventHandlerFrom(stateId, eventHandler);
        }

        public bool ContainsEventHandlerOn(TState stateId, IStateEventHandler eventHandler)
        {
            return _eventHandlers.ContainsEventHandlerOn(stateId, eventHandler);
        }

        public IStateEventHandler[] GetEventHandlersOf(TState stateId)
        {
            ValidateContainsState(stateId);

            return _eventHandlers.GetEventHandlersOf(stateId);
        }

        public void SendEvent(IEvent ev, Action<bool> callback = null)
        {
            _actionQueue.Enqueue(() =>
            {
                var handled = _eventDispatcher.SendEvent(ev);

                callback?.Invoke(handled);
            });

            ProcessActions();
        }

        public void AddGuardConditionTo(Transition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            ValidateContainsTransition(transition);

            _transitionValidator.AddGuardConditionTo(transition, guardCondition);
        }

        public bool RemoveGuardConditionFrom(Transition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
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

        public void SetInitialState(TState stateId)
        {
            _states.SetInitialState(stateId);
        }

        public void AddState(TState stateId, IState state)
        {
            ValidateCanAddState(stateId, state);

            _states.AddState(stateId, state);
        }

        public bool RemoveState(TState stateId)
        {
            if (_states.RemoveState(stateId))
            {
                RemoveObjectsRelatedTo(stateId);

                return true;
            }

            return false;
        }

        private void RemoveObjectsRelatedTo(TState stateId)
        {
            var removedTransitions = _transitions.RemoveTransitionsRelatedTo(stateId);
            _transitionValidator.RemoveAllGuardConditionsFrom(removedTransitions);
            _eventHandlers.RemoveEventHandlersFrom(stateId);
        }

        public bool ContainsState(TState stateId)
        {
            if (stateId == null)
                throw new ArgumentNullException(nameof(stateId));

            return _states.ContainsState(stateId);
        }

        public TState[] GetStates()
        {
            return _states.GetStates();
        }

        public void AddTransition(Transition<TState, TTrigger> transition)
        {
            ValidateContainsState(transition.StateFrom);
            ValidateContainsState(transition.StateTo);

            _transitions.AddTransition(transition);
        }

        public bool RemoveTransition(Transition<TState, TTrigger> transition)
        {
            if(_transitions.RemoveTransition(transition))
            {
                _transitionValidator.RemoveAllGuardConditionsFrom(transition);

                return true;
            }

            return false;
        }

        public bool ContainsTransition(Transition<TState, TTrigger> transition)
        {
            return _transitions.ContainsTransition(transition);
        }

        public Transition<TState, TTrigger>[] GetTransitions()
        {
            return _transitions.GetTransitions();
        }

        public IState GetStateObjectById(TState stateId)
        {
            return _states.GetStateObjectById(stateId);
        }

        public void Trigger(TTrigger trigger, Action<bool> callback = null)
        {
            _actionQueue.Enqueue(() =>
            {
                var hasChanged = _stateBehaviourScheduler.Trigger(trigger);
                callback?.Invoke(hasChanged);
            });

            ProcessActions();
        }

        public bool IsInState(TState stateId)
        {
            return _stateBehaviourScheduler.IsInState(stateId);
        }

        private void ValidateContainsState(TState stateId)
        {
            if (!ContainsState(stateId))
                throw new StateIdNotAddedException(this, stateId);
        }

        private void ValidateContainsTransition(Transition<TState, TTrigger> transition)
        {
            if (!ContainsTransition(transition))
                throw new TransitionNotAddedException(this, transition.StateFrom, transition.Trigger, transition.StateTo);
        }

        private void ValidateCanAddState(TState stateId, IState state)
        {
            if (stateId == null)
            {
                throw new ArgumentNullException(nameof(stateId));
            }
            else if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
        }

        private class StateEqualityComparer : IEqualityComparer<TState>
        {
            private IEqualityComparer<TState> _equalityComparer;

            public StateEqualityComparer(IEqualityComparer<TState> equalityComparer)
            {
                SetEqualityComparer(equalityComparer);
            }

            public bool Equals(TState x, TState y)
            {
                return _equalityComparer.Equals(x, y);
            }

            public int GetHashCode(TState obj)
            {
                return _equalityComparer.GetHashCode(obj);
            }

            public void SetEqualityComparer(IEqualityComparer<TState> equalityComparer)
            {
                _equalityComparer = equalityComparer ?? EqualityComparer<TState>.Default;
            }
        }

        private class TriggerEqualityComparer : IEqualityComparer<TTrigger>
        {
            private IEqualityComparer<TTrigger> _equalityComparer;

            public TriggerEqualityComparer(IEqualityComparer<TTrigger> equalityComparer)
            {
                SetEqualityComparer(equalityComparer);
            }

            public bool Equals(TTrigger x, TTrigger y)
            {
                return _equalityComparer.Equals(x, y);
            }

            public int GetHashCode(TTrigger obj)
            {
                return _equalityComparer.GetHashCode(obj);
            }

            public void SetEqualityComparer(IEqualityComparer<TTrigger> equalityComparer)
            {
                _equalityComparer = equalityComparer ?? EqualityComparer<TTrigger>.Default;
            }
        }
    }
}