using System.Collections.Generic;
using Potman.Common.StateMachine.Abstractions;

namespace Potman.Common.StateMachine
{
    public abstract class State : IState
    {
        public int Order { get; set; }
        public virtual string Id { get; }
        public IState ParentState { get; }

        protected State(string id, IState parent = null) : this(id)
        {
            ParentState = parent;
        }

        protected State(string id)
        {
            Id = id;
        }

        public abstract void OnEnter(params object[] args);

        public virtual void OnExit() {}

        public virtual IEnumerable<IState> GetAllStates()
        {
            return new List<IState> {this};
        }
    }
}