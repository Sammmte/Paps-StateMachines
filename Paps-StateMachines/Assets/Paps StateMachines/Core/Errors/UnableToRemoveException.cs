using System;

namespace Paps.StateMachines
{
    public class UnableToRemoveException : Exception
    {
        public object FailedToRemoveObject { get; private set; }

        public UnableToRemoveException(object failedToRemoveObject) : base("State machine was unable to remove an object. It could be a state, a transition, a guard condition, etc." +
             "Check FailedToRemoveObject property for more information")
        {
            FailedToRemoveObject = failedToRemoveObject;
        }
    }
}
