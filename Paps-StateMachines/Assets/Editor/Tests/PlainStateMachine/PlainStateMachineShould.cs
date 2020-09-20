using NSubstitute;
using NUnit.Framework;
using Paps.StateMachines;

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

        private IPlainStateMachine<TState, TTrigger> _stateMachine;

        private TState _stateId1, _stateId2, _stateId3, _stateId4, _stateId5;
        private TTrigger _trigger1, _trigger2, _trigger3, _trigger4, _trigger5;
        private IState _stateObject1, _stateObject2, _stateObject3, _stateObject4, _stateObject5;

        [SetUp]
        public void SetUp()
        {
            _stateMachine = NewStateMachine();

            _stateId1 = _stateId2 = _stateId3 = _stateId4 = _stateId5 = NewStateId();
            _trigger1 = _trigger2 = _trigger3 = _trigger4 = _trigger5 = NewTrigger();
            _stateObject1 = _stateObject2 = _stateObject3 = _stateObject4 = _stateObject5 = Substitute.For<IState>();
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
    }
}