using Paps.StateMachines;

namespace Tests.HierarchicalStateMachine.WithStructs
{
    public class HierarchicalStateMachineWithStructsShould : HierarchicalStateMachineShould<int, int>
    {
        private int _stateInt;
        private int _triggerInt;

        protected override int NewStateId()
        {
            return _stateInt++;
        }

        protected override HierarchicalStateMachine<int, int> NewStateMachine()
        {
            return NewStateMachine<int, int>();
        }

        protected override HierarchicalStateMachine<T, U> NewStateMachine<T, U>()
        {
            return new HierarchicalStateMachine<T, U>();
        }

        protected override Transition<int, int> NewTransition()
        {
            return new Transition<int, int>(NewStateId(), NewTrigger(), NewStateId());
        }

        protected override Transition<int, int> NewTransition(int sourceTarget, int trigger, int targetState)
        {
            return NewTransition<int, int>(sourceTarget, trigger, targetState);
        }

        protected override Transition<T, U> NewTransition<T, U>(T sourceTarget, U trigger, T targetState)
        {
            return new Transition<T, U>(sourceTarget, trigger, targetState);
        }

        protected override int NewTrigger()
        {
            return _triggerInt++;
        }
    }
}