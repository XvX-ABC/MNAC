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
        ArmedWeaponArmBehaviourBase_SO[] _behaviours;

        public IArmedWeaponArmBehaviour[] ArmedWeaponBehaviours
        {
            get
            {
                var length = _behaviours.Length;
                var behaviours = new ArmedWeaponArmBehaviourBase_SO[_behaviours.Length];
                for (int i = 0; i < length; i++)
                {
                    behaviours[i] = GameObject.Instantiate(_behaviours[i]);
                }
                return behaviours;
            }
        }

        public IArmedWeaponArmBehaviour[] GetArmBehaviours(Transform parent)
        {
            throw new NotImplementedException();
        }
    }
}
