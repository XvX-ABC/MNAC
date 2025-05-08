using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Tests.Assets;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Assets.Tests.Scripts.Assets
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
        void LoadAllAssetFields()
        {
            var t = target.GetType();
            var fields = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            var list = new List<AssetField>();
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
                if (t.IsArray)
                {
                    t = t.GetElementType();
                    interfaces = t.GetInterfaces();
                    if (!interfaces.Any(i => i == typeof(IAssetAgent_Managed)))
                    {
                        continue;
                    }
                    isArray = true;
                }
                if (!interfaces.Any(i => i == typeof(IAssetAgent_Managed)) || !t.IsSerializable)
                {
                    continue;
                }

                var prop = serializedObject.FindProperty(field.Name);
                var assetField = new AssetField()
                {
                    Info = field,
                    Obj = field.GetValue(target),
                    Property = prop,
                    IsArray = isArray
                };

                list.Add(assetField);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine();
            }
            //Debug.Log(sb.ToString());
            _assetFields = list.ToArray();
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
            Debug.Log("OnEnable");
            LoadAllAssetFields();
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawAllAssetFields();
            DrawButtons();
            serializedObject.ApplyModifiedProperties();
        }
    }
    [Serializable]
    public class Data
    {
        public string Field_0;
        public string Field_1;
        public string Field_2;
        public string Field_3;
    }
    public class JsonAsset_Test : MonoBehaviour
    {

        [SerializeField]
        JsonAssetAgent_Managed<Data> _assetAgent;
        [SerializeField]
        JsonAssetAgent_Managed<Data>[] _assetAgents;
        [SerializeField]
        PrefabAssetAgent_Managed _prefabAgent;
    }
}
