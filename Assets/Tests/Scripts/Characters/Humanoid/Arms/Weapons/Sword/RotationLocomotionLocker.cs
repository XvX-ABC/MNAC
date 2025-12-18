using System;
using Tests.Behaviours.Arms.Weapons.Sword;
using Tests.Characters.Humanoid.Locomotion;
using UnityEngine;
namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{
    internal class RotationLocomotionLocker : IRotationLocker
    {
        RotationLocomotionBase _rotation;

        public RotationLocomotionLocker(RotationLocomotionBase rotation)
        {
            _rotation = rotation ?? throw new ArgumentNullException(nameof(rotation));
        }

        public bool Locked => !_rotation.Enabled;

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
