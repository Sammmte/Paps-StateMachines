using NUnit.Framework;
using Paps.StateMachines;
using System.Collections.Generic;
using Paps.StateMachines.Extensions.BehaviouralStates;
using Paps.StateMachines.Extensions;
using System.Linq;
using NSubstitute;
using System;

namespace Tests
{
    public abstract class StateMachineExtensionsShould<TState, TTrigger>
    {
        protected class TestStateBehaviour : IStateBehaviour
        {
            public void OnEnter()
            {

            }

            public void OnExit()
            {

            }

            public void OnUpdate()
            {

            }
        }

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

        protected abstract IStateMachine<TState, TTrigger> NewStateMachine();
        protected abstract IStateMachine<T, U> NewStateMachine<T, U>();

        protected abstract TState NewStateId();

        protected abstract TTrigger NewTrigger();

        protected abstract Transition<TState, TTrigger> NewTransition();

        protected abstract Transition<TState, TTrigger> NewTransition(TState stateFrom, TTrigger trigger, TState stateTo);
        protected abstract Transition<T, U> NewTransition<T, U>(T stateFrom, U trigger, T stateTo);

        [Test]
        public void Get_State_Of_Specific_Type()
        {
            var state1 = new TestState();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddState(stateId1, state1);

            var shouldBeState1 = fsm.GetState<TestState, TState, TTrigger>();

            Assert.AreEqual(state1, shouldBeState1);
        }

        [Test]
        public void Get_States_Of_Specific_Type()
        {
            var state1 = new TestState();
            var state2 = new TestState();
            var state3 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);
            fsm.AddState(stateId3, state3);

            var states = fsm.GetStates<TestState, TState, TTrigger>();

            Assert.That(states.Contains(state1) && 
                states.Contains(state2) && 
                states.Contains(state3) == false, "Contains states of specified type");
        }

        [Test]
        public void Return_Corresponding_Value_When_Asked_If_Contains_By_Reference()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);

            Assert.IsTrue(fsm.ContainsStateByReference(state1));
        }

        [Test]
        public void Add_Timer_State()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddTimerState(stateId1, TimeSpan.FromMilliseconds(1000), () => { });

            Assert.That(fsm.GetStateById(stateId1) is TimerState, "Has a timer state");
        }

        [Test]
        public void Add_Empty()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddEmpty(stateId1);

            Assert.That(fsm.GetStateById(stateId1) is EmptyState, "Has an empty state");
        }

        [Test]
        public void Add_With_Events()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddWithEvents(stateId1, () => { }, () => { }, () => { });

            Assert.That(fsm.GetStateById(stateId1) is DelegateState, "Has a delegate state");
        }

        [Test]
        public void Add_Transition_From_Any_State()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();
            var state3 = Substitute.For<IState>();
            var state4 = Substitute.For<IState>();
            var stateTarget = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();
            var stateId4 = NewStateId();
            var stateId5 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);
            fsm.AddState(stateId3, state3);
            fsm.AddState(stateId4, state4);
            fsm.AddState(stateId5, stateTarget);
            fsm.FromAny(trigger, stateId5);

            Assert.IsTrue(
                fsm.ContainsTransition(NewTransition(stateId1, trigger, stateId5)) &&
                fsm.ContainsTransition(NewTransition(stateId2, trigger, stateId5)) &&
                fsm.ContainsTransition(NewTransition(stateId3, trigger, stateId5)) &&
                fsm.ContainsTransition(NewTransition(stateId4, trigger, stateId5)) &&
                fsm.ContainsTransition(NewTransition(stateId5, trigger, stateId5))
                        );
        }

        [Test]
        public void Add_Transition_From_Any_State_Except_Target()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();
            var state3 = Substitute.For<IState>();
            var state4 = Substitute.For<IState>();
            var stateTarget = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();
            var stateId4 = NewStateId();
            var stateId5 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);
            fsm.AddState(stateId3, state3);
            fsm.AddState(stateId4, state4);
            fsm.AddState(stateId5, stateTarget);
            fsm.FromAnyExceptTarget(trigger, stateId5);

            Assert.IsTrue(
                fsm.ContainsTransition(NewTransition(stateId1, trigger, stateId5)) &&
                fsm.ContainsTransition(NewTransition(stateId2, trigger, stateId5)) &&
                fsm.ContainsTransition(NewTransition(stateId3, trigger, stateId5)) &&
                fsm.ContainsTransition(NewTransition(stateId4, trigger, stateId5)));

            Assert.IsFalse(fsm.ContainsTransition(NewTransition(stateId5, trigger, stateId5)));
        }

        [Test]
        public void Return_Transitions_With_Specific_Trigger()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();
            var stateTarget = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();

            var trigger1 = NewTrigger();
            var trigger2 = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);
            fsm.AddState(stateId3, stateTarget);
            fsm.FromAny(trigger1, stateId3);
            fsm.AddTransition(NewTransition(stateId1, trigger2, stateId1));

            var transitions = fsm.GetTransitionsWithTrigger(trigger1);

            Assert.IsTrue(transitions.Any(t => t.Trigger.Equals(trigger1)));
            Assert.IsFalse(transitions.Any(t => t.Trigger.Equals(trigger2)));
        }

        [Test]
        public void Return_Transitions_With_Specific_State_From()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger1 = NewTrigger();
            var trigger2 = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);
            fsm.AddTransition(NewTransition(stateId1, trigger1, stateId1));
            fsm.AddTransition(NewTransition(stateId1, trigger2, stateId1));
            fsm.AddTransition(NewTransition(stateId2, trigger2, stateId1));

            var transitions = fsm.GetTransitionsWithStateFrom(stateId1);

            Assert.IsTrue(transitions.Any(t => t.StateFrom.Equals(stateId1)));
            Assert.IsFalse(transitions.Any(t => t.StateFrom.Equals(stateId2)));
        }

        [Test]
        public void Return_Transitions_With_Specific_State_To()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger1 = NewTrigger();
            var trigger2 = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);
            fsm.AddTransition(NewTransition(stateId1, trigger1, stateId1));
            fsm.AddTransition(NewTransition(stateId1, trigger2, stateId1));
            fsm.AddTransition(NewTransition(stateId1, trigger2, stateId2));

            var transitions = fsm.GetTransitionsWithStateTo(stateId1);

            Assert.IsTrue(transitions.Any(t => t.StateTo.Equals(stateId1)));
            Assert.IsFalse(transitions.Any(t => t.StateTo.Equals(stateId2)));
        }

        [Test]
        public void Return_Transition_With_Related_State()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();

            var trigger1 = NewTrigger();
            var trigger2 = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);
            fsm.AddTransition(NewTransition(stateId1, trigger1, stateId1));
            fsm.AddTransition(NewTransition(stateId1, trigger2, stateId1));
            fsm.AddTransition(NewTransition(stateId2, trigger1, stateId1));

            var transitions = fsm.GetTransitionsRelatedTo(stateId1);

            Assert.IsTrue(transitions.Any(t => t.StateTo.Equals(stateId1) || t.StateFrom.Equals(stateId1)));
            Assert.IsFalse(transitions.Any(t => t.StateTo.Equals(stateId2) || t.StateFrom.Equals(stateId3)));
        }

        [Test]
        public void Return_If_Has_Transition_With_Specific_State_From()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();

            var trigger1 = NewTrigger();
            var trigger2 = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);
            fsm.AddTransition(NewTransition(stateId1, trigger1, stateId1));
            fsm.AddTransition(NewTransition(stateId1, trigger2, stateId1));
            fsm.AddTransition(NewTransition(stateId2, trigger2, stateId1));

            Assert.IsTrue(fsm.ContainsTransitionWithStateFrom(stateId1));
            Assert.IsFalse(fsm.ContainsTransitionWithStateFrom(stateId3));
        }

        [Test]
        public void Return_If_Has_Transition_With_Specific_State_To()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();

            var trigger1 = NewTrigger();
            var trigger2 = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);
            fsm.AddTransition(NewTransition(stateId1, trigger1, stateId1));
            fsm.AddTransition(NewTransition(stateId1, trigger2, stateId1));
            fsm.AddTransition(NewTransition(stateId2, trigger2, stateId1));

            Assert.IsTrue(fsm.ContainsTransitionWithStateTo(stateId1));
            Assert.IsFalse(fsm.ContainsTransitionWithStateTo(stateId3));
        }

        [Test]
        public void Return_If_Has_Transition_With_Specific_Trigger()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger1 = NewTrigger();
            var trigger2 = NewTrigger();
            var trigger3 = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);
            fsm.AddTransition(NewTransition(stateId1, trigger1, stateId1));
            fsm.AddTransition(NewTransition(stateId1, trigger2, stateId1));
            fsm.AddTransition(NewTransition(stateId2, trigger2, stateId1));

            Assert.IsTrue(fsm.ContainsTransitionWithTrigger(trigger2));
            Assert.IsFalse(fsm.ContainsTransitionWithTrigger(trigger3));
        }

        [Test]
        public void Return_If_Has_Transition_Related_To_Specific_State()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();

            var trigger1 = NewTrigger();
            var trigger2 = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);
            fsm.AddTransition(NewTransition(stateId1, trigger1, stateId1));
            fsm.AddTransition(NewTransition(stateId1, trigger2, stateId1));
            fsm.AddTransition(NewTransition(stateId1, trigger2, stateId2));

            Assert.IsTrue(fsm.ContainsTransitionRelatedTo(stateId1));
            Assert.IsFalse(fsm.ContainsTransitionRelatedTo(stateId3));
        }

        [Test]
        public void Remove_All_Transitions()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddEmpty(stateId1);
            fsm.AddEmpty(stateId2);
            fsm.AddEmpty(stateId3);
            fsm.FromAny(trigger, stateId1);
            fsm.FromAny(trigger, stateId2);
            fsm.FromAny(trigger, stateId3);

            fsm.RemoveAllTransitions();

            Assert.IsTrue(fsm.TransitionCount == 0);
        }

        [Test]
        public void Remove_All_Transitions_Related_To_Specific_State()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddEmpty(stateId1);
            fsm.AddEmpty(stateId2);
            fsm.AddEmpty(stateId3);
            fsm.FromAny(trigger, stateId1);
            fsm.FromAny(trigger, stateId2);
            fsm.FromAny(trigger, stateId3);

            fsm.RemoveAllTransitionsRelatedTo(stateId1);

            Assert.IsFalse(fsm.ContainsTransitionRelatedTo(stateId1));
            Assert.IsTrue(fsm.ContainsTransitionRelatedTo(stateId2));
            Assert.IsTrue(fsm.ContainsTransitionRelatedTo(stateId3));
        }

        [Test]
        public void Return_Corresponding_Value_When_User_Asks_If_Contains_All_States()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();
            var stateId4 = NewStateId();

            fsm.AddEmpty(stateId1);
            fsm.AddEmpty(stateId2);
            fsm.AddEmpty(stateId3);

            Assert.IsTrue(fsm.ContainsAll(stateId1, stateId2, stateId3));
            Assert.IsFalse(fsm.ContainsAll(stateId1, stateId2, stateId3, stateId4));
        }

        [Test]
        public void Add_Multiple_States()
        {
            IState state1 = Substitute.For<IState>();
            IState state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            fsm.AddStates((stateId1, state1), (stateId2, state2));
        }

        [Test]
        public void Throw_An_Exception_If_A_State_Is_Already_Added_When_Adding_Multiple_States_Without_Side_Effects()
        {
            var fsm = NewStateMachine();

            IState state1 = Substitute.For<IState>();
            IState state2 = Substitute.For<IState>();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            fsm.AddState(stateId2, state2);

            Assert.Throws<StateIdAlreadyAddedException>(() => fsm.AddStates((stateId1, state1), (stateId2, state2)));
            Assert.IsFalse(fsm.ContainsState(stateId1));
        }

        [Test]
        public void Add_Multiple_Empty_States()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();
            var stateId4 = NewStateId();

            fsm.AddEmptyStates(stateId1, stateId2, stateId3, stateId4);

            Assert.IsTrue(fsm.ContainsAll(stateId1, stateId2, stateId3, stateId4));
        }

        [Test]
        public void Configure_With_States_As_Triggers_With_No_Reentrant()
        {
            var fsm = NewStateMachine<TState, TState>();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();

            fsm.AddEmptyStates(stateId1, stateId2, stateId3);

            fsm.ConfigureWithStatesAsTriggersWithNoReentrant();

            Assert.IsTrue(
                fsm.ContainsTransition(NewTransition(stateId1, stateId2, stateId2)) &&
                fsm.ContainsTransition(NewTransition(stateId1, stateId3, stateId3)) &&
                fsm.ContainsTransition(NewTransition(stateId2, stateId1, stateId1)) &&
                fsm.ContainsTransition(NewTransition(stateId2, stateId3, stateId3)) &&
                fsm.ContainsTransition(NewTransition(stateId3, stateId1, stateId1)) &&
                fsm.ContainsTransition(NewTransition(stateId3, stateId2, stateId2))
                );
        }

        [Test]
        public void Configure_With_States_As_Triggers()
        {
            var fsm = NewStateMachine<TState, TState>();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();

            fsm.AddEmptyStates(stateId1, stateId2, stateId3);

            fsm.ConfigureWithStatesAsTriggers();

            Assert.IsTrue(
                fsm.ContainsTransition(NewTransition(stateId1, stateId1, stateId1)) &&
                fsm.ContainsTransition(NewTransition(stateId1, stateId2, stateId2)) &&
                fsm.ContainsTransition(NewTransition(stateId1, stateId3, stateId3)) &&
                fsm.ContainsTransition(NewTransition(stateId2, stateId2, stateId2)) &&
                fsm.ContainsTransition(NewTransition(stateId2, stateId1, stateId1)) &&
                fsm.ContainsTransition(NewTransition(stateId2, stateId3, stateId3)) &&
                fsm.ContainsTransition(NewTransition(stateId3, stateId3, stateId3)) &&
                fsm.ContainsTransition(NewTransition(stateId3, stateId1, stateId1)) &&
                fsm.ContainsTransition(NewTransition(stateId3, stateId2, stateId2))
                );
        }

        [Test]
        public void Add_Composite_States()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddComposite(stateId1);

            Assert.That(fsm.GetStateById(stateId1) is CompositeState, "Contains a composite state");
        }

        [Test]
        public void Iterate_Over_Transitions()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();
            var state3 = Substitute.For<IState>();
            var state4 = Substitute.For<IState>();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();
            var stateId4 = NewStateId();

            var trigger1 = NewTrigger();
            var trigger2 = NewTrigger();

            var transition1 = NewTransition(stateId1, trigger1, stateId2);
            var transition2 = NewTransition(stateId3, trigger2, stateId4);

            var fsm = NewStateMachine();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);

            fsm.AddState(stateId3, state3);
            fsm.AddState(stateId4, state4);

            fsm.AddTransition(transition1);
            fsm.AddTransition(transition2);

            Transition<TState, TTrigger> item1 = default;
            Transition<TState, TTrigger> item2 = default;

            int cont = 1;

            fsm.ForeachTransition(
                transition =>
                {
                    if (cont == 1)
                    {
                        item1 = transition;
                    }
                    else
                    {
                        item2 = transition;
                    }

                    cont++;

                    return false;
                }
                );

            Assert.IsTrue(item1.Equals(transition1) && item2.Equals(transition2));
        }

        [Test]
        public void Iterate_Over_States()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            IState item1 = null;
            IState item2 = null;

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);

            int cont = 1;

            fsm.ForeachState(
                state =>
                {
                    if (cont == 1)
                    {
                        item1 = fsm.GetStateById(state);
                    }
                    else
                    {
                        item2 = fsm.GetStateById(state);
                    }

                    cont++;

                    return false;
                }
                );

            Assert.IsTrue(fsm.GetStateById(stateId1) == item1 && fsm.GetStateById(stateId2) == item2);
        }

        [Test]
        public void Add_Behavioural_States()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var behaviouralState1 = fsm.AddWithBehaviours(stateId1);

            Assert.IsTrue(fsm.ContainsState(stateId1));
            Assert.IsTrue(fsm.GetStateById(stateId1) == behaviouralState1);

            IStateBehaviour stateBehaviour1 = Substitute.For<IStateBehaviour>();
            IStateBehaviour stateBehaviour2 = Substitute.For<IStateBehaviour>();

            var behaviouralState2 = fsm.AddWithBehaviours(stateId2, stateBehaviour1, stateBehaviour2);

            Assert.IsTrue(fsm.ContainsState(stateId2));
            Assert.IsTrue(fsm.GetStateById(stateId2) == behaviouralState2);
            Assert.IsTrue(fsm.ContainsBehaviour(stateBehaviour1));
            Assert.IsTrue(fsm.ContainsBehaviour(stateBehaviour2));
            Assert.IsTrue(fsm.BehaviourCount() == 2);
            Assert.IsTrue(fsm.BehaviourCountOf(stateId2) == 2);
        }

        [Test]
        public void Add_Behaviour_To_Behavioural_States()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            var stateBehaviour1 = Substitute.For<IStateBehaviour>();

            fsm.AddWithBehaviours(stateId1);

            fsm.AddBehaviourTo(stateId1, stateBehaviour1);

            Assert.IsTrue(fsm.ContainsBehaviour(stateBehaviour1));
        }

        [Test]
        public void Add_Multiple_Behaviours_To_Behavioural_States()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            var stateBehaviour1 = Substitute.For<IStateBehaviour>();
            var stateBehaviour2 = Substitute.For<IStateBehaviour>();

            fsm.AddWithBehaviours(stateId1);

            fsm.AddBehavioursTo(stateId1, stateBehaviour1, stateBehaviour2);

            Assert.IsTrue(fsm.ContainsBehaviour(stateBehaviour1));
            Assert.IsTrue(fsm.ContainsBehaviour(stateBehaviour2));
        }

        [Test]
        public void Do_Nothing_If_User_Tries_To_Add_The_Same_Behaviour_On_The_Same_State_Twice()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            var stateBehaviour1 = Substitute.For<IStateBehaviour>();

            fsm.AddWithBehaviours(stateId1);

            fsm.AddBehaviourTo(stateId1, stateBehaviour1);
            Assert.DoesNotThrow(() => fsm.AddBehaviourTo(stateId1, stateBehaviour1));
            Assert.IsTrue(fsm.BehaviourCount() == 1);
        }

        [Test]
        public void Remove_Behaviours_From_Behavioural_States()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            var stateBehaviour1 = Substitute.For<IStateBehaviour>();
            var stateBehaviour2 = Substitute.For<IStateBehaviour>();

            fsm.AddWithBehaviours(stateId1, stateBehaviour1, stateBehaviour2);

            fsm.RemoveBehaviourFrom(stateId1, stateBehaviour1);

            Assert.IsTrue(fsm.BehaviourCount() == 1);
            Assert.IsTrue(fsm.BehaviourCountOf(stateId1) == 1);
            Assert.IsFalse(fsm.ContainsBehaviour(stateBehaviour1));
            Assert.IsFalse(fsm.ContainsBehaviourOn(stateId1, stateBehaviour1));
            Assert.IsTrue(fsm.ContainsBehaviour(stateBehaviour2));
            Assert.IsTrue(fsm.ContainsBehaviourOn(stateId1, stateBehaviour2));

            fsm.RemoveBehaviourFrom(stateId1, stateBehaviour2);

            Assert.IsTrue(fsm.BehaviourCount() == 0);
            Assert.IsTrue(fsm.BehaviourCountOf(stateId1) == 0);
            Assert.IsFalse(fsm.ContainsBehaviour(stateBehaviour2));
            Assert.IsFalse(fsm.ContainsBehaviourOn(stateId1, stateBehaviour2));
        }

        [Test]
        public void Return_Behaviour_Of_Specific_Type_From_Any_State()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            var stateBehaviour = new TestStateBehaviour();

            fsm.AddWithBehaviours(stateId1, stateBehaviour);

            Assert.AreEqual(stateBehaviour, fsm.GetBehaviour<TestStateBehaviour, TState, TTrigger>());
        }

        [Test]
        public void Return_Behaviour_Of_Specific_Type_From_Specific_State()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            var stateBehaviour = new TestStateBehaviour();

            fsm.AddWithBehaviours(stateId1, stateBehaviour);

            Assert.AreEqual(stateBehaviour, fsm.GetBehaviourOf<TestStateBehaviour, TState, TTrigger>(stateId1));
        }

        [Test]
        public void Return_Behaviours_Of_Specific_Type_From_All_Behavioural_States()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            var stateBehaviour1 = new TestStateBehaviour();
            var stateBehaviour2 = new TestStateBehaviour();
            var stateBehaviour3 = Substitute.For<IStateBehaviour>();

            fsm.AddWithBehaviours(stateId1, stateBehaviour1, stateBehaviour2, stateBehaviour3);

            var behaviors = fsm.GetBehaviours<TestStateBehaviour, TState, TTrigger>();

            Assert.IsTrue(behaviors.Length == 2);
            Assert.IsTrue(behaviors.Contains(stateBehaviour1));
            Assert.IsTrue(behaviors.Contains(stateBehaviour2));
        }

        [Test]
        public void Return_Behaviours_Of_Specific_Type_From_Specific_Behavioural_State()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            var stateBehaviour1 = new TestStateBehaviour();
            var stateBehaviour2 = new TestStateBehaviour();
            var stateBehaviour3 = Substitute.For<IStateBehaviour>();

            fsm.AddWithBehaviours(stateId1, stateBehaviour1, stateBehaviour2, stateBehaviour3);

            var behaviors = fsm.GetBehavioursOf<TestStateBehaviour, TState, TTrigger>(stateId1);

            Assert.IsTrue(behaviors.Length == 2);
            Assert.IsTrue(behaviors.Contains(stateBehaviour1));
            Assert.IsTrue(behaviors.Contains(stateBehaviour2));
        }

        [Test]
        public void Iterate_Over_All_Behaviours()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            var stateBehaviour1 = Substitute.For<IStateBehaviour>();
            var stateBehaviour2 = Substitute.For<IStateBehaviour>();

            fsm.AddWithBehaviours(stateId1, stateBehaviour1, stateBehaviour2);

            var behaviours = new List<IStateBehaviour>();

            behaviours.Add(stateBehaviour1);
            behaviours.Add(stateBehaviour2);

            fsm.ForeachBehaviour(behaviour =>
            {
                behaviours.Remove(behaviour);
                return false;
            });

            Assert.IsTrue(behaviours.Count == 0);
        }

        [Test]
        public void Iterate_Over_All_Behaviours_Of_Specific_State()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            var stateBehaviour1 = Substitute.For<IStateBehaviour>();
            var stateBehaviour2 = Substitute.For<IStateBehaviour>();

            fsm.AddWithBehaviours(stateId1, stateBehaviour1, stateBehaviour2);

            var behaviours = new List<IStateBehaviour>();

            behaviours.Add(stateBehaviour1);
            behaviours.Add(stateBehaviour2);

            fsm.ForeachBehaviourOn(stateId1, behaviour =>
            {
                Assert.IsFalse(behaviours.Count == 0);
                behaviours.Remove(behaviour);
                return false;
            });

            Assert.IsTrue(behaviours.Count == 0);
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Add_A_Null_Behaviour()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            var stateBehaviour1 = Substitute.For<IStateBehaviour>();

            Assert.Throws<ArgumentNullException>(() => fsm.AddWithBehaviours(stateId1, stateBehaviour1, null));

            fsm.AddWithBehaviours(stateId1);

            Assert.Throws<ArgumentNullException>(() => fsm.AddBehaviourTo(stateId1, null));
            Assert.Throws<ArgumentNullException>(() => fsm.AddBehavioursTo(stateId1, stateBehaviour1, null));
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Remove_A_Null_Behaviour()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            Assert.Throws<ArgumentNullException>(() => fsm.RemoveBehaviourFrom(stateId1, null));
        }

        [Test]
        public void Throw_An_Exception_If_User_Asks_If_Contains_Behaviour_With_A_Null_Behaviour()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            Assert.Throws<ArgumentNullException>(() => fsm.ContainsBehaviour(null));
            Assert.Throws<ArgumentNullException>(() => fsm.ContainsBehaviourOn(stateId1, null));
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Iterate_Over_Behaviours_With_A_Null_Delegate()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            Assert.Throws<ArgumentNullException>(() => fsm.ForeachBehaviour(null));
            Assert.Throws<ArgumentNullException>(() => fsm.ForeachBehaviourOn(stateId1, null));
        }
    }
}



