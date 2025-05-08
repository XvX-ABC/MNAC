using System;
using UnityEditor;
using UnityEngine;

namespace Tests.Assets
{
    //[CustomPropertyDrawer(typeof(PrefabAssetAgent_Managed))]
    public class PrefabAssetAgentEditor : PropertyDrawer
    {
        bool _foldout;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var str = property.name.Replace("_", "");
            var plabel = str.Substring(0, 1).ToUpper() + str.Substring(1, str.Length - 1);
            _foldout = EditorGUILayout.Foldout(_foldout, plabel);
            if (_foldout)
            {
                EditorGUI.indentLevel += 2;
                var assetProp = property.FindPropertyRelative("_asset");
                assetProp.objectReferenceValue = EditorGUILayout.ObjectField("Asset", assetProp.objectReferenceValue, typeof(GameObject), false);
                EditorGUILayout.PropertyField(property.FindPropertyRelative("definitions"));
                EditorGUI.indentLevel -= 2;
            }
        }
    }

}
