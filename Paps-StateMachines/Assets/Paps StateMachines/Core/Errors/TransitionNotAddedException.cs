using System;

namespace Paps.StateMachines
{
    public class TransitionNotAddedException : Exception
    {
        public object StateMachine { get; }
        public object SourceState { get; }
        public object Trigger { get; }
        public object TargetState { get; }

        public TransitionNotAddedException(object stateMachine, object sourceTarget, object trigger, object targetState, string message = "") 
            : base(message)
        {
            StateMachine = stateMachine;
            SourceState = sourceTarget;
            Trigger = trigger;
            TargetState = targetState;
        }
    }
}
