using NSubstitute;
using NUnit.Framework;
using Paps.StateMachines;
using Paps.StateMachines.Extensions;

namespace Tests
{
    public class CompositeStateShould
    {
        [Test]
        public void Execute_Corresponding_Inner_States_Methods_In_Its_Own_Methods()
        {
            var state = Substitute.For<IState>();

            var compositeState = new CompositeState(state);

            compositeState.Enter();
            compositeState.Update();
            compositeState.Exit();

            state.Received(1).Enter();
            state.Received(1).Update();
            state.Received(1).Exit();
        }
    }
}