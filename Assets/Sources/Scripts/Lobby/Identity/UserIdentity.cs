using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Potman.Common.Events;
using Potman.Common.SerializeService.Abstractions;
using R3;

namespace Potman.Lobby.Identity
{
    public class UserSaveProgressEvent
    {
        public readonly HashSet<UniTask> Tasks = new();
    }
    public class UserLoadProgressEvent
    {
        public readonly HashSet<UniTask> Tasks = new();
    }
    public readonly struct UserNameChangedEvent{}
    public readonly struct UserCreatedEvent{}
    
    public class UserIdentity:  IDisposable
    {
        private const string UserKey = "User";
        private const string LastUserKey = "LastUser";
        private const string DefaultName = "Unnamed";
        private readonly ISerializeService serializeService;
        private IDisposable subscriptions;
        private static UserModel current;

        public static RedirectionCollection Redirections { get; private set; }
        public static string Id => current?.Id ?? UserKey;
        public static string Name => current?.Name ?? DefaultName;

        public UserIdentity(ISerializeService serializeService)
        {
            this.serializeService = serializeService;
            Redirections = new RedirectionCollection();
            using var builder = new DisposableBuilder();
            
            builder.Add(MessageBroker.Receive<UserSaveProgressEvent>()
                .Subscribe(evData => evData.Tasks.Add(SaveAsync())));
            
            builder.Add(MessageBroker.Receive<UserLoadProgressEvent>()
                .Subscribe(evData => evData.Tasks.Add(LoadAsync())));

            subscriptions = builder.Build();
        }

        public void Dispose()
        {
            subscriptions?.Dispose();
            Redirections?.Clear();
            subscriptions = null;
            Redirections = null;
            current = null;
        }

        public static void SetName(string value)
        {           
            if (current == null)
                return;
            
            current.Name = value ?? DefaultName;
            MessageBroker.Publish(new UserNameChangedEvent());
        }
        
        public static string GetUserPath(string userId = null)
            => Path.Combine(UserKey, userId ?? Id);

        public static void Create(string name)
        {
            current = new UserModel {Name = name};
            MessageBroker.Publish(new UserCreatedEvent());
        }
        
        private UniTask LoadAsync()
        {
            if (serializeService.TryGet(LastUserKey, out var userId))
            {
                current = JsonConvert.DeserializeObject<UserModel>(serializeService.Get(GetUserPath(userId)));
                return UniTask.CompletedTask;
            }

            Create(DefaultName);
            return UniTask.CompletedTask;
        }

        private UniTask SaveAsync()
        {
            if (current == null)
                return UniTask.CompletedTask;

            var jsonContent = JsonConvert.SerializeObject(current);
            serializeService.Patch(LastUserKey, Id);
            serializeService.Patch(GetUserPath(Id), jsonContent);
            serializeService.Save();
            
            return UniTask.CompletedTask;
        }
    }
}