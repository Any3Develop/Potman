using System;

namespace Potman.Lobby.Identity
{
    public class UserModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "Unnamed";
    }
}