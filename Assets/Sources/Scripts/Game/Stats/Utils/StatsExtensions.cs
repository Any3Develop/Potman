using System;
using System.Collections.Generic;
using System.Linq;
using Potman.Game.Stats.Abstractions;
using Potman.Game.Stats.Data;
using R3;
using UnityEngine;

namespace Potman.Game.Stats.Utils
{
    public static class StatsExtensions
    {
        private class Listener : IDisposable
        {
            private Action<IRuntimeStat> value;
            private IRuntimeStat stat;

            public Listener(Action<IRuntimeStat> value, IRuntimeStat stat)
            {
                this.value = value;
                this.stat = stat;
            }
            
            public void Dispose()
            {
                if (stat != null && value != null)
                    stat.OnChanged -= value;

                stat = null;
                value = null;
            }
        }
        
        public static IDisposable SubscribeStat(
            this IStatsCollection statsCollection, 
            StatType statType, 
            Action<IRuntimeStat> onChanged,
            bool forceInit = true)
        {
            if (!statsCollection.TryGet(statType, out var stat))
            {
                Debug.LogWarning($"{nameof(SubscribeStat)} : Stat with type : {statType} doesn't exist.");
                return Disposable.Empty;
            }

            stat.OnChanged += onChanged;
            if (forceInit)
                onChanged?.Invoke(stat);
            return new Listener(onChanged, stat);
        }
        
        /// <summary>
        /// Finds an existing stat and applies merge to it. If the stat doesn't exist, a new one will be created.
        /// </summary>
        /// <param name="source">a collection to be merged into.</param>
        /// <param name="datas">other stat data to merge or create them in a collection.</param>
        public static IStatsCollection Merge(this IStatsCollection source, IEnumerable<StatData> datas)
        {
            foreach (var statData in datas)
            {
                if (source.TryGet(statData.type, out var stat))
                {
                    if (statData.useMax)
                        stat.SetMax(Mathf.Max(stat.Max, statData.max));
                    
                    if (statData.useMin)
                        stat.SetMin(Mathf.Min(stat.Min, statData.min));
                    
                    stat.Add(statData.value);
                    continue;
                }
                
                source.AddNew(statData);
            }

            return source;
        }

#if UNITY_EDITOR
        public static void OnValidateStats(this List<StatData> stats)
        {
            if (stats is {Count: > 0})
            {
                for (var i = 0; i < stats.Count; i++)
                {
                    var data = stats[i];
                    data.name = data.type.ToString();
                    stats[i] = data;
                }
            }

            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                stats.ToDictionary(key => key.type, _ => default(bool));
            }
            catch
            {
                Debug.LogError($"There are duplicates of {nameof(StatType)} in the {nameof(stats)} list. This will lead to unexpected behavior.");
            }
        }
#endif
    }
}