using NSubstitute;
using NUnit.Framework;
using Paps.StateMachines;
using Paps.StateMachines.Extensions;
using System;

namespace Tests.StateMachineExtensions
{
    public abstract class EventDispatcherStateMachineExtensionsShould<TState, TTrigger>
    {
        protected abstract IEventDispatcherStateMachine<TState, TTrigger> NewStateMachine();
        protected abstract IEventDispatcherStateMachine<T, U> NewStateMachine<T, U>();

        protected abstract TState NewStateId();

        protected abstract TTrigger NewTrigger();

        protected abstract Transition<TState, TTrigger> NewTransition();

        protected abstract Transition<TState, TTrigger> NewTransition(TState stateFrom, TTrigger trigger, TState stateTo);
        protected abstract Transition<T, U> NewTransition<T, U>(T stateFrom, U trigger, T stateTo);

        [Test]
        public void Add_Multiple_Event_Handlers()
        {
            var fsm = NewStateMachine();

            IStateEventHandler eventHandler1 = Substitute.For<IStateEventHandler>();
            IStateEventHandler eventHandler2 = Substitute.For<IStateEventHandler>();

            var stateId1 = NewStateId();

            fsm.AddEmpty(stateId1);

            fsm.SubscribeEventHandlersTo(stateId1, eventHandler1, eventHandler2);

            Assert.IsTrue(fsm.ContainsEventHandlerOn(stateId1, eventHandler1));
            Assert.IsTrue(fsm.ContainsEventHandlerOn(stateId1, eventHandler2));
        }

        [Test]
        public void Add_And_Remove_Event_Handlers_From_Delegates()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddEmpty(stateId1);

            Func<IEvent, bool> method1 = Substitute.For<Func<IEvent, bool>>();
            Func<IEvent, bool> method2 = Substitute.For<Func<IEvent, bool>>();

            IEvent stateEvent = Substitute.For<IEvent>();

            method1.Invoke(stateEvent).Returns(false);
            method2.Invoke(stateEvent).Returns(true);

            fsm.SubscribeEventHandlerTo(stateId1, method1);
            fsm.SubscribeEventHandlerTo(stateId1, method2);

            Assert.IsTrue(fsm.HasEventHandler(stateId1, method1));
            Assert.IsTrue(fsm.HasEventHandler(stateId1, method2));
        }

        [Test]
        public void Add_Multiple_Event_Handlers_From_Methods()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddEmpty(stateId1);

            Func<IEvent, bool> method1 = Substitute.For<Func<IEvent, bool>>();
            Func<IEvent, bool> method2 = Substitute.For<Func<IEvent, bool>>();

            fsm.SubscribeEventHandlersTo(stateId1, method1, method2);

            Assert.IsTrue(fsm.HasEventHandler(stateId1, method1));
            Assert.IsTrue(fsm.HasEventHandler(stateId1, method2));
        }
    }
}
