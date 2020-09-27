using System;

namespace Paps.StateMachines
{
    public class UnableToRemoveStateMachineElementException : Exception
    {
        public object StateMachine { get; }
        public object Element { get; }

        public UnableToRemoveStateMachineElementException(object stateMachine, object element)
        {
            StateMachine = stateMachine;
            Element = element;
        }
    }
}
