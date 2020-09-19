using Paps.StateMachines;

namespace Tests.WithStructs
{
    public class EventDispatcherStateMachineExtensionsWithStructsShould : EventDispatcherStateMachineExtensionsShould<int, int>
    {
        private int _stateInt;
        private int _triggerInt;

        protected override int NewStateId()
        {
            return _stateInt++;
        }

        protected override IEventDispatcherStateMachine<int, int> NewStateMachine()
        {
            return NewStateMachine<int, int>();
        }

        protected override IEventDispatcherStateMachine<T, U> NewStateMachine<T, U>()
        {
            return new PlainStateMachine<T, U>();
        }

        protected override Transition<int, int> NewTransition()
        {
            return new Transition<int, int>(NewStateId(), NewTrigger(), NewStateId());
        }

        protected override Transition<int, int> NewTransition(int stateFrom, int trigger, int stateTo)
        {
            return NewTransition<int, int>(stateFrom, trigger, stateTo);
        }

        protected override Transition<T, U> NewTransition<T, U>(T stateFrom, U trigger, T stateTo)
        {
            return new Transition<T, U>(stateFrom, trigger, stateTo);
        }

        protected override int NewTrigger()
        {
            return _triggerInt++;
        }
    }
}
