using System;

namespace Paps.StateMachines
{
    public class StateMachineRunningException : Exception
    {
        public object StateMachine { get; }

        public StateMachineRunningException(object stateMachine) : this(stateMachine, "State machine is running, so the requested operation cannot be done")
        {
            
        }

        public StateMachineRunningException(object stateMachine, string message) : base(message)
        {
            StateMachine = stateMachine;
        }
    }
}
