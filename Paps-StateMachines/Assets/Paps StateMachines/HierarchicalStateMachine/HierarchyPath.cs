using System;
using System.Collections;
using System.Collections.Generic;

namespace Paps.StateMachines
{
    public class HierarchyPath<TState> : IEnumerable<KeyValuePair<TState, IState>>
    {
        private List<KeyValuePair<TState, IState>> _hierarchyPath = new List<KeyValuePair<TState, IState>>();
        private readonly IEqualityComparer<TState> _stateComparer;

        public HierarchyPath(IEnumerable<KeyValuePair<TState, IState>> hierarchyPath, IEqualityComparer<TState> stateComparer)
        {
            if (stateComparer == null)
                throw new ArgumentNullException(nameof(stateComparer));

            _hierarchyPath.AddRange(hierarchyPath);
            _stateComparer = stateComparer;
        }

        public HierarchyPath(IEnumerable<KeyValuePair<TState, IState>> hierarchyPath) : this(hierarchyPath, EqualityComparer<TState>.Default)
        {

        }

        public bool Contains(TState stateId)
        {
            for (int i = 0; i < _hierarchyPath.Count; i++)
            {
                if (_stateComparer.Equals(_hierarchyPath[i].Key, stateId))
                    return true;
            }

            return false;
        }

        public IState this[TState stateId]
        {
            get
            {
                for(int i = 0; i < _hierarchyPath.Count; i++)
                {
                    if (_stateComparer.Equals(_hierarchyPath[i].Key, stateId))
                        return _hierarchyPath[i].Value;
                }

                throw new KeyNotFoundException();
            }
        }

        public IEnumerator<KeyValuePair<TState, IState>> GetEnumerator()
        {
            return _hierarchyPath.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
