using System;
using System.Collections.Generic;
using System.Linq;
using Potman.Common.ResourceManagament.Attributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Potman.Common.ResourceManagament.Editor
{
    [CustomPropertyDrawer(typeof(ResourceAssetIdAttribute))]
    public class ResourceAssetIdDrawer : PropertyDrawer
    {
        private readonly Dictionary<SerializedProperty, (bool, Object)> previewAssets = new();
        private static int _handledDragControlId = -1; // <- глобальный флаг

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (ResourceAssetIdAttribute) attribute;
            var type = attr.FilterType;

            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            var fieldRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y,
                position.width - EditorGUIUtility.labelWidth, position.height);

            EditorGUI.LabelField(labelRect, label);
            
            if (!previewAssets.TryGetValue(property, out var cache) || cache.Item1)
                SetProperyResult(property, FindAssetById(property.stringValue, type), false);

            var objectFieldStyle = new GUIStyle("ObjectField")
            {
                imagePosition = ImagePosition.ImageLeft
            };


            var evt = Event.current;

            const float iconSize = 18f;
            var buttonRect = new Rect(fieldRect.xMax - iconSize, fieldRect.y + 1, iconSize, fieldRect.height - 2);
            var textFieldRect = new Rect(fieldRect.x, fieldRect.y, fieldRect.width - iconSize, fieldRect.height);

            // Контекстное меню по ПКМ
            if (evt.type == EventType.MouseDown && evt.button == 1 && fieldRect.Contains(evt.mousePosition))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Clear"), false, () =>
                {
                    SetProperyResult(property, null);
                });
                menu.ShowAsContext();
                evt.Use();
            }

            // ЛКМ по текстовому полю — пинг ассета
            if (evt.type == EventType.MouseDown && evt.button == 0 && textFieldRect.Contains(evt.mousePosition))
            {
                if (previewAssets.TryGetValue(property, out cache) && cache.Item2)
                {
                    EditorGUIUtility.PingObject(cache.Item2);
                    Selection.activeObject = cache.Item2;
                }

                evt.Use();
            }

            // Кнопка выбора ассета
            var iconButtonStyle = GUI.skin.FindStyle("IN ObjectField");
            if (GUI.Button(buttonRect, GUIContent.none, iconButtonStyle))
            {
                GUI.FocusControl(null);
                ResourcesSelectorWindow.Open(type, result =>
                {
                    SetProperyResult(property, result);
                });
            }
            
            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            var evt2 = Event.current;

            if (fieldRect.Contains(evt2.mousePosition))
            {
                if (evt2.type is EventType.DragUpdated or EventType.DragPerform)
                {
                    var dragged = DragAndDrop.objectReferences.FirstOrDefault();
                    if (ResourcesSelectorWindow.IsMatchedItem(dragged, type))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (evt2.type == EventType.DragPerform)
                        {
                            if (_handledDragControlId != controlId)
                            {
                                _handledDragControlId = controlId;
                                DragAndDrop.AcceptDrag();
                                SetProperyResult(property, dragged);
                            }
                        }

                        evt2.Use();
                    }
                }
            }
            
            var preview = previewAssets.TryGetValue(property, out cache) && cache.Item2 ? AssetPreview.GetMiniThumbnail(cache.Item2) : default;
            var displayName = cache.Item2 ? cache.Item2.name : $"None ({type.Name})";

            // Отображение названия и иконки
            GUI.Button(textFieldRect, new GUIContent(displayName, preview), objectFieldStyle);
            
            // Сбросить флаг после всех событий
            if (evt2.type == EventType.Repaint)
            {
                _handledDragControlId = -1;
            }
        }

        private void SetProperyResult(SerializedProperty property, Object result, bool shouldRepaint = true)
        {
            if (property == null)
                return;

            var id = GetAssetId(result);
            var so = property.serializedObject;
            so.Update();

            property.stringValue = id;
            previewAssets[property] = (shouldRepaint, result);
            so.ApplyModifiedProperties();
        }
        
        private string GetAssetId(Object asset)
        {
            if (!asset || asset == null)
                return string.Empty;

            var path = AssetDatabase.GetAssetPath(asset);
            var guid = AssetDatabase.AssetPathToGUID(path);

#if ADDRESSABLES_INCLUDED
            var settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null)
            {
                var entry = settings.FindAssetEntry(guid);
                if (entry != null)
                    return entry.address;
            }
#endif

            return guid;
        }

        private static Object FindAssetById(string id, Type type)
        {
            string path = null;

#if ADDRESSABLES_INCLUDED
            var settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null)
            {
                var entries = new List<UnityEditor.AddressableAssets.Settings.AddressableAssetEntry>();
                settings.GetAllAssets(entries, false, entryFilter: assetEntry => assetEntry.address == id);
                var entry = entries.FirstOrDefault();
                if (entry != null)
                    path = AssetDatabase.GUIDToAssetPath(entry.guid);
            }
#endif

            if (string.IsNullOrEmpty(path))
                path = AssetDatabase.GUIDToAssetPath(id);

            if (type == typeof(GameObject))
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (typeof(Component).IsAssignableFrom(type))
            {
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                return asset ? asset.GetComponent(type) : asset;
            }

            return AssetDatabase.LoadAssetAtPath(path, type);
        }
    }
}