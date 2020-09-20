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

        protected abstract Transition<TState, TTrigger> NewTransition(TState stateFrom, TTrigger trigger, TState stateTo);
        protected abstract Transition<T, U> NewTransition<T, U>(T stateFrom, U trigger, T stateTo);

        protected IPlainStateMachine<TState, TTrigger> _stateMachine;
        protected TState _stateId1, _stateId2, _stateId3, _stateId4, _stateId5;
        protected TTrigger _trigger1, _trigger2, _trigger3, _trigger4, _trigger5;
        protected IState _stateObject1, _stateObject2, _stateObject3, _stateObject4, _stateObject5;
        protected IGuardCondition _guardCondition1, _guardCondition2, _guardCondition3, _guardCondition4, _guardCondition5;
        protected IStateEventHandler _stateEventHandler1, _stateEventHandler2, _stateEventHandler3, _stateEventHandler4, _stateEventHandler5;

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

            Assert.AreEqual(_stateObject1, _stateMachine.GetStateById(_stateId1));
            Assert.AreEqual(_stateObject2, _stateMachine.GetStateById(_stateId2));
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
        public void Remove_Transitions_Related_To_A_State_Id_When_It_Is_Removed()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);
            _stateMachine.AddState(_stateId2, _stateObject2);

            var transition = NewTransition(_stateId1, _trigger1, _stateId2);

            _stateMachine.AddTransition(transition);

            _stateMachine.RemoveState(_stateId1);

            Assert.That(_stateMachine.ContainsTransition(transition) == false, "Transition was removed");

            _stateMachine.AddState(_stateId1, _stateObject1);

            Assert.That(_stateMachine.ContainsTransition(transition) == false, "Transition was removed");
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
        public void Remove_Guard_Conditions_Related_To_A_Removed_Transition_When_A_Related_State_Is_Removed()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            var transition = NewTransition(_stateId1, _trigger1, _stateId1);

            _stateMachine.AddTransition(transition);

            _stateMachine.AddGuardConditionTo(transition, _guardCondition1);

            _stateMachine.RemoveState(_stateId1);

            Assert.That(_stateMachine.ContainsGuardConditionOn(transition, _guardCondition1) == false, "Guard condition was removed");

            _stateMachine.AddState(_stateId1, _stateObject1);

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
        public void Remove_Event_Handlers_When_Their_Related_State_Is_Removed()
        {
            _stateMachine.AddState(_stateId1, _stateObject1);

            _stateMachine.AddEventHandlerTo(_stateId1, _stateEventHandler1);

            _stateMachine.RemoveState(_stateId1);

            Assert.That(_stateMachine.ContainsEventHandlerOn(_stateId1, _stateEventHandler1) == false, "Event handler was removed");

            _stateMachine.AddState(_stateId1, _stateObject1);

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


    }
}