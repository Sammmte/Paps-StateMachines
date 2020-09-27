using Paps.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Paps.StateMachines
{
    internal class PlainStateCollection<TState, TTrigger>
    {
        public Maybe<TState> InitialState { get; private set; }
        public int StateCount => _states.Count;

        private readonly Dictionary<TState, IState> _states = new Dictionary<TState, IState>();
        private readonly List<TState> _protectedStates = new List<TState>();
        private readonly IPlainStateMachine<TState, TTrigger> _stateMachine;
        private readonly IEqualityComparer<TState> _stateComparer;

        private bool _isAddLocked;
        private bool _isRemoveLocked;

        public PlainStateCollection(IPlainStateMachine<TState, TTrigger> stateMachine, IEqualityComparer<TState> stateComparer)
        {
            _stateMachine = stateMachine;
            _stateComparer = stateComparer;
        }

        public void AddState(TState stateId, IState state)
        {
            ValidateCanAddState(stateId, state);

            _states.Add(stateId, state);

            if (_states.Count == 1)
                SetInitialState(stateId);
        }

        private void ValidateCanAddState(TState stateId, IState state)
        {
            if(_isAddLocked)
            {
                throw new UnableToAddStateMachineElementException(_stateMachine, stateId);
            }
            else if (_states.ContainsKey(stateId))
            {
                throw new StateIdAlreadyAddedException(_stateMachine, stateId.ToString());
            }
        }

        public void SetInitialState(TState stateId)
        {
            ValidateHasStateWithId(stateId);

            InitialState = stateId.ToMaybe();
        }

        private void ValidateHasStateWithId(TState stateId)
        {
            if (ContainsState(stateId) == false)
            {
                throw new StateIdNotAddedException(_stateMachine, stateId);
            }
        }

        public bool ContainsState(TState stateId)
        {
            return _states.ContainsKey(stateId);
        }

        public bool RemoveState(TState stateId)
        {
            ValidateCanRemoveState(stateId);

            if(_states.Remove(stateId))
            {
                if (InitialState.HasValue && AreEquals(InitialState.Value, stateId))
                    InitialState = Maybe<TState>.Nothing;

                return true;
            }

            return false;
        }

        private void ValidateCanRemoveState(TState stateId)
        {
            if (_isRemoveLocked)
                throw new UnableToRemoveStateMachineElementException(_stateMachine, stateId);
            else if (IsProtected(stateId))
                throw new ProtectedStateException(_stateMachine, stateId);
        }

        public IState GetStateObjectById(TState stateId)
        {
            ValidateHasStateWithId(stateId);

            return _states[stateId];
        }

        public TState[] GetStates()
        {
            return _states.Keys.ToArray();
        }

        private bool AreEquals(TState stateId1, TState stateId2)
        {
            return _stateComparer.Equals(stateId1, stateId2);
        }

        public void ProtectState(TState stateId)
        {
            if(!IsProtected(stateId))
                _protectedStates.Add(stateId);
        }

        public void UnprotectState(TState stateId)
        {
            _protectedStates.Remove(stateId);
        }

        private bool IsProtected(TState stateId)
        {
            return _protectedStates.Contains(stateId);
        }

        public void LockRemove()
        {
            _isRemoveLocked = true;
        }

        public void UnlockRemove()
        {
            _isRemoveLocked = false;
        }

        public void Lock()
        {
            _isAddLocked = true;
            _isRemoveLocked = true;
        }

        public void Unlock()
        {
            _isAddLocked = false;
            _isRemoveLocked = false;
        }
    }
}