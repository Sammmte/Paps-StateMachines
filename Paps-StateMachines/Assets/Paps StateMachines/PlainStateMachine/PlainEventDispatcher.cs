namespace Paps.StateMachines
{
    internal class PlainEventDispatcher<TState, TTrigger>
    {
        private PlainStateEventHandlerCollection<TState, TTrigger> _eventHandlers;
        private PlainStateBehaviourScheduler<TState, TTrigger> _stateBehaviourScheduler;

        public PlainEventDispatcher(PlainStateEventHandlerCollection<TState, TTrigger> eventHandlers, PlainStateBehaviourScheduler<TState, TTrigger> stateBehaviourScheduler)
        {
            _eventHandlers = eventHandlers;
            _stateBehaviourScheduler = stateBehaviourScheduler;
        }

        public bool SendEvent(IEvent ev)
        {
            if (_stateBehaviourScheduler.IsRunning == false)
                return false;

            var currentState = _stateBehaviourScheduler.CurrentState.Value;

            _eventHandlers.LockEventHandlersOf(currentState);

            var eventHandlers = _eventHandlers.GetEventHandlersOf(currentState);

            for(int i = 0; i < eventHandlers.Length; i++)
            {
                if (eventHandlers[i].HandleEvent(ev))
                {
                    _eventHandlers.UnlockEventHandlersOf(currentState);
                    return true;
                }
            }

            _eventHandlers.UnlockEventHandlersOf(currentState);

            return false;
        }
    }
}