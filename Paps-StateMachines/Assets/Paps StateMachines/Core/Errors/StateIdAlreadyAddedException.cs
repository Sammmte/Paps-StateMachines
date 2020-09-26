using System;

namespace Paps.StateMachines
{
    public class StateIdAlreadyAddedException : Exception
    {
        public object StateMachine { get; }
        public object StateId { get; }

        public StateIdAlreadyAddedException(object stateMachine, object stateId) : this(stateMachine, stateId, "State id already added to state machine")
        {

        }

        public StateIdAlreadyAddedException(object stateMachine, object stateId, string message) : base(message)
        {
            StateMachine = stateMachine;
            StateId = stateId;
        }
    }
}
