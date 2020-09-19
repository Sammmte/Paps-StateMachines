using Paps.StateMachines.Extensions.BehaviouralStates;
using System;

namespace Paps.StateMachines.Extensions
{
    public static class States
    {
        public static EmptyState Empty()
        {
            return new EmptyState();
        }

        public static TimerState Timer(TimeSpan time, Action onTimerElapsed)
        {
            return new TimerState(time, onTimerElapsed);
        }

        public static DelegateState WithEvents(Action onEnter, Action onUpdate, Action onExit)
        {
            return new DelegateState(onEnter, onUpdate, onExit);
        }

        public static CompositeState Composite(params IState[] innerStates)
        {
            return new CompositeState(innerStates);
        }

        public static BehaviouralState WithBehaviours(params IStateBehaviour[] behaviours)
        {
            return new BehaviouralState(behaviours);
        }
    }
}