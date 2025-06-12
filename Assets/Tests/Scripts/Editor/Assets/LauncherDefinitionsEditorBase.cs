using Assets.Scripts.Utilities.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Tests.Assets;
using Tests.Weapons.Launcher;
using UnityEditor;
using UnityEngine;
using AssetDefinitions = Tests.Assets.AssetDefinitions;
namespace Tests.Editors.Assets
{
    public class LauncherDefinitionsEditorBase<T> : UnityEditor.Editor where T : LauncherDefinitions_AB
    {
        protected T instance;



        protected LauncherNumericalDefinitions definitionsObj;
        FieldInfo _definitionsField;

        SerializedProperty _definitionsAssetAgent;
        JsonAssetAgent<LauncherNumericalDefinitions> _definitionsAssetAgentObj;
        Action _loadDefinitions;




        GameObject _originObj;
        FieldInfo _originField;

        SerializedProperty _originAssetAgent;
        PrefabAssetAgent _originAssetAgentObj;
        Action _loadOrigin;



        protected bool foldout_definitions;
        protected bool foldout_origin;
        protected virtual void OnEnable()
        {
            instance = target as T;



            _definitionsAssetAgent = serializedObject.FindProperty("numericalAssetAgent");
            var field = typeof(LauncherDefinitions_AB).GetField("numericalAssetAgent", BindingFlags.NonPublic | BindingFlags.Instance);
            _definitionsAssetAgentObj = field.GetValue(instance) as JsonAssetAgent<LauncherNumericalDefinitions>;

            _definitionsField = typeof(LauncherDefinitions_AB).GetField("numericalDefinitions", BindingFlags.NonPublic | BindingFlags.Instance);

            var method = typeof(LauncherDefinitions_AB).GetMethod("LoadNumericalDefinitions", BindingFlags.NonPublic | BindingFlags.Instance);
            _loadDefinitions = method.CreateDelegate(typeof(Action), instance) as Action;




            _originAssetAgent = serializedObject.FindProperty("originAssetAgent");
            field = typeof(LauncherDefinitions_AB).GetField("originAssetAgent", BindingFlags.NonPublic | BindingFlags.Instance);
            _originAssetAgentObj = field.GetValue(instance) as PrefabAssetAgent;

            _originField = typeof(LauncherDefinitions_AB).GetField("origin", BindingFlags.NonPublic | BindingFlags.Instance);

            method = typeof(LauncherDefinitions_AB).GetMethod("LoadOrigin", BindingFlags.NonPublic | BindingFlags.Instance);
            _loadOrigin = method.CreateDelegate(typeof(Action), instance) as Action;


            instance.Load();
            GetOrigin();
            GetDefinitions();
        }
        protected virtual void DrawFieldsOfDefinitions()
        {
            var d = definitionsObj;
            d.MagazinePosition = EditorGUILayout.Vector3Field(nameof(d.MagazinePosition), d.MagazinePosition);
            d.MuzzlePosition = EditorGUILayout.Vector3Field(nameof(d.MuzzlePosition), d.MuzzlePosition);

            d.AmmoSpareQuantity = (ushort)EditorGUILayout.IntField(nameof(d.AmmoSpareQuantity), d.AmmoSpareQuantity);
            d.AmmoInMagazineQuantity = (ushort)EditorGUILayout.IntField(nameof(d.AmmoInMagazineQuantity), d.AmmoInMagazineQuantity);


            d.ReloadDuration = EditorGUILayout.FloatField(nameof(d.ReloadDuration), d.ReloadDuration);
            d.LaunchDelayRange = EditorGUILayout.Vector2Field(nameof(d.LaunchDelayRange), d.LaunchDelayRange);
            d.LaunchDurationTime = EditorGUILayout.FloatField(nameof(d.LaunchDurationTime), d.LaunchDurationTime);
        }
        protected virtual void DrawDefinitions()
        {
            foldout_definitions = EditorGUILayout.Foldout(foldout_definitions, "Definitions", true);
            if (foldout_definitions)
            {
                DrawFieldsOfDefinitions();
            }
            EditorGUILayout.PropertyField(_definitionsAssetAgent);
        }
        protected virtual void DrawOrigin()
        {
            foldout_origin = EditorGUILayout.Foldout(foldout_origin, "Origin", true);
            if (foldout_origin)
                _originObj = EditorGUILayout.ObjectField(_originObj, typeof(GameObject), false) as GameObject;
            EditorGUILayout.PropertyField(_originAssetAgent);
        }
        protected virtual void DrawAssetsOption()
        {
            if (GUILayout.Button("Save Asset"))
            {
                SetOrigin(_originObj);
                //_originAssetAgentObj.Save(_originObj);
                instance.Save();
            }
            if (GUILayout.Button("Load Asset"))
            {
                //LoadOrigin();
                instance.Load();
                GetOrigin();
                GetDefinitions();
            }
        }
        protected virtual void GetDefinitions()
        {
            var field = _definitionsField;
            definitionsObj = field.GetValue(instance) as LauncherNumericalDefinitions;
            if (definitionsObj == null)
                definitionsObj = new();
        }
        protected void GetOrigin()
        {
            var field = _originField;
            _originObj = field.GetValue(instance) as GameObject;
        }
        protected void SetOrigin(GameObject obj)
        {
            _originField.SetValue(instance, obj);
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefinitions();
            DrawOrigin();
            DrawAssetsOption();



            serializedObject.ApplyModifiedProperties();
        }
        protected virtual void OnDisable()
        {
            ABLoader.Instance.UnLoadAll();
        }
    }
}
