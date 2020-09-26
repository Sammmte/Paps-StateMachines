using System;
using System.Linq;

public class MultipleValidTransitionsFromSameStateException : Exception
{
    public object StateMachine { get; }
    public object StateFrom { get; }
    public object Trigger { get; }
    public object[] PossibleStateTos { get; }

    public MultipleValidTransitionsFromSameStateException(object stateMachine, object stateFrom, object trigger, params object[] targetStates) 
        : base("There are multiple transitions with valid targets. " + 
            "You may want to check your guard conditions or add some for preventing this exception")
    {
        StateMachine = stateMachine;
        StateFrom = stateFrom;
        Trigger = trigger;
        PossibleStateTos = targetStates;
    }
}
