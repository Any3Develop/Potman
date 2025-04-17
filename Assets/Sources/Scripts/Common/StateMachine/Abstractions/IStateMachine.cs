using System;
using System.Collections.Generic;

namespace Potman.Common.StateMachine.Abstractions
{
    public interface IStateMachine
    {
        event Action<string> OnSwitchState;
        
        string Id { get; }
        int Count { get; }
        
        IState Current { get; }
        
        IState Previous { get; }

        bool Contains(string id);
        
        void SetId(string id);
        
        void Add(IState state);
        
        void Remove(string stateId);
        
        void Switch(string stateId, params object[] args);

        IEnumerable<IState> GetStates(bool includeDefault = false);
        IState GetState(string stateId);
        IState GetState(int index);

        bool Any();
        
        void Clear();
    }
}