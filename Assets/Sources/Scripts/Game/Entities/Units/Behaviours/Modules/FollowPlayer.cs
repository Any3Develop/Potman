using System;
using Potman.Game.Entities.Units.Abstractions;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Potman.Game.Entities.Units.Behaviours.Modules
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "FollowPlayer", story: "Agent starts to follow the player", category: "Action", id: "d7649fb847bd7860b6a286da5aa190d9")]
    public partial class FollowPlayer : Action
    {
        private IUnitEntity unit;
        
        protected override Status OnStart()
        {
            unit ??= GameObject.GetComponent<IUnitEntity>();
            if (!unit.GameContext.Player.IsAlive)
                return Status.Success;
            
            unit.Movement.SetSteering(unit.GameContext.Player.Mapper.Map<Transform>("Root"));
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (!unit.GameContext.Player.IsAlive)
            {
                unit.Movement.Stop();
                return Status.Success;
            }
            
            if (unit.Movement.RemainingDistance > 2f)
                return Status.Running;

            unit.Movement.Stop();
            return Status.Success;
        }
    }
}

