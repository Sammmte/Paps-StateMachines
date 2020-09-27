using System;

namespace Paps.StateMachines
{
    public class ProtectedStateException : Exception
    {
        public object StateMachine { get; }
        public object StateId { get; }

        public ProtectedStateException(object stateMachine, object stateId, string message) : base(message)
        {
            StateMachine = stateMachine;
            StateId = stateId;
        }
    }
}