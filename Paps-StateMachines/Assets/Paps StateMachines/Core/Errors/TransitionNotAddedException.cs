using System;

namespace Paps.StateMachines
{
    public class TransitionNotAddedException : Exception
    {
        public object StateFrom { get; private set; }
        public object Trigger { get; private set; }
        public object StateTo { get; private set; }

        public TransitionNotAddedException(object stateFrom, object trigger, object stateTo) 
            : base("Check StateFrom, Trigger and StateTo properties for more information")
        {
            StateFrom = stateFrom;
            Trigger = trigger;
            StateTo = stateTo;
        }
    }
}
