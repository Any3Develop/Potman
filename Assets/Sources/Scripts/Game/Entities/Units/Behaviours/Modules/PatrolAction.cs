using System;
using Potman.Game.Context.Abstractions;
using Potman.Game.Context.Spawn;
using Potman.Game.Entities.Units.Abstractions;
using Potman.Game.Stats.Data;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Random = UnityEngine.Random;

namespace Potman.Game.Entities.Units.Behaviours.Modules
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Patrol", story: "Agent patrol until player died", category: "Action", id: "69a991a4be6c133fb83cd4f7bc26f2d3")]
    public partial class PatrolAction : Action
    {
        private IUnitEntity unit;
        private SpawnPoint[] spawnPoints;
        private float nextTimeMove;
        
        protected override Status OnStart()
        {
            unit ??= GameObject.GetComponent<IUnitEntity>();
            spawnPoints = unit.GameContext.ServiceProvider.GetRequiredService<ISpawnPointProvider>().GetAll();
            return base.OnStart();
        }

        protected override Status OnUpdate()
        {
            if (nextTimeMove <= Time.time)
                MoveNext();
            
            return unit.GameContext.Player.IsAlive ? Status.Success : Status.Running;
        }

        private void MoveNext()
        {
            var waitTime = Random.value * 60;
            var p1 = spawnPoints[Mathf.FloorToInt(Random.value * spawnPoints.Length)].Position;
            var p2 = unit.Movement.Position;
            var transitionTime = Vector3.Distance(p1, p2) / (unit.StatsCollection.Get(StatType.MoveSpeed).Current);
            
            nextTimeMove = Time.time + waitTime + transitionTime;
            unit.Movement.MoveAuto(Vector3.Lerp(p1, p2, Random.value));
        }
    }
}

