using System;
using UnityEngine;

namespace Potman.Game.Entities.Units.Abstractions.Swarming
{
    public interface ISwarmAgent
    {
        event Action OnMasterChanged;
        event Action<IUnitPath> OnPathChanged;
        
        int Id { get; }
        int MatchType { get; }
        float MatchCoef { get; }
        bool Enabled { get; }
        bool IsMaster { get; }
        Vector3 Position { get; }
        ISwarmMaster Master { get; }
        IUnitPath UnitPath { get; }

        void Enable(bool value);
        void SetPath(IUnitPath unitPath);
        void SetMaster(ISwarmMaster value, ISwarmAgent previous, bool notify = true);
    }
}