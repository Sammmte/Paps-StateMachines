using Paps.StateMachines;
using System;

namespace Tests.WithClasses
{
    public class StateMachineExtensionsWithClassesShould : StateMachineExtensionsShould<string, string>
    {
        protected override string NewStateId()
        {
            return Guid.NewGuid().ToString();
        }

        protected override IStateMachine<string, string> NewStateMachine()
        {
            return NewStateMachine<string, string>();
        }

        protected override IStateMachine<T, U> NewStateMachine<T, U>()
        {
            return new PlainStateMachine<T, U>();
        }

        protected override Transition<string, string> NewTransition()
        {
            return new Transition<string, string>(NewStateId(), NewTrigger(), NewStateId());
        }

        protected override Transition<string, string> NewTransition(string stateFrom, string trigger, string stateTo)
        {
            return NewTransition<string, string>(stateFrom, trigger, stateTo);
        }

        protected override Transition<T, U> NewTransition<T, U>(T stateFrom, U trigger, T stateTo)
        {
            return new Transition<T, U>(stateFrom, trigger, stateTo);
        }

        protected override string NewTrigger()
        {
            return Guid.NewGuid().ToString();
        }
    }
}



