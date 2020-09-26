using System;

namespace Paps.StateMachines
{
    public class InvalidInitialStateException : Exception
    {
        public object StateMachine { get; }

        public InvalidInitialStateException(object stateMachine) : this(stateMachine, "Initial state is invalid")
        {

        }

        public InvalidInitialStateException(object stateMachine, string message) : base(message)
        {
            StateMachine = stateMachine;
        }
    }
}
