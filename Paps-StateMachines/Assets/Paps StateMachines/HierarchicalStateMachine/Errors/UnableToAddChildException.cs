using System;

namespace Paps.StateMachines
{
    public class UnableToAddChildException : Exception
    {
        public object StateMachine { get; }
        public object ParentState { get; }
        public object ChildState { get; }

        public UnableToAddChildException(object stateMachine, object parentState, object childState, 
            string message = "") : base(message)
        {
            StateMachine = stateMachine;
            ParentState = parentState;
            ChildState = childState;
        }
    }
}