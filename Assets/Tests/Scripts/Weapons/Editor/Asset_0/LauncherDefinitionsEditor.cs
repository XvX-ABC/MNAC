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
using Definitions = Assets.Tests.Scripts.Weapons.Assets__0.LauncherDefinitions_AB.Definitions;
namespace Assets.Tests.Scripts.Weapons.Editor.Asset_0
{
    [CustomEditor(typeof(MissileLauncherDefinitions_AB))]
    public class LauncherDefinitionsEditor : UnityEditor.Editor
    {
        LauncherDefinitions_AB _instance;



        Definitions _definitionsObj;


        SerializedProperty _definitionsAssetAgent;
        JsonAssetAgent<Definitions> _definitionsAssetAgentObj;
        Action _loadDefinitions;




        GameObject _originObj;

        SerializedProperty _originAssetAgent;
        GameObjectAssetAgent _originAssetAgentObj;
        Action _loadOrigin;



        bool _foldout_0;
        private void OnEnable()
        {
            _instance = target as LauncherDefinitions_AB;



            _definitionsAssetAgent = serializedObject.FindProperty("definitionsAssetAgent");
            var field = typeof(LauncherDefinitions_AB).GetField("definitionsAssetAgent", BindingFlags.NonPublic | BindingFlags.Instance);
            _definitionsAssetAgentObj = field.GetValue(_instance) as JsonAssetAgent<Definitions>;

            var method = typeof(LauncherDefinitions_AB).GetMethod("LoadDefinitions", BindingFlags.NonPublic | BindingFlags.Instance);
            _loadDefinitions = method.CreateDelegate(typeof(Action), _instance) as Action;
            LoadDefinitions();


            _originAssetAgent = serializedObject.FindProperty("originAssetAgent");
            field = typeof(LauncherDefinitions_AB).GetField("originAssetAgent", BindingFlags.NonPublic | BindingFlags.Instance);
            _originAssetAgentObj = field.GetValue(_instance) as GameObjectAssetAgent;

            method = typeof(LauncherDefinitions_AB).GetMethod("LoadOrigin", BindingFlags.NonPublic | BindingFlags.Instance);
            _loadOrigin = method.CreateDelegate(typeof(Action), _instance) as Action;
            LoadOrigin();
        }
        protected virtual void DrawFieldsOfDefinitions(Definitions d)
        {
            d.MagazinePosition = EditorGUILayout.Vector3Field(nameof(d.MagazinePosition), d.MagazinePosition);
            d.MuzzlePosition = EditorGUILayout.Vector3Field(nameof(d.MuzzlePosition), d.MuzzlePosition);
            d.LaunchRate = EditorGUILayout.FloatField(nameof(d.LaunchRate), d.LaunchRate);
            d.AmmoSpareQuantity = (ushort)EditorGUILayout.IntField(nameof(d.AmmoSpareQuantity), d.AmmoSpareQuantity);
            d.AmmoQuantityInMagazine = (ushort)EditorGUILayout.IntField(nameof(d.AmmoQuantityInMagazine), d.AmmoQuantityInMagazine);
            d.ReloadDuration = EditorGUILayout.FloatField(nameof(d.ReloadDuration), d.ReloadDuration);
        }
        protected void DrawDefinitions()
        {
            var d = _definitionsObj;
            _foldout_0 = EditorGUILayout.Foldout(_foldout_0, "Definitions", true);
            if (_foldout_0)
            {
                DrawFieldsOfDefinitions(d);
            }
        }

        protected void LoadDefinitions()
        {
            _loadDefinitions();
            var field = typeof(LauncherDefinitions_AB).GetField("_definitions", BindingFlags.NonPublic | BindingFlags.Instance);
            _definitionsObj = field.GetValue(_instance) as Definitions;
            if (_definitionsObj == null)
                _definitionsObj = new();
        }
        protected void LoadOrigin()
        {
            _loadOrigin();
            var field = typeof(LauncherDefinitions_AB).GetField("origin", BindingFlags.NonPublic | BindingFlags.Instance);
            _originObj = field.GetValue(_instance) as GameObject;
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefinitions();

            EditorGUILayout.PropertyField(_definitionsAssetAgent);
            if (GUILayout.Button("Save Definitions"))
            {

                _definitionsAssetAgentObj.Save(_definitionsObj);
            }
            if (GUILayout.Button("Loaders Initialize"))
            {
                LoadDefinitions();
            }


            _originObj = EditorGUILayout.ObjectField(_originObj, typeof(GameObject), false) as GameObject;
            EditorGUILayout.PropertyField(_originAssetAgent);
            if (GUILayout.Button("Save Origin"))
            {
                _originAssetAgentObj.Save(_originObj);
            }
            if (GUILayout.Button("Load Origin"))
            {
                LoadOrigin();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
