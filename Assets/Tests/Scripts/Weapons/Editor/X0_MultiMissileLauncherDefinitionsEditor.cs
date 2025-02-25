using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Rendering;

namespace Assets.Tests.Scripts.Weapons.Editor
{
    [CustomEditor(typeof(X0_MultiMissileLauncherDefinitions))]
    public class X0_MultiMissileLauncherDefinitionsEditor : UnityEditor.Editor
    {
        SerializedProperty _ammo;
        SerializedProperty _mountPoints;
        Durations _durations;
        bool _showFoldout;
        private void OnEnable()
        {
            _ammo = serializedObject.FindProperty("_ammo");
            _mountPoints = serializedObject.FindProperty("_mountPoints");
            var d = target as X0_MultiMissileLauncherDefinitions;
            _durations = (Durations)typeof(X0_MultiMissileLauncherDefinitions).GetField("_durations", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(d);
            _ammo = serializedObject.FindProperty("_ammo");
            _mountPoints = serializedObject.FindProperty("_mountPoints");
        }
        void DrawActionNumbersParameters()
        {
            _showFoldout = EditorGUILayout.Foldout(_showFoldout, typeof(Durations).Name, true);
            if (_showFoldout)
            {
                EditorGUI.indentLevel += 2;
                EditorGUI.BeginChangeCheck();
                var coverDuration = EditorGUILayout.FloatField(nameof(_durations.CoverOpenOrCloseDuration), _durations.CoverOpenOrCloseDuration);
                var magazineDuration = EditorGUILayout.FloatField(nameof(_durations.MagazineFullOrEmptyDuration), _durations.MagazineFullOrEmptyDuration);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.FloatField(nameof(_durations.ReloadDuration), _durations.ReloadDuration);
                EditorGUI.EndDisabledGroup();
                if (EditorGUI.EndChangeCheck())
                {
                    _durations.CoverOpenOrCloseDuration = coverDuration;
                    _durations.MagazineFullOrEmptyDuration = magazineDuration;
                }
            }
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_ammo);
            EditorGUILayout.PropertyField(_mountPoints);
            DrawActionNumbersParameters();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
