using Codice.CM.Common;
using System;
using System.Reflection;
using UnityEditor;
using static Assets.Tests.Scripts.Weapons.X0_MultiMissileLauncher;

namespace Assets.Tests.Scripts.Weapons.Editor.Assets
{
    //[CustomEditor(typeof(X0_MultiMissileLauncherDefinitions))]
    public class X0_MultiMissileLauncherDefinitionsEditor : UnityEditor.Editor
    {
        SubLauncherInfo[] _subLauncherInfos;
        X0_MultiMissileLauncher _currentLauncher;
        Type _type;
        private void OnEnable()
        {
            _type = typeof(X0_MultiMissileLauncher);
            _currentLauncher = target as X0_MultiMissileLauncher;
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();



            serializedObject.ApplyModifiedProperties();
        }
        void DrawSubLauncherInfos(SubLauncherInfo info)
        {
            var name = info.Obj.name;
            info.Foldout = EditorGUILayout.Foldout(info.Foldout, name, true);
            if (info.Foldout)
            {
                var definitionEditor = info.DefinitionsEditor;
                EditorGUI.indentLevel+=2;
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("Path: ", info.Path);
                EditorGUI.EndDisabledGroup();
                definitionEditor.DrawElements();
                EditorGUI.indentLevel -=2;
            }
        }
        void LoadSubLauncherInfos()
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var field = _type.GetField(nameof(_subLauncherInfos), flags);
            var v = field.GetValue(_currentLauncher) as SubLauncherInfo[];
            if (v == null)
            {
                var method = _type.GetMethod(nameof(LoadSubLauncherInfos), flags);
                method.Invoke(_currentLauncher, null);
                v = field.GetValue(_currentLauncher) as SubLauncherInfo[];

            }

            _subLauncherInfos = v;
        }
    }
}
