using Assets.Scripts.Utilities.Assets;
using Assets.Tests.Scripts.Weapons.Assets__0;
using Assets.Tests.Scripts.Weapons.Assets__v0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using UnityEditor;
using UnityEngine;
using AssetDefinitions = Assets.Tests.Scripts.Weapons.Assets__0.AssetDefinitions;
namespace Assets.Tests.Scripts.Weapons.Editor.Asset_0
{
    public class LauncherDefinitionsEditorBase<T> : UnityEditor.Editor where T : LauncherDefinitions_AB
    {
        T _instance;



        protected LauncherNumericalDefinitions definitionsObj;
        FieldInfo _definitionsField;

        SerializedProperty _definitionsAssetAgent;
        JsonAssetAgent<LauncherNumericalDefinitions> _definitionsAssetAgentObj;
        Action _loadDefinitions;




        GameObject _originObj;
        FieldInfo _originField;

        SerializedProperty _originAssetAgent;
        GameObjectAssetAgent _originAssetAgentObj;
        Action _loadOrigin;



        bool _foldout_definitions;
        bool _foldout_origin;
        protected virtual void OnEnable()
        {
            _instance = target as T;



            _definitionsAssetAgent = serializedObject.FindProperty("definitionsAssetAgent");
            var field = typeof(LauncherDefinitions_AB).GetField("definitionsAssetAgent", BindingFlags.NonPublic | BindingFlags.Instance);
            _definitionsAssetAgentObj = field.GetValue(_instance) as JsonAssetAgent<LauncherNumericalDefinitions>;

            _definitionsField = typeof(LauncherDefinitions_AB).GetField("_numericalDefinitions", BindingFlags.NonPublic | BindingFlags.Instance);

            var method = typeof(LauncherDefinitions_AB).GetMethod("LoadNumericalDefinitions", BindingFlags.NonPublic | BindingFlags.Instance);
            _loadDefinitions = method.CreateDelegate(typeof(Action), _instance) as Action;
            LoadDefinitions();




            _originAssetAgent = serializedObject.FindProperty("originAssetAgent");
            field = typeof(LauncherDefinitions_AB).GetField("originAssetAgent", BindingFlags.NonPublic | BindingFlags.Instance);
            _originAssetAgentObj = field.GetValue(_instance) as GameObjectAssetAgent;

            _originField = typeof(LauncherDefinitions_AB).GetField("origin", BindingFlags.NonPublic | BindingFlags.Instance);

            method = typeof(LauncherDefinitions_AB).GetMethod("LoadOrigin", BindingFlags.NonPublic | BindingFlags.Instance);
            _loadOrigin = method.CreateDelegate(typeof(Action), _instance) as Action;
            LoadOrigin();
        }
        protected virtual void DrawFieldsOfDefinitions()
        {
            var d = definitionsObj;
            d.MagazinePosition = EditorGUILayout.Vector3Field(nameof(d.MagazinePosition), d.MagazinePosition);
            d.MuzzlePosition = EditorGUILayout.Vector3Field(nameof(d.MuzzlePosition), d.MuzzlePosition);


            d.AmmoSpareQuantity = (ushort)EditorGUILayout.IntField(nameof(d.AmmoSpareQuantity), d.AmmoSpareQuantity);
            d.AmmoQuantityInMagazine = (ushort)EditorGUILayout.IntField(nameof(d.AmmoQuantityInMagazine), d.AmmoQuantityInMagazine);


            d.ReloadDuration = EditorGUILayout.FloatField(nameof(d.ReloadDuration), d.ReloadDuration);
            d.LaunchDelayRange = EditorGUILayout.Vector2Field(nameof(d.LaunchDelayRange), d.LaunchDelayRange);
            d.LaunchDurationTime = EditorGUILayout.FloatField(nameof(d.LaunchDurationTime), d.LaunchDurationTime);
        }
        protected void DrawDefinitions()
        {
            _foldout_definitions = EditorGUILayout.Foldout(_foldout_definitions, "Definitions", true);
            if (_foldout_definitions)
            {
                DrawFieldsOfDefinitions();
            }
        }

        protected void LoadDefinitions()
        {
            var field = _definitionsField;
            definitionsObj = field.GetValue(_instance) as LauncherNumericalDefinitions;
            if (definitionsObj == null)
                definitionsObj = new();
        }
        protected void LoadOrigin()
        {
            var field = _originField;
            _originObj = field.GetValue(_instance) as GameObject;
        }
        protected void SetOrigin(GameObject obj)
        {
            _originField.SetValue(_instance, obj);
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefinitions();

            EditorGUILayout.PropertyField(_definitionsAssetAgent);


            _foldout_origin = EditorGUILayout.Foldout(_foldout_origin, "Origin", true);
            if (_foldout_origin)
                _originObj = EditorGUILayout.ObjectField(_originObj, typeof(GameObject), false) as GameObject;
            EditorGUILayout.PropertyField(_originAssetAgent);
            if (GUILayout.Button("Save Asset"))
            {
                SetOrigin(_originObj);
                //_originAssetAgentObj.Save(_originObj);
                _instance.Save();
            }
            if (GUILayout.Button("Load Asset"))
            {
                //LoadOrigin();
                _instance.Load();
                LoadOrigin();
                LoadDefinitions();
            }

            serializedObject.ApplyModifiedProperties();
        }
        protected virtual void OnDisable()
        {
            ABLoader.Instance.UnLoadAll();
        }
    }
}
