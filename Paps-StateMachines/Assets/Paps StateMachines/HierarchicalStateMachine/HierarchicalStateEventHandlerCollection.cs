using System;
using System.Collections.Generic;

namespace Paps.StateMachines
{
    internal class HierarchicalStateEventHandlerCollection<TState, TTrigger>
    {
        private readonly IHierarchicalStateMachine<TState, TTrigger> _stateMachine;
        private readonly IEqualityComparer<TState> _stateComparer;
        private readonly Dictionary<TState, List<IStateEventHandler>> _eventHandlers;
        private readonly List<TState> _lockedStates = new List<TState>();

        public HierarchicalStateEventHandlerCollection(IHierarchicalStateMachine<TState, TTrigger> stateMachine, IEqualityComparer<TState> stateComparer)
        {
            _stateComparer = stateComparer ?? throw new ArgumentNullException(nameof(stateComparer));
            _stateMachine = stateMachine;
            _eventHandlers = new Dictionary<TState, List<IStateEventHandler>>(_stateComparer);
        }

        public void AddEventHandlerTo(TState stateId, IStateEventHandler eventHandler)
        {
            ValidateIsNotNull(eventHandler);
            ValidateCanAddEventHandler(stateId, eventHandler);

            if (_eventHandlers.ContainsKey(stateId) == false)
                _eventHandlers.Add(stateId, new List<IStateEventHandler>());

            _eventHandlers[stateId].Add(eventHandler);
        }

        private void ValidateCanAddEventHandler(TState stateId, IStateEventHandler eventHandler)
        {
            if (IsLocked(stateId))
                throw new UnableToAddStateMachineElementException(_stateMachine, eventHandler);
        }

        private void ValidateIsNotNull(IStateEventHandler eventHandler)
        {
            if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));
        }

        public bool RemoveEventHandlerFrom(TState stateId, IStateEventHandler eventHandler)
        {
            ValidateCanRemoveEventHandler(stateId, eventHandler);

            if (_eventHandlers.ContainsKey(stateId))
            {
                bool removed = _eventHandlers[stateId].Remove(eventHandler);

                if (_eventHandlers[stateId].Count == 0)
                    _eventHandlers.Remove(stateId);

                return removed;
            }

            return false;
        }

        private void ValidateCanRemoveEventHandler(TState stateId, IStateEventHandler eventHandler)
        {
            if (IsLocked(stateId))
                throw new UnableToRemoveStateMachineElementException(_stateMachine, eventHandler);
        }

        public void RemoveEventHandlersFrom(TState stateId)
        {
            _eventHandlers.Remove(stateId);
        }

        public bool ContainsEventHandlerOn(TState stateId, IStateEventHandler eventHandler)
        {
            if (_eventHandlers.ContainsKey(stateId))
                return _eventHandlers[stateId].Contains(eventHandler);

            return false;
        }

        public IStateEventHandler[] GetEventHandlersOf(TState stateId)
        {
            if (_eventHandlers.ContainsKey(stateId))
                return _eventHandlers[stateId].ToArray();
            else
                return new IStateEventHandler[0];
        }

        public void LockEventHandlersOf(TState stateId)
        {
            if (!IsLocked(stateId))
                _lockedStates.Add(stateId);
        }

        public void UnlockEventHandlersOf(TState stateId)
        {
            _lockedStates.Remove(stateId);
        }

        private bool IsLocked(TState stateId)
        {
            return _lockedStates.Contains(stateId);
        }
    }

    /*public class HierarchicalStateMachine<TState, TTrigger> : IHierarchicalStateMachine<TState, TTrigger>, IStartableStateMachine<TState, TTrigger>, IUpdatableStateMachine<TState, TTrigger>
    {
        private enum StateMachineEvent
        {
            Start,
            Stop,
            Update,
            Trigger,
            AddState,
            RemoveState,
            AddTransition,
            RemoveTransition,
            AddGuardCondition,
            RemoveGuardCondition,
            AddChildState,
            RemoveChildState
        }

        public int StateCount => _stateHierarchy.StateCount;

        public int TransitionCount => _transitionHandler.TransitionCount;

        public bool IsRunning { get; private set; }

        public Maybe<TState> InitialState => _stateHierarchy.InitialState;

        public event HierarchyPathChanged<TTrigger> OnBeforeActiveHierarchyPathChanges;
        public event HierarchyPathChanged<TTrigger> OnActiveHierarchyPathChanged;

        private Comparer<TState> _stateComparer;
        private Comparer<TTrigger> _triggerComparer;

        private StateHierarchy<TState> _stateHierarchy;
        private StateHierarchyBehaviourScheduler<TState> _stateHierarchyBehaviourScheduler;
        private TransitionValidator<TState, TTrigger> _transitionValidator;
        private TransitionHandler<TState, TTrigger> _transitionHandler;
        private HierarchicalEventDispatcher<TState> _hierarchicalEventDispatcher;

        private Transition<TState, TTrigger> _currentValidatedTransition;
        private Queue<StateMachineEvent> _eventQueue = new Queue<StateMachineEvent>();

        public HierarchicalStateMachine(IEqualityComparer<TState> stateComparer, IEqualityComparer<TTrigger> triggerComparer)
        {
            if (stateComparer == null) throw new ArgumentNullException(nameof(stateComparer));
            if (triggerComparer == null) throw new ArgumentNullException(nameof(triggerComparer));

            _stateComparer = new Comparer<TState>();
            _triggerComparer = new Comparer<TTrigger>();

            SetStateComparer(stateComparer);
            SetTriggerComparer(triggerComparer);

            _stateHierarchy = new StateHierarchy<TState>(_stateComparer);
            _stateHierarchyBehaviourScheduler = new StateHierarchyBehaviourScheduler<TState>(_stateHierarchy, _stateComparer);
            _transitionValidator = new TransitionValidator<TState, TTrigger>(_stateComparer, _triggerComparer, _stateHierarchyBehaviourScheduler);
            _transitionHandler = new TransitionHandler<TState, TTrigger>(_stateComparer, _triggerComparer, _stateHierarchyBehaviourScheduler, _transitionValidator);
            _hierarchicalEventDispatcher = new HierarchicalEventDispatcher<TState>(_stateComparer, _stateHierarchyBehaviourScheduler);

            SubscribeToHierarchyPathChangeEvents();
            SubscribeToEventsForSavingValidTransition();
        }

        public HierarchicalStateMachine() : this(EqualityComparer<TState>.Default, EqualityComparer<TTrigger>.Default)
        {

        }

        private void SubscribeToEventsForSavingValidTransition()
        {
            _transitionHandler.OnTransitionValidated += transition => _currentValidatedTransition = _currentValidatedTransition = transition;
            _stateHierarchyBehaviourScheduler.OnActiveHierarchyPathChanged += () => _currentValidatedTransition = default;
        }

        private void CallOnBeforeActiveHierarchyPathChangesEvent()
        {
            OnBeforeActiveHierarchyPathChanges?.Invoke(_currentValidatedTransition.Trigger);
        }

        private void CallOnActiveHierarchyPathChangedEvent()
        {
            OnActiveHierarchyPathChanged?.Invoke(_currentValidatedTransition.Trigger);
        }

        private void SubscribeToHierarchyPathChangeEvents()
        {
            _stateHierarchyBehaviourScheduler.OnBeforeActiveHierarchyPathChanges += CallOnBeforeActiveHierarchyPathChangesEvent;
            _stateHierarchyBehaviourScheduler.OnActiveHierarchyPathChanged += CallOnActiveHierarchyPathChangedEvent;
        }

        public void SetStateComparer(IEqualityComparer<TState> stateComparer)
        {
            _stateComparer.EqualityComparer = stateComparer;
        }

        public void SetTriggerComparer(IEqualityComparer<TTrigger> triggerComparer)
        {
            _triggerComparer.EqualityComparer = triggerComparer;
        }

        public void AddGuardConditionTo(Transition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            ValidateContainsTransition(transition);

            _transitionValidator.AddGuardConditionTo(transition, guardCondition);
        }

        public void AddState(TState stateId, IState state)
        {
            _stateHierarchy.AddState(stateId, state);
        }

        public void AddTransition(Transition<TState, TTrigger> transition)
        {
            ValidateContainsId(transition.StateFrom);
            ValidateContainsId(transition.StateTo);

            _transitionHandler.AddTransition(transition);
        }

        public bool ContainsGuardConditionOn(Transition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            ValidateContainsTransition(transition);

            return _transitionValidator.ContainsGuardConditionOn(transition, guardCondition);
        }

        public bool ContainsState(TState stateId)
        {
            return _stateHierarchy.ContainsState(stateId);
        }

        public bool AreImmediateParentAndChild(TState superState, TState substate)
        {
            return _stateHierarchy.AreImmediateParentAndChild(superState, substate);
        }

        public bool ContainsTransition(Transition<TState, TTrigger> transition)
        {
            return _transitionHandler.ContainsTransition(transition);
        }

        public IEnumerable<TState> GetActiveHierarchyPath()
        {
            var activeHierarchyPath = _stateHierarchyBehaviourScheduler.GetActiveHierarchyPath();

            TState[] array = new TState[activeHierarchyPath.Count];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = activeHierarchyPath[i].Key;
            }

            return array;
        }

        public IGuardCondition[] GetGuardConditionsOf(Transition<TState, TTrigger> transition)
        {
            ValidateContainsTransition(transition);

            return _transitionValidator.GetGuardConditionsOf(transition);
        }

        public IState GetStateObjectById(TState stateId)
        {
            return _stateHierarchy.GetStateById(stateId);
        }

        public TState[] GetStates()
        {
            return _stateHierarchy.GetStates();
        }

        public Transition<TState, TTrigger>[] GetTransitions()
        {
            return _transitionHandler.GetTransitions();
        }

        public bool IsInState(TState stateId)
        {
            return _stateHierarchyBehaviourScheduler.IsInState(stateId);
        }

        public bool RemoveGuardConditionFrom(Transition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            ValidateContainsTransition(transition);

            return _transitionValidator.RemoveGuardConditionFrom(transition, guardCondition);
        }

        public bool RemoveState(TState stateId)
        {
            if(ContainsState(stateId))
            {
                ValidateIsNotInActiveHierarchy(stateId, "Cannot remove state because it is in the active hierarchy path");

                bool removed = _stateHierarchy.RemoveState(stateId);

                if(removed)
                {
                    RemoveTransitionsAndGuardConditionsRelatedTo(stateId);
                    _hierarchicalEventDispatcher.RemoveEventHandlersFrom(stateId);
                }

                return removed;
            }

            return false;
        }

        private void RemoveTransitionsAndGuardConditionsRelatedTo(TState stateId)
        {
            var removedTransitions = _transitionHandler.RemoveTransitionsRelatedTo(stateId);

            for (int i = 0; i < removedTransitions.Count; i++)
            {
                _transitionValidator.RemoveAllGuardConditionsFrom(removedTransitions[i]);
            }

            removedTransitions.Clear();
        }

        private bool IsInTheNextInitialActiveHierarchyPath(TState stateId)
        {
            return (AreEquals(_currentValidatedTransition.StateTo, stateId) ||
                            _stateHierarchy.AreParentAndInitialChildAtAnyLevel(_currentValidatedTransition.StateTo, stateId));
        }

        private void ValidateIsNotInActiveHierarchy(TState stateId, string message)
        {
            if (IsInState(stateId)) throw new InvalidOperationException(message);
        }

        public bool RemoveChildFromParent(TState childState)
        {
            ValidateIsNotInActiveHierarchy(childState, "Cannot remove child " + childState + 
                                                       " from its parent because it is in the active hierarchy path");

            return _stateHierarchy.RemoveChildFromParent(childState);
        }

        public bool RemoveTransition(Transition<TState, TTrigger> transition)
        {
            bool removed = _transitionHandler.RemoveTransition(transition);

            if (removed)
                _transitionValidator.RemoveAllGuardConditionsFrom(transition);

            return removed;
        }

        public void AddChildTo(TState parentState, TState childState)
        {
            ValidateChildIsNotActiveOnAddChild(childState);

            ValidateDoesNotWantToAddChildBeingActiveWithNoOthersChilds(parentState, childState);

            _stateHierarchy.AddChildTo(parentState, childState);
        }

        private void ValidateChildIsNotActiveOnAddChild(TState childState)
        {
            if(IsInState(childState)) throw new CannotAddChildException("Cannot set state " + childState +
                                                                        " because it is in the active hierarchy path");
        }

        private void ValidateDoesNotWantToAddChildBeingActiveWithNoOthersChilds(TState parentState, TState childState)
        {
            if (IsInState(parentState))
            {
                if (_stateHierarchy.ChildCountOf(parentState) == 0)
                {
                    throw new CannotAddChildException("Cannot set child " + childState +
                                                      " to parent " + parentState +
                                                      " because parent actually has no childs and is in the active hierarchy path." +
                                                      " If a state has childs, at least one must be active");
                }
            }
        }

        public void Start(Action callback = null)
        {
            ValidateIsNotStarted();
            ValidateIsNotEmpty();

            IsRunning = true;

            _stateHierarchyBehaviourScheduler.Enter();
        }

        private void ValidateIsNotEmpty()
        {
            //if (StateCount == 0) throw new EmptyStateMachineException();
        }

        public void Stop(Action callback = null)
        {
            if(IsRunning)
            {
                _stateHierarchyBehaviourScheduler.Exit();
            }

            IsRunning = false;
        }

        public void Trigger(TTrigger trigger, Action<bool> callback = null)
        {
            ValidateIsStarted();

            _transitionHandler.EnqueueTrigger(trigger);

            if(_stateHierarchyBehaviourScheduler.IsIdle == false)
            {
                _transitionHandler.ProcessEnqueuedTriggers();
            }
        }

        public void Update(Action callback = null)
        {
            ValidateIsStarted();

            _stateHierarchyBehaviourScheduler.Update();
        }

        public TState[] GetImmediateChildsOf(TState parent)
        {
            return _stateHierarchy.GetImmediateChildsOf(parent);
        }

        public TState GetParentOf(TState child)
        {
            return _stateHierarchy.GetParentOf(child);
        }

        public void SetInitialStateTo(TState parentState, TState initialState)
        {
            _stateHierarchy.SetInitialStateTo(parentState, initialState);
        }

        public TState GetInitialStateOf(TState parentState)
        {
            return _stateHierarchy.GetInitialStateOf(parentState);
        }

        public TState[] GetRoots()
        {
            return _stateHierarchy.GetRoots();
        }

        public void SendEvent(IEvent messageEvent, Action<bool> callback = null)
        {
            ValidateIsStarted();

            _hierarchicalEventDispatcher.SendEvent(messageEvent);
        }

        public void AddEventHandlerTo(TState stateId, IStateEventHandler eventHandler)
        {
            _hierarchicalEventDispatcher.AddEventHandlerTo(stateId, eventHandler);
        }

        public bool RemoveEventHandlerFrom(TState stateId, IStateEventHandler eventHandler)
        {
            return _hierarchicalEventDispatcher.RemoveEventHandlerFrom(stateId, eventHandler);
        }

        public bool ContainsEventHandlerOn(TState stateId, IStateEventHandler eventHandler)
        {
            return _hierarchicalEventDispatcher.HasEventHandlerOn(stateId, eventHandler);
        }

        public IStateEventHandler[] GetEventHandlersOf(TState stateId)
        {
            return _hierarchicalEventDispatcher.GetEventHandlersOf(stateId);
        }

        private void ValidateContainsId(TState stateId)
        {
            //if (ContainsState(stateId) == false) throw new StateIdNotAddedException(stateId.ToString());
        }

        private void ValidateContainsTransition(Transition<TState, TTrigger> transition)
        {
            /*if (ContainsTransition(transition) == false)
                throw new TransitionNotAddedException(transition.StateFrom.ToString(),
                    transition.Trigger.ToString(),
                    transition.StateTo.ToString());
        }

        private void ValidateIsStarted()
        {
            /*if(IsRunning == false)
                throw new StateMachineNotStartedException();
        }

        private void ValidateIsNotStarted()
        {
            /*if (IsRunning)
                throw new StateMachineRunningException();
        }

        private bool AreEquals(TState stateId1, TState stateId2)
        {
            return _stateComparer.Equals(stateId1, stateId2);
        }

        public void SetInitialState(TState stateId)
        {
            _stateHierarchy.SetInitialState(stateId);
        }

        private class Comparer<T> : IEqualityComparer<T>
        {
            public IEqualityComparer<T> EqualityComparer;

            public bool Equals(T x, T y)
            {
                return EqualityComparer.Equals(x, y);
            }

            public int GetHashCode(T obj)
            {
                return EqualityComparer.GetHashCode(obj);
            }
        }
    }*/
}
