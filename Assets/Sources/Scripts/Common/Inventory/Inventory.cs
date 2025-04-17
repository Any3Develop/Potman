using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Potman.Common.Inventory.Abstractions;
using Potman.Common.SerializeService.Abstractions;
using Potman.Common.Utilities;
using Potman.Lobby.Identity;
using UnityEngine;

namespace Potman.Common.Inventory
{
    public class Inventory : IInventory
    {
        private readonly JsonSerializerSettings settings;
        private readonly ISerializeService serializeService;
        private bool loaded;

        public IInventoryRepository Repository { get; }

        public Inventory(
            IInventoryRepository inventoryRepository,
            ISerializeService serializeService)
        {
            this.serializeService = serializeService;
            settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects
            };
            Repository = inventoryRepository;
        }

        public UniTask LoadAsync()
        {
            if (loaded || !serializeService.TryGet(GetUserPath(), out var jsonContent))
                return UniTask.CompletedTask;

            try
            {
                var progressItems = JsonConvert.DeserializeObject<IInventoryItem[]>(jsonContent, settings);
                progressItems.Each(Repository.Add);
                loaded = true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                loaded = false;
            }

            return UniTask.CompletedTask;
        }

        public UniTask SaveAsync()
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(Repository.GetAll(), settings);
                serializeService.Patch(GetUserPath(), jsonContent);
                serializeService.Save();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return UniTask.CompletedTask;
        }

        private static string GetUserPath() => Path.Combine(UserIdentity.GetUserPath(), "Inventory");
    }
}