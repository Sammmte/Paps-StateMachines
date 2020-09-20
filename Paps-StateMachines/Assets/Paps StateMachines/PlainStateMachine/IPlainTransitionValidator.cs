namespace Paps.StateMachines
{
    internal interface IPlainTransitionValidator<TState, TTrigger>
    {
        bool IsValid(Transition<TState, TTrigger> transition);
    }
}