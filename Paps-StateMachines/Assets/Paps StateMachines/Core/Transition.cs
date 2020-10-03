namespace Paps.StateMachines
{
    public struct Transition<TState, TTrigger>
    {
        public TState SourceState { get; private set; }
        public TTrigger Trigger { get; private set; }
        public TState TargetState { get; private set; }

        public Transition(TState sourceState, TTrigger trigger, TState targetState)
        {
            SourceState = sourceState;
            Trigger = trigger;
            TargetState = targetState;
        }
    }
}
