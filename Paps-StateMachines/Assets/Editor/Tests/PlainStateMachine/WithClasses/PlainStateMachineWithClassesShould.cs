using NUnit.Framework;
using Paps.StateMachines;
using System;

namespace Tests.PlainStateMachine.WithClasses
{
    public class PlainStateMachineWithClassesShould : PlainStateMachineShould<string, string>
    {
        protected override string NewStateId()
        {
            return Guid.NewGuid().ToString();
        }

        protected override PlainStateMachine<string, string> NewStateMachine()
        {
            return NewStateMachine<string, string>();
        }

        protected override PlainStateMachine<T, U> NewStateMachine<T, U>()
        {
            return new PlainStateMachine<T, U>();
        }

        protected override Transition<string, string> NewTransition()
        {
            return new Transition<string, string>(NewStateId(), NewTrigger(), NewStateId());
        }

        protected override Transition<string, string> NewTransition(string sourceTarget, string trigger, string targetState)
        {
            return NewTransition<string, string>(sourceTarget, trigger, targetState);
        }

        protected override Transition<T, U> NewTransition<T, U>(T sourceTarget, U trigger, T targetState)
        {
            return new Transition<T, U>(sourceTarget, trigger, targetState);
        }

        protected override string NewTrigger()
        {
            return Guid.NewGuid().ToString();
        }

        [Test]
        public void Throw_An_Exception_When_User_Adds_A_Null_State_Id()
        {
            Assert.Throws<ArgumentNullException>(() => _stateMachine.AddState(null, _stateObject1));
        }

        [Test]
        public void Throw_An_Exception_When_User_Asks_If_Contains_State_With_A_Null_Id()
        {
            Assert.Throws<ArgumentNullException>(() => _stateMachine.ContainsState(null));
        }
    }
}
