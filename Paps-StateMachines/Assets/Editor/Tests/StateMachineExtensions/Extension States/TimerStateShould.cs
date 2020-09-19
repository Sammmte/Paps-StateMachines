using NUnit.Framework;
using Paps.StateMachines.Extensions;
using System;
using System.Threading;

namespace Tests.StateMachineExtensions.ExtensionStates
{
    public class TimerStateShould
    {
        [Test]
        public void Execute_On_Timer_Elapsed()
        {
            bool executed = false;
            var timerState = new TimerState(TimeSpan.FromMilliseconds(1000), () => executed = true);

            Thread.Sleep(1100);

            timerState.Update();

            Assert.That(executed, "On Timer Elapsed delegate has been executed");
        }
    }
}