using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Potman.Common.Pools.Abstractions;
using Potman.Game.Entities.Units.Abstractions.Swarming;
using UnityEngine;

namespace Potman.Game.Entities.Units.Swarming
{ 
    public class SwarmMaster : ISwarmMaster
    {
        private readonly IPool<ISwarmMaster> masterPool;
        private readonly List<ISwarmAgent> agents;
        private static int msterIndex;

        public int Id { get; } = msterIndex += 1;

        #region Properties

        public bool Available { get; private set; }

        public int Count => agents.Count;

        public ISwarmAgent Agent { get; private set; }

        public ISwarmAgent[] Agents => agents.ToArray();

        #endregion

        public SwarmMaster(IPool<ISwarmMaster> masterPool)
        {
            this.masterPool = masterPool;
            agents = new List<ISwarmAgent>();
        }

        public bool Join(ISwarmAgent agent, bool asMaster = false, bool notify = true)
        {
            if (!Available || agent is not {Enabled:true} || agent == Agent)
                return false;

            if (agent.Master is {Available:true} && agent.Master != this)
                return MigrateInternal(agent);
                
            JoinInternal(agent, asMaster, notify);
            return true;
        }

        public void Disconnect(ISwarmAgent agent, bool notify = true)
        {
            if (!Available || agent == null)
                return;

            if (agent.Master != this)
            {
                agent.Master?.Disconnect(agent, notify);
                return;
            }

            agents.Remove(agent);
            agent.SetMaster(null, Agent, notify);
                
            if (notify && agent == Agent)
                NotifyMasterChanged(agents.FirstOrDefault(x => x.Enabled));

            TryRelease();
        }

        public void Release()
        {
            if (agents.Count > 0)
                Debug.LogError($"Released with agents : {agents.Count}");
            
            Available = false;
            agents.Clear();
            Agent = null;
        }

        public void Spawn()
        {
            Available = true;
        }

        public void Dispose()
        {
            agents.Clear();
            Available = false;
            Agent = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void NotifyMasterChanged(ISwarmAgent current, bool notify = true)
        {
            var previous = Agent;
            Agent = current;
            
            foreach (var swarmAgent in Agents)
                swarmAgent.SetMaster(this, previous, notify);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void JoinInternal(ISwarmAgent agent, bool asMaster, bool notify = true)
        {
            if (!agents.Contains(agent)) 
                agents.Add(agent);

            if (asMaster || Agent == null)
            {
                NotifyMasterChanged(agent, notify);
                return;
            }
                
            agent.SetMaster(this, Agent, notify);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool MigrateInternal(ISwarmAgent agent)
        {
            var otherMaster = agent.Master;
            foreach (var migrateAgent in otherMaster.Agents)
            {
                otherMaster.Disconnect(migrateAgent, false);
                JoinInternal(migrateAgent, false);
            }
            
            return true;
        }

        private void TryRelease()
        {
            if (Available && (Count == 0 || Agent == null))
            {
                Debug.LogWarning("MasterPool Released a master");
                masterPool.Release(this);
            }
        }
    }
}