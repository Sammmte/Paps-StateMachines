using NSubstitute;
using NUnit.Framework;
using Paps.StateMachines;
using System;

namespace Tests.HierarchicalStateMachine
{
    public abstract class HierarchicalStateMachineShould<TState, TTrigger>
    {
        protected class TestState : IState
        {
            public void Enter()
            {

            }

            public void Exit()
            {

            }

            public void Update()
            {

            }
        }

        protected abstract HierarchicalStateMachine<TState, TTrigger> NewStateMachine();
        protected abstract HierarchicalStateMachine<T, U> NewStateMachine<T, U>();

        protected abstract TState NewStateId();

        protected abstract TTrigger NewTrigger();

        protected abstract Transition<TState, TTrigger> NewTransition();

        protected abstract Transition<TState, TTrigger> NewTransition(TState stateFrom, TTrigger trigger, TState stateTo);
        protected abstract Transition<T, U> NewTransition<T, U>(T stateFrom, U trigger, T stateTo);

        protected IHierarchicalStateMachine<TState, TTrigger> _stateMachine;
        protected TState _stateId1, _stateId2, _stateId3, _stateId4, _stateId5;
        protected TTrigger _trigger1, _trigger2, _trigger3, _trigger4, _trigger5;
        protected IState _stateObject1, _stateObject2, _stateObject3, _stateObject4, _stateObject5;
        protected IGuardCondition _guardCondition1, _guardCondition2, _guardCondition3, _guardCondition4, _guardCondition5;
        protected IStateEventHandler _stateEventHandler1, _stateEventHandler2, _stateEventHandler3, _stateEventHandler4, _stateEventHandler5;
        protected IEvent _event1;
        protected HierarchyPathChanged<TTrigger> _onBeforeStateChangesSubscriptor1, _onBeforeStateChangesSubscriptor2,
            _onBeforeStateChangesSubscriptor3;
        protected HierarchyPathChanged<TTrigger> _onStateChangedSubscriptor1, _onStateChangedSubscriptor2, _onStateChangedSubscriptor3;
        protected Action _startCallback;
        protected Action _stopCallback;
        protected Action _updateCallback;
        protected Action<bool> _triggerCallback;
        protected Action<bool> _sendEventCallback;

        [SetUp]
        public void SetUp()
        {
            _stateMachine = NewStateMachine();

            _stateId1 = NewStateId();
            _stateId2 = NewStateId();
            _stateId3 = NewStateId();
            _stateId4 = NewStateId();
            _stateId5 = NewStateId();

            _trigger1 = NewTrigger();
            _trigger2 = NewTrigger();
            _trigger3 = NewTrigger();
            _trigger4 = NewTrigger();
            _trigger5 = NewTrigger();

            _stateObject1 = Substitute.For<IState>();
            _stateObject2 = Substitute.For<IState>();
            _stateObject3 = Substitute.For<IState>();
            _stateObject4 = Substitute.For<IState>();
            _stateObject5 = Substitute.For<IState>();

            _guardCondition1 = Substitute.For<IGuardCondition>();
            _guardCondition2 = Substitute.For<IGuardCondition>();
            _guardCondition3 = Substitute.For<IGuardCondition>();
            _guardCondition4 = Substitute.For<IGuardCondition>();
            _guardCondition5 = Substitute.For<IGuardCondition>();

            _stateEventHandler1 = Substitute.For<IStateEventHandler>();
            _stateEventHandler2 = Substitute.For<IStateEventHandler>();
            _stateEventHandler3 = Substitute.For<IStateEventHandler>();
            _stateEventHandler4 = Substitute.For<IStateEventHandler>();
            _stateEventHandler5 = Substitute.For<IStateEventHandler>();

            _event1 = Substitute.For<IEvent>();

            _onBeforeStateChangesSubscriptor1 = Substitute.For<HierarchyPathChanged<TTrigger>>();
            _onBeforeStateChangesSubscriptor2 = Substitute.For<HierarchyPathChanged<TTrigger>>();
            _onBeforeStateChangesSubscriptor3 = Substitute.For<HierarchyPathChanged<TTrigger>>();

            _onStateChangedSubscriptor1 = Substitute.For<HierarchyPathChanged<TTrigger>>();
            _onStateChangedSubscriptor2 = Substitute.For<HierarchyPathChanged<TTrigger>>();
            _onStateChangedSubscriptor3 = Substitute.For<HierarchyPathChanged<TTrigger>>();

            _startCallback = Substitute.For<Action>();
            _stopCallback = Substitute.For<Action>();
            _updateCallback = Substitute.For<Action>();
            _triggerCallback = Substitute.For<Action<bool>>();
            _sendEventCallback = Substitute.For<Action<bool>>();
        }
    }
}