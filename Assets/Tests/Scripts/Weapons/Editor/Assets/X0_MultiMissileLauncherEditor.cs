using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using static Assets.Tests.Scripts.Weapons.X0_MultiMissileLauncher;

namespace Assets.Tests.Scripts.Weapons.Editor.Assets
{
    //[CustomEditor(typeof(X0_MultiMissileLauncher))]
    public class X0_MultiMissileLauncherEditor : UnityEditor.Editor
    {
        SubLauncherInfo[] _subLauncherInfos;
        X0_MultiMissileLauncher _currentLauncher;
        GameObject _obj;
        Type _type;
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
        void DrawSubLauncherInfo(SubLauncherInfo info)
        {
            var name = info.Obj.name;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField(name, info.Path);
            EditorGUI.EndDisabledGroup();
        }
        protected void OnEnable()
        {
            _type = typeof(X0_MultiMissileLauncher);
            _currentLauncher = target as X0_MultiMissileLauncher;
            _obj = _type.GetProperty("gameObject").GetValue(target) as GameObject;


            LoadSubLauncherInfos();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            foreach (var info in _subLauncherInfos)
                DrawSubLauncherInfo(info);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
