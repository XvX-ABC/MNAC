using Assets.Tests.Scripts.Weapons.Assets__0;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Weapons;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons.Assets__v0
{
    public class MultiMissileLauncherDefinitions_AB : MonoBehaviour, IMissileLauncherDefinitions, IMissileLauncherDefinitionsEditor
    {

        [SerializeField]
        JsonAssetAgent<MissileLauncherNumericalDefinitions> _numericalDefinitionsAssetAgent;
        MissileLauncherNumericalDefinitions _numericalDefinitions;

        [SerializeField]
        GameObjectAssetAgent _originAssetAgent;
        GameObject _origin;
        public GameObject AmmoOrigin { get => _origin; set => _origin = value; }
        public float LaunchDurationTime { get => _numericalDefinitions.LaunchDurationTime; set => _numericalDefinitions.LaunchDurationTime = value; }

        public Vector2 LaunchDelayRange { get => _numericalDefinitions.LaunchDelayRange; set => _numericalDefinitions.LaunchDelayRange = value; }


        public Vector3 MagazinePosition { get => _numericalDefinitions.MagazinePosition; set => _numericalDefinitions.MagazinePosition = value; }

        public Vector3 MuzzlePosition { get => _numericalDefinitions.MuzzlePosition; set => _numericalDefinitions.MuzzlePosition = value; }

        public ushort AmmoTotalQuantity { get => (ushort)(_numericalDefinitions.AmmoSpareQuantity + _numericalDefinitions.AmmoQuantityInMagazine); }

        public ushort AmmoSpareQuantity { get => _numericalDefinitions.AmmoSpareQuantity; set => _numericalDefinitions.AmmoSpareQuantity = value; }

        public ushort AmmoInMagazineQuantity { get => _numericalDefinitions.AmmoQuantityInMagazine; set => _numericalDefinitions.AmmoQuantityInMagazine = value; }

        public float ReloadDuration { get => _numericalDefinitions.ReloadDuration; set => _numericalDefinitions.ReloadDuration = value; }

        public void LoadOrigin()
        {
            _origin = _originAssetAgent.Load();
        }
        public void LoadNumericalDefinitions()
        {
            _numericalDefinitions = _numericalDefinitionsAssetAgent.Load();
        }
#if UNITY_EDITOR
        IMissileLauncherDefinitionsEditor[] _subEditors;




        protected void LoadSubEditors()
        {
            var list = this.gameObject.GetComponentsInChildren<IMissileLauncherDefinitionsEditor>().ToList();
            if (list.Contains(this))
                list.Remove(this);
            _subEditors = list.ToArray();

        }
        protected void ApplyDefinitionsForSubEditor(IMissileLauncherDefinitionsEditor editor)
        {
            editor.AmmoOrigin = _origin;
            editor.MagazinePosition = _numericalDefinitions.MagazinePosition;
            editor.MuzzlePosition = _numericalDefinitions.MuzzlePosition;
            editor.LaunchDurationTime = _numericalDefinitions.LaunchDurationTime;

            editor.AmmoSpareQuantity = 1;
            editor.AmmoInMagazineQuantity = 1;
            editor.ReloadDuration = _numericalDefinitions.ReloadDuration;
            editor.LaunchDelayRange = _numericalDefinitions.LaunchDelayRange;
        }

        public void Save()
        {
            _originAssetAgent.Save(_origin);
            _numericalDefinitionsAssetAgent.Save(_numericalDefinitions);
        }

        public void Load()
        {
            LoadOrigin();
            LoadNumericalDefinitions();
        }
#endif
    }
}
