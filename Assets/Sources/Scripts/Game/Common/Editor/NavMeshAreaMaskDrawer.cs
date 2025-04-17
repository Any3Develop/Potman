using System.Linq;
using Potman.Game.Common.Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Potman.Game.Common.Editor
{
    [CustomPropertyDrawer(typeof(NavMeshAreaMaskAttribute))]
    public class NavMeshAreaMaskDrawer : PropertyDrawer
    {
        private string[] areaNames;
        private int[] areaIndices;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.LabelField(position, label.text, "Use NavMeshAreaMask with int fields.");
                return;
            }

            if (areaNames == null || areaIndices == null)
            {
                areaNames = NavMesh.GetAreaNames();
                areaIndices = Enumerable.Range(0, areaNames.Length).ToArray();
            }
            
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

            var currentMask = ConvertNavMeshMaskToLocalMask(property.intValue);

            EditorGUI.BeginChangeCheck();
            var newMask = EditorGUI.MaskField(position, label, currentMask, areaNames);

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.Update();

                var newNavMeshMask = ConvertLocalMaskToNavMeshMask(newMask);
                property.intValue = newNavMeshMask;

                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.showMixedValue = false;
            EditorGUI.EndProperty();
        }
        
        private int ConvertNavMeshMaskToLocalMask(int navMeshMask)
        {
            var mask = 0;
            for (var i = 0; i < areaIndices.Length; i++)
            {
                if ((navMeshMask & (1 << areaIndices[i])) != 0)
                {
                    mask |= 1 << i;
                }
            }

            return mask;
        }
        
        private int ConvertLocalMaskToNavMeshMask(int localMask)
        {
            return areaIndices.Where((i) => (localMask & (1 << i)) != 0).Aggregate(0, (current, t) => current | 1 << t);
        }
    }
}