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
        protected JsonAssetAgent<MissileLauncherNumericalDefinitions> numericalDefinitionsAssetAgent;
        protected MissileLauncherNumericalDefinitions numericalDefinitions;

        [SerializeField]
        protected GameObjectAssetAgent originAssetAgent;
        protected GameObject origin;
        public GameObject AmmoOrigin { get => origin; set => origin = value; }
        public virtual float LaunchDurationTime { get => numericalDefinitions.LaunchDurationTime; set => numericalDefinitions.LaunchDurationTime = value; }

        public virtual Vector2 LaunchDelayRange { get => numericalDefinitions.LaunchDelayRange; set => numericalDefinitions.LaunchDelayRange = value; }


        public virtual Vector3 MagazinePosition { get => numericalDefinitions.MagazinePosition; set => numericalDefinitions.MagazinePosition = value; }

        public virtual Vector3 MuzzlePosition { get => numericalDefinitions.MuzzlePosition; set => numericalDefinitions.MuzzlePosition = value; }

        public virtual ushort AmmoTotalQuantity { get => (ushort)(numericalDefinitions.AmmoSpareQuantity + numericalDefinitions.AmmoInMagazineQuantity); }

        public virtual ushort AmmoSpareQuantity { get => numericalDefinitions.AmmoSpareQuantity; set => numericalDefinitions.AmmoSpareQuantity = value; }

        public virtual ushort AmmoInMagazineQuantity { get => numericalDefinitions.AmmoInMagazineQuantity; set => numericalDefinitions.AmmoInMagazineQuantity = value; }

        public virtual float ReloadDuration { get => numericalDefinitions.ReloadDuration; set => numericalDefinitions.ReloadDuration = value; }

        public Dictionary<string, AssetDefinitions> AssetDefinitionsMap => throw new NotImplementedException();

        public AssetDefinitions OriginAssetDefinitions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AssetDefinitions NumericalAssetDefinitions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void LoadOrigin()
        {
            origin = originAssetAgent.Load();
        }
        public void LoadNumericalDefinitions()
        {
            numericalDefinitions = numericalDefinitionsAssetAgent.Load();
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
            editor.AmmoOrigin = origin;
            editor.MagazinePosition = numericalDefinitions.MagazinePosition;
            editor.MuzzlePosition = numericalDefinitions.MuzzlePosition;
            editor.LaunchDurationTime = numericalDefinitions.LaunchDurationTime;

            editor.AmmoSpareQuantity = 1;
            editor.AmmoInMagazineQuantity = 1;
            editor.ReloadDuration = numericalDefinitions.ReloadDuration;
            editor.LaunchDelayRange = numericalDefinitions.LaunchDelayRange;
        }

        public void Save()
        {
            originAssetAgent.Save(origin);
            numericalDefinitionsAssetAgent.Save(numericalDefinitions);
            foreach(var e in _subEditors)
            {
                ApplyDefinitionsForSubEditor(e);
                e.Save();
            }
        }

        public void Load()
        {
            LoadOrigin();
            LoadNumericalDefinitions();
            foreach (var e in _subEditors)
                e.Load();
        }
#endif
    }
}
