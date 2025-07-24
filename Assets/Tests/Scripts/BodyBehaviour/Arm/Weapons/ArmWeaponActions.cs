using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using Tests.BT;
using Tests.Input;
using Tests.Weapons;
using UnityEngine;

namespace Tests.Behaviours.Arm
{
    [Obsolete]
    public class ArmWeaponActions : Sequencer, IArmAction
    {
        Dictionary<string, IArmWeaponAction> _weaponActions;
        IArmWeaponAction[] _activatedActions;
        protected IArmWeaponAction[] activatedActions
        {
            get => _activatedActions;
            set
            {
                _activatedActions = value;
                children.TrimExcess();
                children.Clear();
                children.AddRange(_activatedActions);
            }
        }
        IInput _input;
        public IInput Input
        {
            set
            {
                if (_activatedActions != null)
                    foreach (var a in _activatedActions)
                        a.Input = value;
                _input = value;
            }
        }
        public ArmWeaponActions(params (string name, IArmWeaponAction actions)[] weaponActions)
        {
            _weaponActions = new();
            foreach (var wa in weaponActions)
            {
                var n = wa.name;
                var a = wa.actions;
                _weaponActions.Add(n, a);
            }
        }

        public void ActivateActionBy(IWeapon weapon)
        {
            if (weapon == null)
                throw new ArgumentNullException(nameof(weapon));
            var name = weapon.Name;
            if (name == null || name.Length == 0)
                throw new Exception("The weapon name can't is empty.");
            if (!_weaponActions.TryGetValue(name, out var a))
            {
                Debug.LogWarning($"Can't find weapon behaviour by the name '{name}'");
                return;
            }
            if (activatedActions == null)
                activatedActions = new IArmWeaponAction[] { a };
            else
            {
                Array.Resize(ref _activatedActions, activatedActions.Length + 1);
                activatedActions[^1] = a;
            }
            a.Weapon = weapon;
            a.Input = _input;
        }
        public void UnactivateActionBy(IWeapon weapon)
        {
            if (weapon == null)
                throw new ArgumentNullException(nameof(weapon));
            var name = weapon.Name;
            if (name == null || name.Length == 0)
                throw new Exception("The name can't be empty");
            if (!_weaponActions.TryGetValue(name, out var a))
            {
                Debug.LogWarning($"Can't find weapon behaviour by the name '{name}'");
                return;
            }
            if (activatedActions == null)
                return;
            for (int i = 0; i < activatedActions.Length; i++)
            {
                var aa = activatedActions[i];
                if (aa == a)
                {
                    var length = activatedActions.Length;
                    if (i != length - 1)
                        Array.Copy(activatedActions, i + 1, activatedActions, i, length - i - 1);
                    Array.Resize(ref _activatedActions, length - 1);
                    break;
                }
            }
        }
    }
}
