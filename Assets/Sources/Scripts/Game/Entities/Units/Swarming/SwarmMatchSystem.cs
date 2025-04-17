using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Potman.Game.Entities.Units.Abstractions.Swarming;
using R3;
using UnityEngine;

namespace Potman.Game.Entities.Units.Swarming
{
    public class SwarmMatchSystem : ISwarmMatchSystem
    {
        private readonly ISwarmMasterFactory masterFactory;
        private readonly List<ISwarmAgent> freeAgents = new();
        private IDisposable update;
        public float MatchDistance { get; } = 25;

        public int MaxGroupSize { get; set; } = 25;

        public SwarmMatchSystem(ISwarmMasterFactory masterFactory)
        {
            this.masterFactory = masterFactory;
        }

        public void Start(ISwarmAgent agent)
        {
            if (agent is not {Enabled: true}
                || agent.Master?.Count >= MaxGroupSize
                || agent.MatchType <= 0
                || freeAgents.Contains(agent))
                return;

            freeAgents.Add(agent);

            if (update != null)
                return;

            update = Observable.Interval(TimeSpan.FromSeconds(1/60f*10), CancellationToken.None).Subscribe(_ => Match());
        }

        public void Stop(ISwarmAgent agent)
        {
            freeAgents.Remove(agent);
            StopUpdate();
        }

        private void Match()
        {
            if (freeAgents.Count < 2)
                return;

            var agents = freeAgents.ToArray();
            for (var i = 0; i < agents.Length; i++)
            {
                var agent = agents[i];
                CollectAgents(agent, agents);

                if (agent.Master?.Agent == agent && agent.Master.Count >= MaxGroupSize)
                    Stop(agent);
            }
        }

        public void Dispose()
        {
            freeAgents.Clear();
        }

        private void StopUpdate()
        {
            if (freeAgents.Count >= 2)
                return;

            update?.Dispose();
            update = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CollectAgents(ISwarmAgent agent, ISwarmAgent[] others)
        {
            var master = agent.Master;
            var mPos = agent.Position;
            var matchType = agent.MatchType;
            // var coefRange = (agent.MatchCoef / 100f) * 10f; 
            // var coefMax = agent.MatchCoef + coefRange;
            // var coefMin = agent.MatchCoef - coefRange;
            for (var i = 0; i < others.Length; i++)
            {
                var other = others[i];
                if (agent == other
                    || matchType == other.MatchType
                    || (other.Master != null && (master == null || other.Master.Count > master.Count))
                    || !freeAgents.Contains(other)
                    || !InRadiusXZ(mPos, other.Position)
                    // || !InRange(other.MatchCoef, coefMin, coefMax)
                    )
                    continue;

                if (master == null)
                {
                    master = masterFactory.Create();
                    master.Join(agent, true);
                }

                if (master.Join(other))
                    Stop(other);

                if (master.Count >= MaxGroupSize)
                    return;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool InRadiusXZ(Vector3 pointA, Vector3 pointB)
            => Vector3.Distance(pointA, pointB) <= MatchDistance;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool InRange(float a, float min, float max)
            => a >= min && a <= max;
    }
}