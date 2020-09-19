using NUnit.Framework;
using Paps.StateMachines.Extensions;

namespace Tests
{
    public class DelegateStateShould
    {
        [Test]
        public void Execute_Its_Delegates_On_Corresponding_Methods()
        {
            bool executedEnter = false;
            bool executedUpdate = false;
            bool executedExit = false;

            var delegateState = new DelegateState(
                () => executedEnter = true, () => executedUpdate = true, () => executedExit = true);

            delegateState.Enter();
            delegateState.Update();
            delegateState.Exit();

            Assert.That(executedEnter && executedUpdate && executedExit, "All methods have been executed");
        }
    }
}