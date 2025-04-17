using System.Collections.Generic;
using System.Linq;
using Potman.Lobby.Identity.Abstractions;

namespace Potman.Lobby.Identity
{
    public class RedirectionCollection : List<IRedirectionArg>
    {
        public T GetArg<T>() where T : IRedirectionArg => (T) this.FirstOrDefault(x => x is T);
        public T[] GetArgs<T>() where T : IRedirectionArg => this.OfType<T>().ToArray();
    }
}