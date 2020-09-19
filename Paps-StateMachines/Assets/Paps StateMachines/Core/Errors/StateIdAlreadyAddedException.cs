using System;

namespace Paps.StateMachines
{
    public class StateIdAlreadyAddedException : Exception
    {
        public object StateId { get; private set; }

        public StateIdAlreadyAddedException(object stateId) : base("State id already added to state machine")
        {
            StateId = stateId;
        }
    }
}
