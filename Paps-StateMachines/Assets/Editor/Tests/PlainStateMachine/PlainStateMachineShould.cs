using NSubstitute;
using NUnit.Framework;
using Paps.Maybe;
using Paps.StateMachines;
using System;

namespace Tests.PlainStateMachine
{
    public abstract class PlainStateMachineShould<TState, TTrigger>
    {
        protected class TestState : IState
        {
            public void Enter()
            {

            }

            public void Exit()
            {

            }

            public void Update()
            {

            }
        }

        protected abstract PlainStateMachine<TState, TTrigger> NewStateMachine();
        protected abstract PlainStateMachine<T, U> NewStateMachine<T, U>();

        protected abstract TState NewStateId();

        protected abstract TTrigger NewTrigger();

        protected abstract Transition<TState, TTrigger> NewTransition();

        protected abstract Transition<TState, TTrigger> NewTransition(TState sourceTarget, TTrigger trigger, TState targetState);
        protected abstract Transition<T, U> NewTransition<T, U>(T sourceTarget, U trigger, T targetState);

        protected IPlainStateMachine<TState, TTrigger> _stateMachine;
        protected TState _stateId1, _stateId2, _stateId3, _stateId4, _stateId5;
        protected TTrigger _trigger1, _trigger2, _trigger3, _trigger4, _trigger5;
        protected IState _stateObject1, _stateObject2, _stateObject3, _stateObject4, _stateObject5;
        protected IGuardCondition _guardCondition1, _guardCondition2, _guardCondition3, _guardCondition4, _guardCondition5;
        protected IStateEventHandler _stateEventHandler1, _stateEventHandler2, _stateEventHandler3, _stateEventHandler4, _stateEventHandler5;
        protected IEvent _event1;
        protected StateChanged<TState, TTrigger> _onBeforeStateChangesSubscriptor1, _onBeforeStateChangesSubscriptor2,
            _onBeforeStateChangesSubscriptor3;
        protected StateChanged<TState, TTrigger> _onStateChangedSubscriptor1, _onStateChangedSubscriptor2, _onStateChangedSubscriptor3;
        protected Action _startCallback;
        protected Action _stopCallback;
        protected Action _updateCallback;
        protected Action<bool> _triggerCallback;
        protected Action<bool> _sendEventCallback;

        [SetUp]
        public void SetUp()
        {
            _stateMachine = NewStateMachine();

            _stateId1 = NewStateId();
            _stateId2 = NewStateId();
            _stateId3 = NewStateId();
            _stateId4 = NewStateId();
            _stateId5 = NewStateId();

            _trigger1 = NewTrigger();
            _trigger2 = NewTrigger();
            _trigger3 = NewTrigger();
            _trigger4 = NewTrigger();
            _trigger5 = NewTrigger();

            _stateObject1 = Substitute.For<IState>();
            _stateObject2 = Substitute.For<IState>();
            _stateObject3 = Substitute.For<IState>();
            _stateObject4 = Substitute.For<IState>();
            _stateObject5 = Substitute.For<IState>();

            _guardCondition1 = Substitute.For<IGuardCondition>();
            _guardCondition2 = Substitute.For<IGuardCondition>();
            _guardCondition3 = Substitute.For<IGuardCondition>();
            _guardCondition4 = Substitute.For<IGuardCondition>();
            _guardCondition5 = Substitute.For<IGuardCondition>();

            _stateEventHandler1 = Substitute.For<IStateEventHandler>();
            _stateEventHandler2 = Substitute.For<IStateEventHandler>();
            _stateEventHandler3 = Substitute.For<IStateEventHandler>();
            _stateEventHandler4 = Substitute.For<IStateEventHandler>();
            _stateEventHandler5 = Substitute.For<IStateEventHandler>();

            _event1 = Substitute.For<IEvent>();

            _onBeforeStateChangesSubscriptor1 = Substitute.For<StateChanged<TState, TTrigger>>();
            _onBeforeStateChangesSubscriptor2 = Substitute.For<StateChanged<TState, TTrigger>>();
            _onBeforeStateChangesSubscriptor3 = Substitute.For<StateChanged<TState, TTrigger>>();

            _onStateChangedSubscriptor1 = Substitute.For<StateChanged<TState, TTrigger>>();
            _onStateChangedSubscriptor2 = Substitute.For<StateChanged<TState, TTrigger>>();
            _onStateChangedSubscriptor3 = Substitute.For<StateChanged<TState, TTrigger>>();

            _startCallback = Substitute.For<Action>();
            _stopCallback = Substitute.For<Action>();
            _updateCallback = Substitute.For<Action>();
            _triggerCallback = Substitute.For<Action<bool>>();
            _sendEventCallback = Substitute.For<Action<bool>>();
        }

        [Test]
        public void Add_States()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            Assert.That(_stateMachine.ContainsState(_stateId1), "Contains state 1");
        }

        [Test]
        public void Remove_States()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateMachine.RemoveState(_stateId1);

            Assert.That(_stateMachine.ContainsState(_stateId1) == false, "State 1 was removed");
        }

        [Test]
        public void Throw_An_Exception_When_User_Adds_An_Existing_State()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            Assert.Throws<StateIdAlreadyAddedException>(() => _stateMachine.AddState(_stateId1, _stateObject1));
        }

        [Test]
        public void Throw_An_Exception_When_User_Adds_A_Null_State_Object()
        {
            Assert.Throws<ArgumentNullException>(() => _stateMachine.AddState(_stateId1, null));
        }

        [Test]
        public void Return_State_Count()
        {
            Assert.That(_stateMachine.StateCount == 0, "State count starts at zero");

            _stateMachine.AddState(_stateId1, _stateObject1);

            Assert.That(_stateMachine.StateCount == 1, "State count has been increased");

            _stateMachine.RemoveState(_stateId1);

            Assert.That(_stateMachine.StateCount == 0, "State count has been decreased");
        }

        [Test]
        public void Return_State_Object_By_Id()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            Assert.AreEqual(_stateObject1, _stateMachine.GetStateObjectById(_stateId1));
            Assert.AreEqual(_stateObject2, _stateMachine.GetStateObjectById(_stateId2));
        }

        [Test]
        public void Return_States()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var states = _stateMachine.GetStates();

            Assert.Contains(_stateId1, states);
            Assert.Contains(_stateId2, states);
            Assert.That(states.Length == 2, "Contains only 2 states");
        }

        [Test]
        public void Add_Transitions()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            Assert.That(_stateMachine.ContainsTransition(transition), "Contains transition");
        }

        [Test]
        public void Remove_Transitions()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _stateMachine.RemoveTransition(transition);

            Assert.That(_stateMachine.ContainsTransition(transition) == false, "Transition was removed");
        }

        [Test]
        public void Return_Transition_Count()
        {
            Assert.That(_stateMachine.TransitionCount == 0, "Transition count starts at zero");

            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            Assert.That(_stateMachine.TransitionCount == 1, "Transition count has been increased");

            _stateMachine.RemoveTransition(transition);

            Assert.That(_stateMachine.TransitionCount == 0, "Transition count has been decreased");
        }

        [Test]
        public void Return_Transitions()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition1 = NewTransition(_stateId1, _trigger1, _stateId2);
            var transition2 = NewTransition(_stateId2, _trigger1, _stateId1);

            _stateMachine.AddTransition(transition1);
            _stateMachine.AddTransition(transition2);

            var transitions = _stateMachine.GetTransitions();

            Assert.Contains(transition1, transitions);
            Assert.Contains(transition2, transitions);
            Assert.That(transitions.Length == 2, "Contains only 2 transitions");
        }

        [Test]
        public void Throw_An_Exception_When_User_Adds_A_Transition_With_A_Not_Added_State()
        {
            Assert.Throws<StateIdNotAddedException>(() => _stateMachine.AddTransition(NewTransition(_stateId1, _trigger1, _stateId1)));
        }

        [Test]
        public void Add_Guard_Conditions()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            var transition = NewTransition(_stateId1, _trigger1, _stateId1);

            _stateMachine.AddTransition(transition);

            _stateMachine.AddGuardConditionTo(transition, _guardCondition1);

            Assert.That(_stateMachine.ContainsGuardConditionOn(transition, _guardCondition1), "Guard condition was added");
        }

        [Test]
        public void Remove_Guard_Conditions()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            var transition = NewTransition(_stateId1, _trigger1, _stateId1);

            _stateMachine.AddTransition(transition);

            _stateMachine.AddGuardConditionTo(transition, _guardCondition1);

            _stateMachine.RemoveGuardConditionFrom(transition, _guardCondition1);

            Assert.That(_stateMachine.ContainsGuardConditionOn(transition, _guardCondition1) == false, "Guard condition was removed");
        }

        [Test]
        public void Return_Guard_Conditions_Of_A_Specified_Transition()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            var transition = NewTransition(_stateId1, _trigger1, _stateId1);

            _stateMachine.AddTransition(transition);

            _stateMachine.AddGuardConditionTo(transition, _guardCondition1);
            _stateMachine.AddGuardConditionTo(transition, _guardCondition2);

            var guardConditions = _stateMachine.GetGuardConditionsOf(transition);

            Assert.Contains(_guardCondition1, guardConditions);
            Assert.Contains(_guardCondition2, guardConditions);
            Assert.That(guardConditions.Length == 2, "Only contains 2 guard conditions");
        }

        [Test]
        public void Remove_Guard_Conditions_Related_To_A_Transition_When_It_Is_Removed()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            var transition = NewTransition(_stateId1, _trigger1, _stateId1);

            _stateMachine.AddTransition(transition);

            _stateMachine.AddGuardConditionTo(transition, _guardCondition1);

            _stateMachine.RemoveTransition(transition);

            Assert.That(_stateMachine.ContainsGuardConditionOn(transition, _guardCondition1) == false, "Guard condition was removed");

            _stateMachine.AddTransition(transition);

            Assert.That(_stateMachine.ContainsGuardConditionOn(transition, _guardCondition1) == false, "Guard condition was removed");
        }

        [Test]
        public void Throw_An_Exception_When_Asked_To_Return_Guard_Conditions_Of_A_Not_Added_Transition()
        {
            var transition = NewTransition(_stateId1, _trigger1, _stateId1);

            Assert.Throws<TransitionNotAddedException>(() => _stateMachine.GetGuardConditionsOf(transition));
        }

        [Test]
        public void Throw_An_Exception_When_User_Tries_To_Add_A_Guard_Condition_To_A_Not_Added_Transition()
        {
            var transition = NewTransition(_stateId1, _trigger1, _stateId1);

            Assert.Throws<TransitionNotAddedException>(() => _stateMachine.AddGuardConditionTo(transition, _guardCondition1));
        }

        [Test]
        public void Add_State_Event_Handlers()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler1);

            Assert.That(_stateMachine.ContainsEventHandlerOn(_stateId1, _stateEventHandler1), "Event handler was added");
        }

        [Test]
        public void Remove_State_Event_Handlers()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler1);

            _stateMachine.RemoveEventHandlerFrom(_stateId1, _stateEventHandler1);

            Assert.That(_stateMachine.ContainsEventHandlerOn(_stateId1, _stateEventHandler1) == false, "Event handler was removed");
        }

        [Test]
        public void Return_State_Event_Handlers_Of_A_Specified_State()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler1);
            _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler2);

            var stateEventHandlers = _stateMachine.GetEventHandlersOf(_stateId1);

            Assert.Contains(_stateEventHandler1, stateEventHandlers);
            Assert.Contains(_stateEventHandler2, stateEventHandlers);
            Assert.That(stateEventHandlers.Length == 2, "Only contains 2 event handlers");
        }

        [Test]
        public void Remove_Transitions_Guard_Conditions_And_Event_Handlers_When_Their_Related_State_Is_Removed()
        {
            var transition = NewTransition(_stateId1, _trigger1, _stateId1);

            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddTransition(transition);
            _stateMachine.AddGuardConditionTo(transition, _guardCondition1);
            _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler1);

            _stateMachine.RemoveState(_stateId1);

            Assert.That(_stateMachine.ContainsTransition(transition) == false, "Transition was removed");
            Assert.That(_stateMachine.ContainsGuardConditionOn(transition, _guardCondition1) == false, "Guard condition was removed");
            Assert.That(_stateMachine.ContainsEventHandlerOn(_stateId1, _stateEventHandler1) == false, "Event handler was removed");
        }

        [Test]
        public void Throw_An_Exception_When_Asked_To_Return_Event_Handlers_Of_A_Not_Added_State()
        {
            Assert.Throws<StateIdNotAddedException>(() => _stateMachine.GetEventHandlersOf(_stateId1));
        }

        [Test]
        public void Throw_An_Exception_When_User_Tries_To_Add_An_Event_Handler_To_Not_Added_State()
        {
            Assert.Throws<StateIdNotAddedException>(() => _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler1));
        }

        [Test]
        public void Let_Set_Initial_State()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            _stateMachine.SetInitialState(_stateId2);

            Assert.AreEqual(_stateMachine.InitialState.Value, _stateId2);
        }

        [Test]
        public void Set_Initial_State_Automatically_When_First_State_Is_Added()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            Assert.AreEqual(_stateMachine.InitialState.Value, _stateId1);
        }

        [Test]
        public void Leave_Initial_State_With_Nothing_When_That_State_Is_Removed()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            _stateMachine.RemoveState(_stateId1);

            Assert.That(_stateMachine.InitialState.IsNothing(), "Initial state is nothing");
        }

        [Test]
        public void Throw_An_Exception_When_A_Not_Added_State_Is_Set_As_Initial_State()
        {
            Assert.Throws<StateIdNotAddedException>(() => _stateMachine.SetInitialState(_stateId1));
        }

        [Test]
        public void Start_From_Initial_State()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateMachine.Start();

            Assert.That(_stateMachine.CurrentState.Value.Equals(_stateId1), "Started from initial state");
        }

        [Test]
        public void Return_Nothing_From_Current_State_When_It_Is_Stopped()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            Assert.That(_stateMachine.CurrentState.HasValue == false, "Current state is nothing before start");

            _stateMachine.Start();

            _stateMachine.Stop();

            Assert.That(_stateMachine.CurrentState.HasValue == false, "Current state is nothing after stop");
        }

        [Test]
        public void Return_That_Is_Running_When_It_Is()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            Assert.That(_stateMachine.IsRunning == false, "Returns false before start");

            _stateMachine.Start();

            Assert.That(_stateMachine.IsRunning, "Returns true after start");

            _stateMachine.Stop();

            Assert.That(_stateMachine.IsRunning == false, "Returns false after stop");
        }

        [Test]
        public void Throw_An_Exception_When_Is_Started_With_No_Added_State()
        {
            Assert.Throws<EmptyStateMachineException>(() => _stateMachine.Start());
        }

        [Test]
        public void Throw_An_Exception_When_Is_Started_Being_Already_Started()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.Start();

            Assert.Throws<StateMachineRunningException>(() => _stateMachine.Start());
        }

        [Test]
        public void Throw_An_Exception_When_Is_Started_Without_Initial_State()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            _stateMachine.RemoveState(_stateId1);

            Assert.Throws<InvalidInitialStateException>(() => _stateMachine.Start());
        }

        [Test]
        public void Throw_An_Exception_When_Asked_To_Remove_Current_State()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateMachine.Start();

            Assert.Throws<ProtectedStateException>(() => _stateMachine.RemoveState(_stateId1));
        }

        [Test]
        public void Return_If_Is_In_A_Specified_State()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.Start();

            Assert.That(_stateMachine.IsInState(_stateId1), "Returns true when asked if is in current state");
        }

        [Test]
        public void Execute_Enter_Of_Initial_State_When_Started()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.Start();

            _stateObject1.Received(1).Enter();
        }

        [Test]
        public void Execute_Update_Of_Current_State_When_Updated()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.Start();

            _stateMachine.Update();

            _stateObject1.Received(1).Update();
        }

        [Test]
        public void Do_Nothing_When_Update_Is_Called_While_Not_Running()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            Assert.DoesNotThrow(() => _stateMachine.Update());

            _stateObject1.DidNotReceive().Update();
        }

        [Test]
        public void Execute_Exit_Of_Current_State_When_Stopped()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.Start();

            _stateMachine.Stop();

            _stateObject1.Received(1).Exit();
        }

        [Test]
        public void Do_Nothing_When_Stop_Is_Called_While_Not_Running()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            Assert.DoesNotThrow(() => _stateMachine.Stop());

            _stateObject1.DidNotReceive().Exit();
        }

        [Test]
        public void Change_Current_State_When_A_Valid_Transition_Is_Triggered_With_No_Guard_Conditions()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _stateMachine.Start();

            _stateMachine.Trigger(_trigger1);

            Assert.That(_stateMachine.CurrentState.Value.Equals(_stateId2), "Current state has changed");
        }

        [Test]
        public void Execute_Previous_State_Exit_And_Then_New_Current_State_Enter_When_A_Transition_Has_Been_Triggered()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _stateMachine.Start();

            _stateMachine.Trigger(_trigger1);

            Received.InOrder(() =>
            {
                _stateObject1.Exit();
                _stateObject2.Enter();
            });
        }

        [Test]
        public void Call_On_Before_State_Changes_Subscriptors_Before_Previous_State_Exit_When_A_Transition_Has_Been_Triggered()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _stateMachine.OnBeforeStateChanges += _onBeforeStateChangesSubscriptor1;

            _stateMachine.Start();

            _stateMachine.Trigger(_trigger1);

            Received.InOrder(() =>
            {
                _onBeforeStateChangesSubscriptor1.Invoke(_stateId1, _trigger1, _stateId2);
                _stateObject1.Exit();
            });
        }

        [Test]
        public void Call_On_State_Changed_Subscriptors_After_Previous_State_Exit_And_Before_Current_State_Enter_When_A_Transition_Has_Been_Triggered()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _stateMachine.OnStateChanged += _onStateChangedSubscriptor1;

            _stateMachine.Start();

            _stateMachine.Trigger(_trigger1);

            Received.InOrder(() =>
            {
                _stateObject1.Exit();
                _onStateChangedSubscriptor1.Invoke(_stateId1, _trigger1, _stateId2);
                _stateObject2.Enter();
            });
        }

        [Test]
        public void Do_Nothing_When_Trigger_Is_Called_While_Not_Running()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            var transition = NewTransition(_stateId1, _trigger1, _stateId1);

            _stateMachine.AddTransition(transition);

            Assert.DoesNotThrow(() => _stateMachine.Trigger(_trigger1));
            _stateObject1.DidNotReceive().Enter();
            _stateObject1.DidNotReceive().Exit();
        }

        [Test]
        public void Do_Not_Change_State_When_A_Transition_Is_Triggered_But_A_Guard_Condition_Is_Invalid()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _stateMachine.AddGuardConditionTo(transition, _guardCondition1);
            _stateMachine.AddGuardConditionTo(transition, _guardCondition2);

            _guardCondition1.IsValid().Returns(true);
            _guardCondition2.IsValid().Returns(false);

            _stateMachine.Start();

            _stateMachine.Trigger(_trigger1);

            Assert.That(_stateMachine.CurrentState.Value.Equals(_stateId1), "State has not changed");
        }

        [Test]
        public void Change_State_When_A_Transition_Is_Triggered_And_All_Guard_Conditions_Are_Valid()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _stateMachine.AddGuardConditionTo(transition, _guardCondition1);
            _stateMachine.AddGuardConditionTo(transition, _guardCondition2);

            _guardCondition1.IsValid().Returns(true);
            _guardCondition2.IsValid().Returns(true);

            _stateMachine.Start();

            _stateMachine.Trigger(_trigger1);

            Assert.That(_stateMachine.CurrentState.Value.Equals(_stateId2), "State has changed");
        }

        [Test]
        public void Not_Throw_When_Multiple_Transitions_Are_Triggered_But_Only_One_Is_Valid()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition1 = NewTransition(_stateId1, _trigger1, _stateId1);
            var transition2 = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition1);
            _stateMachine.AddTransition(transition2);

            _guardCondition1.IsValid().Returns(false);

            _stateMachine.AddGuardConditionTo(transition1, _guardCondition1);

            _stateMachine.Start();

            Assert.DoesNotThrow(() => _stateMachine.Trigger(_trigger1));
            Assert.That(_stateMachine.CurrentState.Value.Equals(_stateId2), "Valid transition has been made");
        }

        [Test]
        public void Throw_An_Exception_When_Multiple_Triggered_Transitions_Have_Valid_Target_States()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition1 = NewTransition(_stateId1, _trigger1, _stateId1);
            var transition2 = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition1);
            _stateMachine.AddTransition(transition2);

            _stateMachine.Start();

            Assert.Throws<MultipleValidTransitionsException>(() => _stateMachine.Trigger(_trigger1));
        }

        [Test]
        public void Send_Event_To_Current_State_Until_Any_Event_Handler_Handles_It()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler1);
            _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler2);
            _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler3);

            _stateEventHandler1.HandleEvent(_event1).Returns(false);
            _stateEventHandler2.HandleEvent(_event1).Returns(true);
            _stateEventHandler3.HandleEvent(_event1).Returns(true);

            _stateMachine.Start();

            _stateMachine.SendEvent(_event1);

            _stateEventHandler1.Received(1).HandleEvent(_event1);
            _stateEventHandler2.Received(1).HandleEvent(_event1);
            _stateEventHandler3.DidNotReceive().HandleEvent(Arg.Any<IEvent>());
        }

        [Test]
        public void Do_Nothing_When_Send_Event_Is_Called_While_Not_Running()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler1);

            _stateMachine.SendEvent(_event1);

            _stateEventHandler1.DidNotReceive().HandleEvent(Arg.Any<IEvent>());
        }

        [Test]
        public void Call_Start_Callback_When_It_Finishes()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateMachine.Start(_startCallback);

            _startCallback.Received(1).Invoke();
        }

        [Test]
        public void Call_Stop_Callback_When_It_Finishes()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateMachine.Start();

            _stateMachine.Stop(_stopCallback);

            _stopCallback.Received(1).Invoke();
        }

        [Test]
        public void Call_Update_Callback_When_It_Finishes()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateMachine.Start();

            _stateMachine.Update(_updateCallback);

            _updateCallback.Received(1).Invoke();
        }

        [Test]
        public void Call_Trigger_Callback_With_False_Value_When_It_Finishes_And_No_Transition_Has_Been_Made()
        {
            _stateMachine.Trigger(_trigger1, _triggerCallback);

            _triggerCallback.Received(1).Invoke(false);
        }

        [Test]
        public void Call_Trigger_Callback_With_True_Value_When_It_Finishes_And_A_Transition_Has_Been_Made()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _stateMachine.Start();

            _stateMachine.Trigger(_trigger1, _triggerCallback);

            _triggerCallback.Received(1).Invoke(true);
        }

        [Test]
        public void Call_Send_Event_Callback_With_False_Value_When_It_Finishes_And_The_Event_Was_Not_Handled()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler1);

            _stateEventHandler1.HandleEvent(_event1).Returns(false);

            _stateMachine.Start();

            _stateMachine.SendEvent(_event1, _sendEventCallback);

            _sendEventCallback.Received(1).Invoke(false);
        }

        [Test]
        public void Call_Send_Event_Callback_With_True_Value_When_It_Finishes_And_The_Event_Was_Handled()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler1);

            _stateEventHandler1.HandleEvent(_event1).Returns(true);

            _stateMachine.Start();

            _stateMachine.SendEvent(_event1, _sendEventCallback);

            _sendEventCallback.Received(1).Invoke(true);
        }

        [Test]
        public void Enqueue_Start_Stop_Update_Trigger_And_Send_Event_Actions()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _startCallback.When(obj => obj.Invoke()).Do(_ =>
            {
                _stateMachine.Stop(_stopCallback);
                _stopCallback.DidNotReceive().Invoke();
                _stateMachine.Update(_updateCallback);
                _updateCallback.DidNotReceive().Invoke();
                _stateMachine.Trigger(_trigger1, _triggerCallback);
                _triggerCallback.DidNotReceive().Invoke(Arg.Any<bool>());
                _stateMachine.SendEvent(_event1, _sendEventCallback);
                _sendEventCallback.DidNotReceive().Invoke(Arg.Any<bool>());
            });

            _stateMachine.Start(_startCallback);

            Received.InOrder(() =>
            {
                _startCallback.Invoke();
                _stopCallback.Invoke();
                _updateCallback.Invoke();
                _triggerCallback.Invoke(Arg.Any<bool>());
                _sendEventCallback.Invoke(Arg.Any<bool>());
            });
        }

        [Test]
        public void Prevent_States_From_Being_Removed_When_A_Transition_Is_Being_Evaluated()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);
            _stateMachine.AddState(_stateId3, _stateObject3);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _guardCondition1.When(obj => obj.IsValid()).Do(_ => _stateMachine.RemoveState(_stateId3));

            _stateMachine.AddGuardConditionTo(transition, _guardCondition1);

            _stateMachine.Start();

            Assert.Throws<UnableToRemoveStateMachineElementException>(() => _stateMachine.Trigger(_trigger1));
        }

        [Test]
        public void Let_Remove_States_After_A_Succeeded_Transition()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _stateMachine.Start();

            _stateMachine.Trigger(_trigger1);

            Assert.DoesNotThrow(() => _stateMachine.RemoveState(_stateId1));
            Assert.That(_stateMachine.ContainsState(_stateId1) == false, "State was removed");
        }

        [Test]
        public void Let_Remove_States_After_A_Failed_Transition()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _stateMachine.Start();

            _stateMachine.Trigger(_trigger2);

            Assert.DoesNotThrow(() => _stateMachine.RemoveState(_stateId2));
            Assert.That(_stateMachine.ContainsState(_stateId2) == false, "State was removed");
        }

        [Test]
        public void Let_Remove_States_When_OnBeforeStateChanges_Event_Is_Being_Executed_Except_From_The_Involved_States()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);
            _stateMachine.AddState(_stateId3, _stateObject3);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _onBeforeStateChangesSubscriptor1.When(obj => obj.Invoke(Arg.Any<TState>(), Arg.Any<TTrigger>(), Arg.Any<TState>()))
                .Do(_ =>
                {
                    _stateMachine.RemoveState(_stateId3);
                    _stateMachine.RemoveState(_stateId1);
                });

            _stateMachine.OnBeforeStateChanges += _onBeforeStateChangesSubscriptor1;

            _stateMachine.Start();

            var exception = Assert.Throws<ProtectedStateException>(() => _stateMachine.Trigger(_trigger1));

            Assert.That(exception.StateId.Equals(_stateId1), "Exception was thrown when tried to remove stateId1 (previous state)");
        }

        [Test]
        public void Let_Remove_States_Including_Previous_State_When_Next_State_Enter_Method_Is_Being_Executed_Except_From_Next_State()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);
            _stateMachine.AddState(_stateId3, _stateObject3);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _stateObject2.When(obj => obj.Enter()).Do(_ =>
            {
                _stateMachine.RemoveState(_stateId3);
                _stateMachine.RemoveState(_stateId1);
                _stateMachine.RemoveState(_stateId2);
            });

            _stateMachine.Start();

            var exception = Assert.Throws<ProtectedStateException>(() => _stateMachine.Trigger(_trigger1));

            Assert.That(exception.StateId.Equals(_stateId2), "Exception was thrown when tried to remove stateId2 (next state)");
        }

        [Test]
        public void Prevent_Initial_State_From_Being_Removed_When_Its_Enter_Method_Is_Being_Execute_On_Start()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateObject1.When(obj => obj.Enter()).Do(_ => _stateMachine.RemoveState(_stateId1));

            Assert.Throws<ProtectedStateException>(() => _stateMachine.Start());
        }

        [Test]
        public void Let_Remove_Current_State_When_Its_Exit_Method_Is_Being_Execute_On_Stop()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateObject1.When(obj => obj.Exit()).Do(_ => _stateMachine.RemoveState(_stateId1));

            _stateMachine.Start();

            Assert.DoesNotThrow(() => _stateMachine.Stop());
            Assert.That(_stateMachine.ContainsState(_stateId1) == false, "State was removed");
        }

        [Test]
        public void Prevent_Transitions_From_Being_Removed_When_A_Transition_Is_Being_Evaluated()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _guardCondition1.When(obj => obj.IsValid()).Do(_ => _stateMachine.RemoveTransition(transition));

            _stateMachine.AddGuardConditionTo(transition, _guardCondition1);

            _stateMachine.Start();

            Assert.Throws<UnableToRemoveStateMachineElementException>(() => _stateMachine.Trigger(_trigger1));
        }

        [Test]
        public void Prevent_Transitions_From_Being_Added_When_A_Transition_Is_Being_Evaluated()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition1 = NewTransition(_stateId1, _trigger1, _stateId2);
            var transition2 = NewTransition(_stateId1, _trigger1, _stateId1);

            _stateMachine.AddTransition(transition1);

            _guardCondition1.When(obj => obj.IsValid()).Do(_ => _stateMachine.AddTransition(transition2));

            _stateMachine.AddGuardConditionTo(transition1, _guardCondition1);

            _stateMachine.Start();

            Assert.Throws<UnableToAddStateMachineElementException>(() => _stateMachine.Trigger(_trigger1));
        }

        [Test]
        public void Let_Remove_Transitions_When_A_Transition_Is_Taking_Place()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _stateObject1.When(obj => obj.Exit()).Do(_ => _stateMachine.RemoveTransition(transition));

            _stateMachine.Start();

            Assert.DoesNotThrow(() => _stateMachine.Trigger(_trigger1));
        }

        [Test]
        public void Let_Remove_Or_Add_Transitions_After_A_Succeeded_Transition()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _stateMachine.Start();

            _stateMachine.Trigger(_trigger1); //Succeeds

            _stateMachine.RemoveTransition(transition);

            Assert.That(_stateMachine.ContainsTransition(transition) == false, "Transition was removed");

            _stateMachine.AddTransition(transition);

            Assert.That(_stateMachine.ContainsTransition(transition), "Transition was added");
        }

        [Test]
        public void Let_Remove_Or_Add_Transitions_After_A_Failed_Transition()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _stateMachine.Start();

            _stateMachine.Trigger(_trigger2); //Fails

            _stateMachine.RemoveTransition(transition);

            Assert.That(_stateMachine.ContainsTransition(transition) == false, "Transition was removed");

            _stateMachine.AddTransition(transition);

            Assert.That(_stateMachine.ContainsTransition(transition), "Transition was added");
        }

        [Test]
        public void Prevent_Guard_Conditions_From_Being_Removed_When_A_Transition_Is_Being_Evaluated()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _guardCondition1.When(obj => obj.IsValid()).Do(_ => _stateMachine.RemoveGuardConditionFrom(transition, _guardCondition1));

            _stateMachine.AddGuardConditionTo(transition, _guardCondition1);

            _stateMachine.Start();

            Assert.Throws<UnableToRemoveStateMachineElementException>(() => _stateMachine.Trigger(_trigger1));
        }

        [Test]
        public void Prevent_Guard_Conditions_From_Being_Added_When_A_Transition_Is_Being_Evaluated()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _guardCondition1.When(obj => obj.IsValid()).Do(_ => _stateMachine.AddGuardConditionTo(transition, _guardCondition2));

            _stateMachine.AddGuardConditionTo(transition, _guardCondition1);

            _stateMachine.Start();

            Assert.Throws<UnableToAddStateMachineElementException>(() => _stateMachine.Trigger(_trigger1));
        }

        [Test]
        public void Let_Remove_Guard_Conditions_When_A_Transition_Is_Taking_Place()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _guardCondition1.IsValid().Returns(true);

            _stateMachine.AddGuardConditionTo(transition, _guardCondition1);

            _stateObject1.When(obj => obj.Exit()).Do(_ => _stateMachine.RemoveGuardConditionFrom(transition, _guardCondition1));

            _stateMachine.Start();

            Assert.DoesNotThrow(() => _stateMachine.Trigger(_trigger1));
            _stateObject1.Received(1).Exit();
        }

        [Test]
        public void Let_Remove_Or_Add_Guard_Conditions_After_A_Succeeded_Transition()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _guardCondition1.IsValid().Returns(true);

            _stateMachine.AddGuardConditionTo(transition, _guardCondition1);

            _stateMachine.Start();

            _stateMachine.Trigger(_trigger1);

            _stateMachine.RemoveGuardConditionFrom(transition, _guardCondition1);

            Assert.That(_stateMachine.ContainsGuardConditionOn(transition, _guardCondition1) == false, "Guard condition was removed");

            _stateMachine.AddGuardConditionTo(transition, _guardCondition1);

            Assert.That(_stateMachine.ContainsGuardConditionOn(transition, _guardCondition1), "Guard condition was added");
        }

        [Test]
        public void Let_Remove_Or_Add_Guard_Conditions_After_A_Failed_Transition()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _guardCondition1.IsValid().Returns(false);

            _stateMachine.AddGuardConditionTo(transition, _guardCondition1);

            _stateMachine.Start();

            _stateMachine.Trigger(_trigger1);

            _stateMachine.RemoveGuardConditionFrom(transition, _guardCondition1);

            Assert.That(_stateMachine.ContainsGuardConditionOn(transition, _guardCondition1) == false, "Guard condition was removed");

            _stateMachine.AddGuardConditionTo(transition, _guardCondition1);

            Assert.That(_stateMachine.ContainsGuardConditionOn(transition, _guardCondition1), "Guard condition was added");
        }

        [Test]
        public void Prevent_Event_Handlers_From_Being_Removed_While_Sending_An_Event()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateEventHandler1.When(obj => obj.HandleEvent(Arg.Any<IEvent>()))
                .Do(_ => _stateMachine.RemoveEventHandlerFrom(_stateId1, _stateEventHandler1));

            _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler1);

            _stateMachine.Start();

            Assert.Throws<UnableToRemoveStateMachineElementException>(() => _stateMachine.SendEvent(_event1));
        }

        [Test]
        public void Prevent_Event_Handlers_From_Being_Added_While_Sending_An_Event()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateEventHandler1.When(obj => obj.HandleEvent(Arg.Any<IEvent>()))
                .Do(_ => _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler2));

            _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler1);

            _stateMachine.Start();

            Assert.Throws<UnableToAddStateMachineElementException>(() => _stateMachine.SendEvent(_event1));
        }

        [Test]
        public void Let_Add_Or_Remove_Event_Handlers_After_Sending_An_Event_That_Was_Handled()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateEventHandler1.HandleEvent(Arg.Any<IEvent>()).Returns(true);

            _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler1);

            _stateMachine.Start();

            _stateMachine.SendEvent(_event1);

            Assert.DoesNotThrow(() => _stateMachine.RemoveEventHandlerFrom(_stateId1, _stateEventHandler1));

            Assert.That(_stateMachine.ContainsEventHandlerOn(_stateId1, _stateEventHandler1) == false, "Event handler was removed");

            Assert.DoesNotThrow(() => _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler1));

            Assert.That(_stateMachine.ContainsEventHandlerOn(_stateId1, _stateEventHandler1), "Event handler was added");
        }

        [Test]
        public void Let_Add_Or_Remove_Event_Handlers_After_Sending_An_Event_That_Was_Not_Handled()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateEventHandler1.HandleEvent(Arg.Any<IEvent>()).Returns(false);

            _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler1);

            _stateMachine.Start();

            _stateMachine.SendEvent(_event1);

            Assert.DoesNotThrow(() => _stateMachine.RemoveEventHandlerFrom(_stateId1, _stateEventHandler1));

            Assert.That(_stateMachine.ContainsEventHandlerOn(_stateId1, _stateEventHandler1) == false, "Event handler was removed");

            Assert.DoesNotThrow(() => _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler1));

            Assert.That(_stateMachine.ContainsEventHandlerOn(_stateId1, _stateEventHandler1), "Event handler was added");
        }
    }
}