using System;

public class MultipleValidTransitionsException : Exception
{
    public object StateMachine { get; }
    public object[] Transitions { get; }

    public MultipleValidTransitionsException(object stateMachine, params object[] transitions) 
        : base("There are multiple transitions with valid targets. " + 
            "You may want to check your guard conditions or add some for preventing this exception")
    {
        StateMachine = stateMachine;
        Transitions = transitions;
    }
}
