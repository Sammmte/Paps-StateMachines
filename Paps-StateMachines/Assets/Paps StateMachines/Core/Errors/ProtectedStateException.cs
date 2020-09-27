using System;

namespace Paps.StateMachines
{
    public class ProtectedStateException : Exception
    {
        public object StateMachine { get; }
        public object StateId { get; }

        public ProtectedStateException(object stateMachine, object stateId)
        {
            StateMachine = stateMachine;
            StateId = stateId;
        }
    }
}