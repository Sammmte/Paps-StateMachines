using System;

namespace Paps.StateMachines
{
    public class CannotAddChildException : Exception
    {
        public object StateMachine { get; }
        public object ChildStateId { get; }

        public CannotAddChildException(object stateMachine, object childStateId, string message) : base(message)
        {
            StateMachine = stateMachine;
            ChildStateId = childStateId;
        }
    }
}