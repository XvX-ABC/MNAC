using Assets.Scripts.Utilities.Assets;
using Assets.Tests.Scripts.Weapons.Assets;
using System.IO;
using System.Reflection;
using Tests.Weapons;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Tests.Scripts.Weapons.Editor.Assets
{
    public class LauncherDefinitionsEditorBase<T> : UnityEditor.Editor where T : class, ILauncherDefinitions
    {
        object _definitions;
        GameObject _origin;
        JsonAssetAgent _definitionsAssetAgent;
        ObjectAssetAgent _ammoOriginAssetAgent;
        O LoadPrivateFieldValue<O>(object obj, string name)
        {
            var type = typeof(T);
            var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
            return (O)field.GetValue(obj);
        }
        private void OnEnable()
        {
            var daa = serializedObject.FindProperty("definitionsAssetAgent");
            var abPath = Path.Combine(daa.FindPropertyRelative("bundlePath").stringValue, daa.FindPropertyRelative("resourceName").stringValue);
            _definitions = LoadPrivateFieldValue<object>(target, "_definitions");
            _definitionsAssetAgent = new(abPath, () => _definitions);

            var aoaa = serializedObject.FindProperty("origin");
            abPath = Path.Combine(aoaa.FindPropertyRelative("bundlePath").stringValue, aoaa.FindPropertyRelative("resourceName").stringValue);
            _origin = LoadPrivateFieldValue<GameObject>(target, "origin");
        }
        public override void OnInspectorGUI()
        {

        }
    }
}
