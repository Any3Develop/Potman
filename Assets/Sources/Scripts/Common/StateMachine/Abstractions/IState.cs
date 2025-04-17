using System.Collections.Generic;

namespace Potman.Common.StateMachine.Abstractions
{
    public interface IState
    {
	    int Order { get; set; }
	    string Id { get; }
	    IState ParentState { get; }
	    void OnEnter(params object[] args);
        void OnExit();
        IEnumerable<IState> GetAllStates();
    }
}