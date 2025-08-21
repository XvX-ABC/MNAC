using System;
using System.IO;
using System.Reflection;
using System.Text;
using Tests.Weapons.MissileLauncher;
using UnityEditor;
using UnityEngine;
using Utilities.Timeline;

namespace Tests.Weapons.MultiMissileLauncher
{

    public class X0_MultiMissileLauncher : MultiMissileLauncher
    {
#if UNITY_EDITOR
        PropertyInfo _objProp;
        SubLauncherInfo[] _subLauncherInfos;
        public class SubLauncherInfo
        {
            public string Path;
            public string FullPath;
            public IMissileLauncher Launcher;
            public GameObject Obj;
            public IMissileLauncherDefinitions Definitions;

            public DefinitionsEditor DefinitionsEditor;
            public bool Foldout;

            public SubLauncherInfo(string path, string fullPath, IMissileLauncher launcher, GameObject obj)
            {
                Path = path;
                FullPath = fullPath;
                Launcher = launcher;
                Obj = obj;
                Definitions = obj.GetComponent<IMissileLauncherDefinitions>();
                DefinitionsEditor = new DefinitionsEditor(Definitions);
            }
        }

        public class DefinitionsEditor
        {
            IMissileLauncherDefinitions _definition;
            //JsonAssetSaver _definitionsSaver;
            //ObjectAssetSaver _originAssetSaver;
            GameObject _origin;
            bool _foldout;
            public DefinitionsEditor(IMissileLauncherDefinitions definition)
            {
                _definition = definition ?? throw new ArgumentNullException(nameof(definition));

            }
            public void DrawElements()
            {
                _foldout = EditorGUILayout.Foldout(_foldout, "Definition", true);
                if (_foldout)
                {
                    EditorGUI.indentLevel += 2;

                    EditorGUI.indentLevel -= 2;
                }
            }
        }
        void LoadSubLauncherInfos()
        {
            if (_objProp == null)
                _objProp = typeof(Component).GetProperty("gameObject");
            if (subLaunchers == null)
                LoadSubLaunchers();
            var currentPath = GetPath(this.transform);
            _subLauncherInfos = new SubLauncherInfo[subLaunchers.Length];
            for (int i = 0; i < subLaunchers.Length; i++)
            {
                var subLauncher = subLaunchers[i];
                var obj = _objProp.GetValue(subLauncher) as GameObject;
                var fullPath = GetPath(obj.transform);
                var path = Path.GetRelativePath(currentPath, fullPath);
                var info = new SubLauncherInfo(path, fullPath, subLauncher, obj);
                _subLauncherInfos[i] = info;
            }
        }

        string GetPath(Transform trans)
        {
            var currentTrans = trans;
            var result = new StringBuilder();
            while (currentTrans != null)
            {
                var name = currentTrans.gameObject.name;
                result.Insert(0, "/" + name);
                currentTrans = currentTrans.parent;
            }
            return result.ToString();
        }
        protected override ITimeline CreateLaunchDurationTimeline()
        {
            var timeline = base.CreateLaunchDurationTimeline();
            timeline.EndAction += _ => { StartReload(); Debug.Log("Start reload"); };
            return timeline;
        }
#endif
    }
}