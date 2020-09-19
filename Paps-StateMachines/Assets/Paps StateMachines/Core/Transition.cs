using System;

namespace Paps.StateMachines
{
    public struct Transition<TState, TTrigger>
    {
        public TState StateFrom { get; private set; }
        public TTrigger Trigger { get; private set; }
        public TState StateTo { get; private set; }

        public Transition(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            StateFrom = stateFrom;
            Trigger = trigger;
            StateTo = stateTo;
        }
    }
}
