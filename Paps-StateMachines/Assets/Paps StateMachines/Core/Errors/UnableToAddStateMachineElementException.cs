using System;

namespace Paps.StateMachines
{
    public class UnableToAddStateMachineElementException : Exception
    {
        public object StateMachine { get; }
        public object Element { get; }
        public Type ElementType { get; }

        public UnableToAddStateMachineElementException(object stateMachine, object element)
        {
            StateMachine = stateMachine;
            Element = element;
            ElementType = Element.GetType();
        }
    }
}
