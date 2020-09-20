using NSubstitute;
using NUnit.Framework;
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
    }
}