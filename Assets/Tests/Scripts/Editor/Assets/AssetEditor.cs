using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tests.Assets;
using UnityEditor;
using UnityEngine;

namespace Tests.Editors.Assets
{
    public class AssetEditor : Editor
    {
        class AssetField
        {
            public FieldInfo Info;
            public object Obj;
            public SerializedProperty Property;
            public bool IsArray;
        }
        AssetField[] _assetFields;
        SerializedProperty[] _normallyProps;
        void LoadAllAssetFields()
        {
            var t = target.GetType();
            var fields = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            var assetsList = new List<AssetField>();
            var normallyList = new List<SerializedProperty>();
            var sb = new StringBuilder();
            foreach (var field in fields)
            {
                sb.AppendLine(field.Name);
                t = field.FieldType;

                var interfaces = t.GetInterfaces();

                foreach (var i in interfaces)
                {
                    sb.AppendLine(i.FullName);
                }
                var isArray = false;

                var prop = serializedObject.FindProperty(field.Name);

                if (t.IsArray)
                {
                    t = t.GetElementType();
                    interfaces = t.GetInterfaces();
                    if (!interfaces.Any(i => i == typeof(IAssetAgent_Managed)))
                    {
                        goto Add_To_Normally_List;
                    }
                    isArray = true;
                }
                if (!interfaces.Any(i => i == typeof(IAssetAgent_Managed)) || !t.IsSerializable)
                {
                    goto Add_To_Normally_List;
                }

                var assetField = new AssetField()
                {
                    Info = field,
                    Obj = field.GetValue(target),
                    Property = prop,
                    IsArray = isArray
                };

                assetsList.Add(assetField);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine();
            Add_To_Normally_List:
                normallyList.Add(prop);
            }
            Debug.Log(sb.ToString());
            _assetFields = assetsList.ToArray();
            _normallyProps = normallyList.ToArray();
        }
        void DrawAllNormallyProperties()
        {
            if (_normallyProps == null)
                return;
            foreach (var prop in _normallyProps)
                EditorGUILayout.PropertyField(prop);
        }
        void DrawAllAssetFields()
        {
            if (_assetFields == null)
                return;
            foreach (var field in _assetFields)
            {
                EditorGUILayout.PropertyField(field.Property);
            }
        }
        void DrawButtons()
        {
            if (GUILayout.Button("Save"))
            {
                foreach (var f in _assetFields)
                {
                    var isArray = f.IsArray;
                    if (!isArray)
                    {
                        var assetAgent = (IAssetAgent_Managed)f.Obj;
                        assetAgent.Save();
                        continue;
                    }
                    var assetAgents = (IAssetAgent_Managed[])f.Obj;
                    foreach (var agent in assetAgents)
                    {
                        agent.Save();
                    }
                }
            }
            if (GUILayout.Button("Load"))
            {
                foreach (var f in _assetFields)
                {
                    var isArray = f.IsArray;
                    if (!isArray)
                    {
                        var assetAgent = (IAssetAgent_Managed)f.Obj;
                        assetAgent.Load();
                        continue;
                    }
                    var assetAgents = (IAssetAgent_Managed[])f.Obj;
                    foreach (var agent in assetAgents)
                    {
                        agent.Load();
                    }
                }
            }
        }
        private void OnEnable()
        {
            LoadAllAssetFields();
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawAllNormallyProperties();
            DrawAllAssetFields();
            DrawButtons();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
