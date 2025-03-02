using Assets.Tests.Scripts.Weapons.Assets__0;
using Assets.Tests.Scripts.Weapons.Assets__v0;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons
{
    public class X0_MultiMissileLauncherDefinitions : MissileLauncherDefinitions_AB, ILauncherActionDefinitions, ILauncherActionDefinitionsEditor
    {
        [SerializeField]
        protected JsonAssetAgent<X0_ActionNumericalDefinitions> actionNumericalDefinitionsAssetAgent;
        protected X0_ActionNumericalDefinitions actionNumericalDefinitions;
        public float CoverOpenOrCloseDuration
        {
            get => actionNumericalDefinitions.CoverOpenOrCloseDurationTime;
            set => actionNumericalDefinitions.CoverOpenOrCloseDurationTime = value;
        }
        
        public float MagazineFullOrEmptyDuration
        {
            get => actionNumericalDefinitions.MagazineFullOrEmptyDurationTime;
            set => actionNumericalDefinitions.MagazineFullOrEmptyDurationTime = value;
        }
#if UNITY_EDITOR
        LauncherNumericalDefinitions _subNumericalDefinitions;
        [SerializeField]
        JsonAssetAgent<LauncherNumericalDefinitions> _subNumericalAssetAgent;

        public override void Save()
        {
            base.Save();
            actionNumericalDefinitionsAssetAgent.Save(actionNumericalDefinitions);


            ApplyDefinitionsForSubNumericalDefinitions();
            _subNumericalAssetAgent.Save(_subNumericalDefinitions);


            if (_subEditors == null)
                LoadSubEditors();
            foreach (var l in _subEditors)
            {
                ApplyAssetDefinitionsForSubEditor(l);

                l.Load();
            }
        }
        public override void Load()
        {
            base.Load();
            actionNumericalDefinitions = actionNumericalDefinitionsAssetAgent.Load() ?? new();
            _subNumericalDefinitions = _subNumericalAssetAgent.Load() ?? new();
            if (_subEditors == null)
                LoadSubEditors();
            foreach (var l in _subEditors)
            {
                ApplyAssetDefinitionsForSubEditor(l);
                l.Load();
            }
        }
        IMissileLauncherDefinitionsEditor[] _subEditors;
        protected void LoadSubEditors()
        {
            var list = this.gameObject.GetComponentsInChildren<IMissileLauncherDefinitionsEditor>().ToList();
            if (list.Contains(this))
                list.Remove(this);
            _subEditors = list.ToArray();
        }
        protected void ApplyAssetDefinitionsForSubEditor(IMissileLauncherDefinitionsEditor editor)
        {
            editor.OriginAssetDefinitions.Name = originAssetAgent.Definitions.Name;
            editor.OriginAssetDefinitions.BundleName = originAssetAgent.Definitions.BundleName;
            editor.OriginAssetDefinitions.DirPath = originAssetAgent.Definitions.DirPath;


            editor.NumericalAssetDefinitions.Name = _subNumericalAssetAgent.Definitions.Name;
            editor.NumericalAssetDefinitions.BundleName = _subNumericalAssetAgent.Definitions.BundleName;
            editor.NumericalAssetDefinitions.DirPath = _subNumericalAssetAgent.Definitions.DirPath;
        }

        protected void ApplyDefinitionsForSubNumericalDefinitions()
        {
            _subNumericalDefinitions.MagazinePosition = numericalDefinitions.MagazinePosition;
            _subNumericalDefinitions.MuzzlePosition = numericalDefinitions.MuzzlePosition;
            _subNumericalDefinitions.AmmoSpareQuantity = 1;
            _subNumericalDefinitions.AmmoInMagazineQuantity = 1;
            _subNumericalDefinitions.ReloadDuration = actionNumericalDefinitions.CoverOpenOrCloseDurationTime + actionNumericalDefinitions.MagazineFullOrEmptyDurationTime * 2 + numericalDefinitions.ReloadDuration;
            _subNumericalDefinitions.LaunchDurationTime = numericalDefinitions.LaunchDurationTime;
            _subNumericalDefinitions.LaunchDelayRange = new Vector2(actionNumericalDefinitions.CoverOpenOrCloseDurationTime + numericalDefinitions.LaunchDelayRange.x, numericalDefinitions.LaunchDelayRange.y);
        }
#endif
    }
}
