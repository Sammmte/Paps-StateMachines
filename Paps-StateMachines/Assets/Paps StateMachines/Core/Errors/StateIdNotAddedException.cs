using System;

namespace Paps.StateMachines
{
    public class StateIdNotAddedException : Exception
    {
        public object StateMachine { get; }
        public object StateId { get; }

        public StateIdNotAddedException(object stateMachine, object stateId) : this(stateMachine, stateId, "No state id was added to state machine")
        {
            
        }

        public StateIdNotAddedException(object stateMachine, object stateId, string message) : base(message)
        {
            StateMachine = stateMachine;
            StateId = stateId;
        }
    }
}
