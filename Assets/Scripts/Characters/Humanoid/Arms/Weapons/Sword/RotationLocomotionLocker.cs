using System;
using MNAC.Behaviours.Arms.Weapons.Sword;
using MNAC.Characters.Humanoid.Locomotion;
using UnityEngine;
namespace MNAC.Characters.Humanoid.Arms.Weapons.Sword
{
    internal class RotationLocomotionLocker : IRotationLocker
    {
        RotationLocomotionBase _rotation;

        public RotationLocomotionLocker(RotationLocomotionBase rotation)
        {
            _rotation = rotation ?? throw new ArgumentNullException(nameof(rotation));
        }

        public bool IsLocked => !_rotation.Enabled;

        public void Lock()
        {
            _rotation.Enabled = false;
        }

        public void UnLock()
        {
            _rotation.Enabled = true;
        }
    }
}
