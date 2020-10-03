namespace Paps.StateMachines
{
    public interface IHierarchicalStateMachine<TState, TTrigger> : IStateMachine<TState, TTrigger>, IGuardedStateMachine<TState, TTrigger>,
        IEventDispatcherStateMachine<TState, TTrigger>
    {
        event HierarchyPathChanged<TTrigger> OnBeforeActiveHierarchyPathChanges;
        event HierarchyPathChanged<TTrigger> OnActiveHierarchyPathChanged;

        void AddChildTo(TState parentState, TState childState);
        bool RemoveChildFromParent(TState childState);

        bool AreImmediateParentAndChild(TState parentState, TState childState);

        HierarchyPath<TState> GetActiveHierarchyPath();

        TState[] GetImmediateChildsOf(TState parentId);

        TState GetParentOf(TState childId);

        void SetInitialStateOf(TState parentState, TState initialState);
        TState GetInitialStateOf(TState parentState);

        TState[] GetRoots();
    }
}