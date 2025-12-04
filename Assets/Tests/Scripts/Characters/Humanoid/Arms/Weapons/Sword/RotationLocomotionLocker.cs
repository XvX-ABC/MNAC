using System;
using Tests.Behaviours.Arms.Weapons.Sword;
using Tests.Characters.Humanoid.Locomotion;
namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{
    internal class RotationLocomotionLocker : IRotationLocker
    {
        RotationByPlayerLocomotion _rotation;

        public RotationLocomotionLocker(RotationByPlayerLocomotion rotation)
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
