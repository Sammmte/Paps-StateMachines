using Paps.StateMachines;
using System;

namespace Tests.StateMachineExtensions.WithClasses
{
    public class EventDispatcherStateMachineExtensionsWithClassesShould : EventDispatcherStateMachineExtensionsShould<string, string>
    {
        protected override string NewStateId()
        {
            return Guid.NewGuid().ToString();
        }

        protected override IEventDispatcherStateMachine<string, string> NewStateMachine()
        {
            return NewStateMachine<string, string>();
        }

        protected override IEventDispatcherStateMachine<T, U> NewStateMachine<T, U>()
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
    }
}
