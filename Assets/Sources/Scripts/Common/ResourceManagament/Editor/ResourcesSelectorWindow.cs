using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Potman.Common.ResourceManagament.Editor
{
    public class ResourcesSelectorWindow : EditorWindow
    {
        private Type _filterType;
        private List<Object> _list = new();
        private Action<Object> _result;
        private Vector2 _scroll;
        private string _search = "";

        private GUIStyle _searchStyle;

        public static void Open(Type type, Action<Object> result)
        {
            var window = CreateInstance<ResourcesSelectorWindow>();
            window.titleContent = new GUIContent($"Select {type.Name} Asset");
            window._filterType = type;
            window.minSize = new Vector2(500, 400);
            window._result = result;
            window.ShowUtility();
            window.Focus();
            window.FetchFilteredItems();
        }

        private void OnGUI()
        {
            DrawSearchBar();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            foreach (var obj in _list)
            {
                if (!string.IsNullOrEmpty(_search) && !obj.name.ToLower().Contains(_search.ToLower()))
                    continue;

                DrawItem(obj);
            }
            
            EditorGUILayout.EndScrollView();

            if (_list.Count == 0)
                EditorGUILayout.HelpBox($"No {_filterType.Name} found.", MessageType.Info);
        }

        private void DrawSearchBar()
        {
            if (_searchStyle == null)
                _searchStyle = GUI.skin.FindStyle("ToolbarSeachTextField") ?? GUI.skin.textField;

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            _search = GUILayout.TextField(_search, _searchStyle, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(20)))
            {
                _search = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawItem(Object item)
        {
            if (!item)
                return;
            
            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label(AssetPreview.GetAssetPreview(item) ?? AssetPreview.GetMiniThumbnail(item), GUILayout.Width(64), GUILayout.Height(64));
            EditorGUILayout.BeginVertical();
            GUILayout.Label(item.name, EditorStyles.boldLabel);
            GUILayout.Label(AssetDatabase.GetAssetPath(item), EditorStyles.miniLabel);

            if (GUILayout.Button("Select", GUILayout.Width(80)))
            {
                _result?.Invoke(item);
                Close();
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(4);
        }
        
        public static bool IsMatchedItem(Object obj, Type filterType)
        {
            if (!obj) 
                return false;
            
            var isComponent = typeof(Component).IsAssignableFrom(filterType);
            var isPrefab = typeof(GameObject) == filterType || isComponent;

            if (isPrefab && (obj is not GameObject go || (isComponent && !go.TryGetComponent(filterType, out _))))
                return false;

            return true;
        }
        
        private void FetchFilteredItems()
        {
            _list.Clear();
            var isComponent = typeof(Component).IsAssignableFrom(_filterType);
            var isPrefab = typeof(GameObject) == _filterType || isComponent;
            var filter = $"t:{(isPrefab ? "Prefab" : _filterType.Name)}";
            var guids = AssetDatabase.FindAssets(filter);

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path))
                    continue;
                
                var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (!obj) 
                    continue;

                if (!IsMatchedItem(obj, _filterType))
                    continue;

                _list.Add(obj);
            }

            _list = _list.OrderBy(p => p.name).ToList();
        }
        
        private void OnLostFocus()
        {
            Close();
        }
    }
}
