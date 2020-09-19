using System;

namespace Paps.StateMachines
{
    public class UnableToAddException : Exception
    {
        public object FailedToAddObject { get; private set; }

        public UnableToAddException(object failedToAddObject) : base("State machine was unable to add an object. It could be a state, a transition, a guard condition, etc." +
             "Check FailedToAddObject property for more information")
        {
            FailedToAddObject = failedToAddObject;
        }
    }
}
