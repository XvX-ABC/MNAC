using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Definitions = Assets.Tests.Scripts.Weapons.X0_AnimatorDefinitions.Definitions;
namespace Assets.Tests.Scripts.Weapons.Editor.Asset_0
{
    [CustomEditor(typeof(X0_AnimatorDefinitions))]
    public class X0_AnimatorDefinitionsEditor : UnityEditor.Editor
    {
        X0_AnimatorDefinitions _instance;
        SerializedProperty _definitionsAssetAgent;
        FieldInfo _definitionsField;
        Definitions _definitionsObj;
        FieldInfo[] _delementFields;
        bool _foldout_0;
        private void OnEnable()
        {
            _instance = target as X0_AnimatorDefinitions;
            _definitionsAssetAgent = serializedObject.FindProperty("_definitionsAssetAgent");
            _definitionsField = typeof(X0_AnimatorDefinitions).GetField("_definitions", BindingFlags.NonPublic | BindingFlags.Instance);
            _delementFields = typeof(Definitions).GetFields();
            _instance.Load();
            GetDefinitions();
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefinitions();
            EditorGUILayout.PropertyField(_definitionsAssetAgent);
            DrawAssetsOptions();

            serializedObject.ApplyModifiedProperties();
        }
        void DrawDefinitions()
        {
            var d = _definitionsObj;
            _foldout_0 = EditorGUILayout.Foldout(_foldout_0, nameof(Definitions), true);
            if (_foldout_0)
            {
                foreach (var f in _delementFields)
                    f.SetValue(d, EditorGUILayout.TextField(f.Name, f.GetValue(d) as string));
            }
        }
        protected virtual void DrawAssetsOptions()
        {
            if (GUILayout.Button("Save Asset"))
            {
                _instance.Save();
            }
            if (GUILayout.Button("Load Asset"))
            {
                _instance.Load();
                GetDefinitions();
            }
        }
        void GetDefinitions()
        {
            _definitionsObj = _definitionsField.GetValue(_instance) as Definitions;
        }
    }
}
