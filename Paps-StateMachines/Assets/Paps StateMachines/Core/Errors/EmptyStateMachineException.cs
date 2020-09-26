using System;

namespace Paps.StateMachines
{
    public class EmptyStateMachineException : Exception
    {
        public object StateMachine { get; }

        public EmptyStateMachineException(object stateMachine) : this(stateMachine, "State machine has no states")
        {

        }

        public EmptyStateMachineException(object stateMachine, string message) : base(message)
        {
            StateMachine = stateMachine;
        }
    }
}


