using System.Reflection;
using UnityEditor;
namespace Assets.Tests.Scripts.Weapons.Editor.Asset_0
{
    [CustomEditor(typeof(X0_MultiMissileLauncherDefinitions))]
    public class X0_MissileLauncherDefinitionsEditor : LauncherDefinitionsEditorBase<X0_MultiMissileLauncherDefinitions>
    {
        SerializedProperty _actionNumericalDefinitionsAssetAgent;

        FieldInfo _actionNumericalDefinitionsField;
        X0_ActionNumericalDefinitions _actionNumericalDefinitionsObj;


        SerializedProperty _subNumericalAssetDefinitions;
        bool _foldout_actionDefinitions;
        protected override void OnEnable()
        {
            _actionNumericalDefinitionsAssetAgent = serializedObject.FindProperty("actionNumericalDefinitionsAssetAgent");
            _actionNumericalDefinitionsField = typeof(X0_MultiMissileLauncherDefinitions).GetField("actionNumericalDefinitions", BindingFlags.NonPublic | BindingFlags.Instance);

            _subNumericalAssetDefinitions = serializedObject.FindProperty("_subNumericalAssetAgent");
            base.OnEnable();
        }
        protected override void GetDefinitions()
        {
            base.GetDefinitions();
            var field = _actionNumericalDefinitionsField;
            _actionNumericalDefinitionsObj = field.GetValue(instance) as X0_ActionNumericalDefinitions;
            if (_actionNumericalDefinitionsObj == null)
                _actionNumericalDefinitionsObj = new();

        }
        protected override void DrawFieldsOfDefinitions()
        {
            var d = definitionsObj;
            d.MagazinePosition = EditorGUILayout.Vector3Field(nameof(d.MagazinePosition), d.MagazinePosition);
            d.MuzzlePosition = EditorGUILayout.Vector3Field(nameof(d.MuzzlePosition), d.MuzzlePosition);

            d.AmmoInMagazineQuantity = (ushort)EditorGUILayout.IntField(nameof(d.AmmoInMagazineQuantity), d.AmmoInMagazineQuantity);

            d.ReloadDuration = EditorGUILayout.FloatField(nameof(d.ReloadDuration), d.ReloadDuration);
            d.LaunchDelayRange = EditorGUILayout.Vector2Field(nameof(d.LaunchDelayRange), d.LaunchDelayRange);
            d.LaunchDurationTime = EditorGUILayout.FloatField(nameof(d.LaunchDurationTime), d.LaunchDurationTime);

        }
        protected virtual void DrawFieldsOfActionDefinitions()
        {
            var d = _actionNumericalDefinitionsObj;
            d.CoverOpenOrCloseDurationTime = EditorGUILayout.FloatField(nameof(d.CoverOpenOrCloseDurationTime), d.CoverOpenOrCloseDurationTime);
            d.MagazineFullOrEmptyDurationTime = EditorGUILayout.FloatField(nameof(d.MagazineFullOrEmptyDurationTime), d.MagazineFullOrEmptyDurationTime);
        }
        protected virtual void DrawActionDefinitions()
        {
            _foldout_actionDefinitions = EditorGUILayout.Foldout(_foldout_actionDefinitions, "Action Definitions", true);
            if (_foldout_actionDefinitions)
                DrawFieldsOfActionDefinitions();
            EditorGUILayout.PropertyField(_actionNumericalDefinitionsAssetAgent);
        }
        protected override void DrawDefinitions()
        {
            base.DrawDefinitions();
            DrawActionDefinitions();
            EditorGUILayout.PropertyField(_subNumericalAssetDefinitions);
        }
    }
}
