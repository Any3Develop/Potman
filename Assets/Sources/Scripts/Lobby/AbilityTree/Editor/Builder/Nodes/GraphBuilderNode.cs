using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Potman.AbilityTree.Upgrades;
using Potman.Lobby.AbilityTree.Data;
using Potman.Lobby.AbilityTree.Editor.Graphs;
using UnityEngine;
using XNode;

namespace Potman.AbilityTree.Builder
{
    [NodeWidth(375)]
    public class GraphBuilderNode : Node
    {
        public AbilityTreeGraph[] graphs;

        [ContextMenu("Build")]
        public void Build()
        {
            var path = Path.Combine(Application.dataPath, "Sources/Resources/Lobby/AbilityTree");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects
            };

            foreach (var treeGraph in graphs)
            {
                if (treeGraph == null)
                {
                    Debug.LogError($"{name} has an empty graph reference.");
                    continue;
                }

                var nodes = treeGraph.nodes?.OfType<PerkNode>().ToList() ?? new List<PerkNode>();

                if (nodes.Count == 0)
                {
                    Debug.LogError($"{treeGraph.name} doesn't have any nodes.");
                    continue;
                }

                var graphNodes = nodes.Select(node =>
                {
                    var dataNode = new AbilityNode
                    {
                        Id = node.Id,
                        GraphId = node.GraphId,
                        Costs = node.Costs,
                        Depends = node.GetInputPort(nameof(node.Optional))
                            .GetInputValues<PerkNodeBase>()
                            .Select(x => new AbilityLinkNode{Id = x.Id, GraphId = x.GraphId})
                            .Concat(node.GetInputPort(nameof(node.Required))
                                .GetInputValues<PerkNodeBase>()
                                .Select(x => new AbilityLinkNode{Id = x.Id, GraphId = x.GraphId, IsRequired = true}))
                            .ToArray(),
                        Upgrades = node.GetInputPort(nameof(node.Upgrades)).GetInputValues<UpgradeNode>().SelectMany(x => x.Upgrades).ToArray(),
                    };

                    if (node.Icon)
                        dataNode.Icon = node.Icon.name;
                    else
                        Debug.LogError($"Missing sprite for : {treeGraph.name}/{node.name}.{nameof(node.Icon)}");

                    return dataNode;
                }).ToArray();

                var fileName = $"{treeGraph.name}.json";
                var jsonContent = JsonConvert.SerializeObject(graphNodes, settings);
                File.WriteAllText(Path.Combine(path, fileName), jsonContent);
                Debug.Log($"Successfully Saved {fileName} to {path}");
            }
        }
    }
}