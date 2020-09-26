using System;

namespace Paps.StateMachines
{
    public class TransitionNotAddedException : Exception
    {
        public object StateMachine { get; }
        public object StateFrom { get; }
        public object Trigger { get; }
        public object StateTo { get; }

        public TransitionNotAddedException(object stateMachine, object stateFrom, object trigger, object stateTo) 
            : this(stateMachine, stateFrom, trigger, stateTo, "Check StateFrom, Trigger and StateTo properties for more information")
        {

        }

        public TransitionNotAddedException(object stateMachine, object stateFrom, object trigger, object stateTo, string message) 
            : base(message)
        {
            StateMachine = stateMachine;
            StateFrom = stateFrom;
            Trigger = trigger;
            StateTo = stateTo;
        }
    }
}
