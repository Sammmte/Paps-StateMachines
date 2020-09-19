using System;

namespace Paps.StateMachines
{
    public class StateIdNotAddedException : Exception
    {
        public object StateId { get; private set; }

        public StateIdNotAddedException(object stateId) : base("No state id was added to state machine")
        {

        }
    }
}
