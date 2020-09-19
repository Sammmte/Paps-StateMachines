using System;
using System.Linq;

public class MultipleValidTransitionsFromSameStateException : Exception
{
    public object StateFrom { get; private set; }
    public object Trigger { get; private set; }
    public object[] _possibleStateTos;

    public object[] PossibleStateTos
    {
        get
        {
            return _possibleStateTos.ToArray();
        }
    }

    public MultipleValidTransitionsFromSameStateException(object stateFrom, object trigger, params object[] targetStates) 
        : base("There are multiple transitions with valid targets. " + 
            "You may want to check your guard conditions or add some for preventing this exception")
    {
        StateFrom = stateFrom;
        Trigger = trigger;
        _possibleStateTos = targetStates;
    }
}
