using NUnit.Framework;
using System;
using System.Collections.Generic;
using Tests.Extensions;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons
{
    [Serializable]
    public class ArmWeaponDefinitions : Behaviours.Arms.Weapons.ArmWeaponDefinitions, IArmedWeaponArmDefinitions
    {
        [SerializeField]
        ArmedWeaponArmBehaviourPrefabLoader[] _behaviourLoaders;
        IArmedWeaponArmBehaviour[] _behaviours;
        public IArmedWeaponArmBehaviour[] ArmedWeaponBehaviours
        {
            get
            {
                _behaviours = null;
                foreach (var loader in _behaviourLoaders)
                {
                    loader.Load();
                    _behaviours = _behaviours.Append(loader.Resource);
                }
                return _behaviours;
            }
        }

        public IArmedWeaponArmBehaviour[] GetArmBehaviours(Transform parent)
        {
            var behaviours = new List<IArmedWeaponArmBehaviour>();
            foreach (var loader in _behaviourLoaders)
            {
                loader.Parent = parent;
                loader.Load();
                behaviours.Add(loader.Resource);
            }
            return behaviours.ToArray();
        }
    }
}
