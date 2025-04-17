namespace Potman.Game.Entities.Abstractions
{
    public interface IEntityMapper
    {
        T Find<T>(bool recursive = false) where T : class;
        T[] FindAll<T>(bool recursive = false) where T : class;
        bool TryFind<T>(out T result, bool recursive = false) where T : class;

        void AddMap<TMap>(string id, TMap obj) where TMap : class;
        TMap Map<TMap>(string id = null) where TMap : class;
        TMap[] MapAll<TMap>() where TMap : class;

        bool TryMap<TMap>(out TMap result, string id = null) where TMap : class;

        TMap RecursiveMap<TMap>(string id = null, int depth = 0) where TMap : class;
        TMap[] RecursiveMapAll<TMap>(string id = null, int depth = 0) where TMap : class;
    }
}