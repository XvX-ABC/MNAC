using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tests.Weapons;
using UnityEditor;

namespace Assets.Tests.Scripts.Weapons.Editor
{
    public class MultiMissileLauncherEditor : UnityEditor.Editor
    {
        IMissileLauncher[] _subLaunchers;
        private void OnEnable()
        {
            var i = target as MultiMissileLauncher;
            _subLaunchers = (IMissileLauncher[])typeof(MultiMissileLauncher).GetField("subLaunchers", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(i);
        }
        void ShowDefinesOfSubLaunchers()
        {
            foreach(var l in _subLaunchers)
            {
                var defines = l.Defines;

            }
        }
        public override void OnInspectorGUI()
        {

        }
    }
}
