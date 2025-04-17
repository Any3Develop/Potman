using System.Collections.Generic;
using Potman.Game.Context.Abstractions;
using Potman.Game.Context.Data.Spawn;
using UnityEngine;

namespace Potman.Game.Context.Spawn
{
    public class PositionProvider : IPositionProvider
    {
        private readonly Dictionary<string, int> iterations = new();

        public void ResetAll() => iterations.Clear();

        public void Reset(string groupId) => iterations.Remove(groupId);

        public T Apply<T>(string groupId, int index, IList<T> ids, FunctionSelector selector)
        {
            switch (selector)
            {
                case FunctionSelector.Index: return ids[Mathf.Clamp(index, 0, ids.Count - 1)];

                case FunctionSelector.RoundRobin:
                    var iter = GetIterations();
                    iterations[groupId] = (iter + 1) % ids.Count;
                    return ids[iter];

                case FunctionSelector.ReverseRoundRobin:
                    var reverseIter = GetIterations();
                    iterations[groupId] = (reverseIter - 1 + ids.Count) % ids.Count;
                    return ids[reverseIter];

                case FunctionSelector.PingPong:
                    var pingPongIter = GetIterations();
                    var cycleLength = 2 * ids.Count - 2;
                    var cycleIndex = pingPongIter % cycleLength;

                    var adjustedIndex = cycleIndex < ids.Count 
                        ? cycleIndex
                        : 2 * ids.Count - 2 - cycleIndex;

                    iterations[groupId] = pingPongIter + 1;
                    return ids[adjustedIndex];

                case FunctionSelector.Random:
                default: return ids[Random.Range(0, ids.Count)];
            }

            int GetIterations(int defaulValue = 0)
            {
                if (!iterations.TryGetValue(groupId, out var iteration))
                    iterations[groupId] = iteration = defaulValue;

                return iteration;
            }
        }
    }
}