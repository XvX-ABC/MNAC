using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Tests.Input;
using Tests.Locomotion;
using Tests.Weapons;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UInput = UnityEngine.Input;

namespace Tests.Behaviours.Arm
{
    internal class ArmedWeaponArmBehaviours : ArmBehaviourPlayableState
    {
        Dictionary<string, IArmedWeaponArmBehaviour> _weaponBehaviours;
        internal IArmedWeaponArmBehaviour[] _activatedBehaviours;
        Action<IWeapon, IArmedWeaponArmBehaviour> _activatedAction;
        Action<IWeapon, IArmedWeaponArmBehaviour> _unactivatedAction;
        public Action<IWeapon, IArmedWeaponArmBehaviour> ActivatedAction { get => _activatedAction; set => _activatedAction = value; }
        public Action<IWeapon, IArmedWeaponArmBehaviour> UnactivatedAction { get => _unactivatedAction; set => _unactivatedAction = value; }

        public ArmedWeaponArmBehaviours(GameObject armObj, params (string name, IArmedWeaponArmBehaviour behaviour)[] weaponBehavioursMapping) : base("behaviors")
        {
            _weaponBehaviours = new();
            foreach (var wwm in weaponBehavioursMapping)
            {
                var n = wwm.name;
                var b = wwm.behaviour;
                _weaponBehaviours.Add(n, b);
            }

        }
        public void ActivateBehaviourBy(IWeapon weapon)
        {
            if (weapon == null)
                throw new ArgumentNullException(nameof(weapon));
            var name = weapon.Name;
            if (name == null || name.Length == 0)
                throw new Exception("The weapon name can't be empty.");
            if (!_weaponBehaviours.TryGetValue(name, out var b))
                throw new CantFindBehaviourByNameException(name);
            if (_activatedBehaviours == null)
            {
                _activatedBehaviours = new IArmedWeaponArmBehaviour[] { b };
            }
            else
            {
                Array.Resize(ref _activatedBehaviours, _activatedBehaviours.Length + 1);
                _activatedBehaviours[^1] = b;
            }
            b.Weapon = weapon;
            _activatedAction?.Invoke(weapon, b);
        }
        public void UnactivateBehaviourBy(IWeapon weapon)
        {
            if (weapon == null)
                throw new ArgumentNullException(nameof(weapon));
            var name = weapon.Name;
            if (name == null || name.Length == 0)
                throw new Exception("The name can't be empty");
            if (!_weaponBehaviours.TryGetValue(name, out var b))
                throw new CantFindBehaviourByNameException(name);
            if (_activatedBehaviours == null)
                return;

            for (int i = 0; i < _activatedBehaviours.Length; i++)
            {
                var ab = _activatedBehaviours[i];
                if (ab == b)
                {
                    var length = _activatedBehaviours.Length;
                    if (length == 1)
                    {
                        _activatedBehaviours = null;
                    }
                    else
                    {
                        if (i != length - 1)
                            Array.Copy(_activatedBehaviours, i + 1, _activatedBehaviours, i, length - i - 1);
                        Array.Resize(ref _activatedBehaviours, length - 1);
                    }
                    break;
                }

            }
            _unactivatedAction?.Invoke(weapon, b);
        }
        public override void OnEnter()
        {
        }
        public override void OnExit()
        {
        }


        public override void OnUpdate()
        {
        }

    }
}
