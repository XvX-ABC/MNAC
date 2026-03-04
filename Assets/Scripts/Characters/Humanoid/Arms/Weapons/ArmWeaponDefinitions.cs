using System;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Arms.Weapons
{
    [Serializable]
    public class ArmWeaponDefinitions : Behaviours.Arms.Weapons.ArmWeaponDefinitions, IArmedArmDefinitions
    {
        [SerializeField]
        ArmedArmBehaviourBase_SO[] _behaviours;

        public IArmedArmBehaviour[] ArmedWeaponBehaviours
        {
            get
            {
                var length = _behaviours.Length;
                var behaviours = new ArmedArmBehaviourBase_SO[_behaviours.Length];
                for (int i = 0; i < length; i++)
                {
                    behaviours[i] = GameObject.Instantiate(_behaviours[i]);
                }
                return behaviours;
            }
        }

        public IArmedArmBehaviour[] GetArmBehaviours(Transform parent)
        {
            throw new NotImplementedException();
        }
    }
}
