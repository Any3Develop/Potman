namespace Potman.Common.SerializeService
{
    public static class SerilizedHelper
    {
        public const string FILE_NAME = "LocalDB.cdb";
        public static string Path => GetPath();

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Clear Serialized Data")]
#endif
        public static void Clear()
        {
            if (System.IO.File.Exists(Path))
                System.IO.File.Delete(Path);
        }

        public static string GetPath()
        {
#if UNITY_EDITOR
            return System.IO.Path.Combine("Library/", FILE_NAME);
#elif UNITY_STANDALONE
			return System.IO.Path.Combine(UnityEngine.Application.dataPath, FILE_NAME);
#else
			return System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, FILE_NAME);
#endif
        }
    }
}