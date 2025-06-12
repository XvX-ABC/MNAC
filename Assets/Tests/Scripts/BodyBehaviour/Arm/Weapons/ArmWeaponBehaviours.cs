using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Tests.Input;
using Tests.Locomotion;
using Tests.Weapons;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Tests.BodyBehaviour.Arm
{
    public class ArmWeaponBehaviours : IArmBehaviour
    {
        Dictionary<string, IArmWeaponBehaviour> _weaponBehaviours;
        IArmWeaponBehaviour[] _activatedBehaviours;
        IInput _input;
        public IInput Input
        {
            set
            {
                if (_activatedBehaviours != null)
                    foreach (var b in _activatedBehaviours)
                        b.Input = value;
                _input = value;
            }
        }
        public bool Continuing
        {
            get => IArmBehaviour.AnyBehaviourIsContinuing(_activatedBehaviours);
        }
        public ArmWeaponBehaviours(params (string name, IArmWeaponBehaviour behaviour)[] weaponBehavioursMapping)
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
                _activatedBehaviours = new IArmWeaponBehaviour[] { b };
            }
            else
            {
                Array.Resize(ref _activatedBehaviours, _activatedBehaviours.Length + 1);
                _activatedBehaviours[^1] = b;
            }
            b.Weapon = weapon;
            b.Input = _input;
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
                    if (i != length - 1)
                        Array.Copy(_activatedBehaviours, i + 1, _activatedBehaviours, i, length - i - 1);
                    Array.Resize(ref _activatedBehaviours, length - 1);
                    break;
                }

            }
        }
        public bool BStart()
        {
            return IArmBehaviour.TryBeginAllBehaviours(_activatedBehaviours);
        }
        public bool BEnd()
        {
            return IArmBehaviour.TryEndAllBehaviours(_activatedBehaviours);
        }

        public void OnAnimatorIK(int layerIndex)
        {
            foreach (var b in _activatedBehaviours)
                b.OnAnimatorIK(layerIndex);
        }

        public void OnUpdate()
        {
            foreach (var b in _activatedBehaviours)
                b.OnUpdate();
        }

    }
}
