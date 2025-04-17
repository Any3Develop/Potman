using UnityEditor;
using UnityEngine;

namespace Potman.Common.Editor
{
    public static class AssetReserializeUtility
    {
        [MenuItem("Assets/Force Reserialize ScriptableObjects In Folder", true)]
        private static bool ValidateSelection()
        {
            return Selection.assetGUIDs.Length == 1 && AssetDatabase.IsValidFolder(AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]));
        }

        [MenuItem("Assets/Force Reserialize ScriptableObjects In Folder")]
        private static void ReserializeScriptableObjectsInFolder()
        {
            var folderPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            var guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] {folderPath});
            var count = 0;

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (so == null) 
                    continue;
                EditorUtility.SetDirty(so);
                count++;
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"[Reserialize] Re-serialized {count} ScriptableObjects in folder: {folderPath}");
        }

        [MenuItem("Tools/Assets/Force Reserialize All ScriptableObjects In Project")]
        public static void ReserializeAllScriptableObjects()
        {
            var guids = AssetDatabase.FindAssets("t:ScriptableObject");
            var count = 0;

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (so == null) continue;
                EditorUtility.SetDirty(so);
                count++;
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"[Reserialize] Re-serialized {count} ScriptableObjects in entire project");
        }
    }
}