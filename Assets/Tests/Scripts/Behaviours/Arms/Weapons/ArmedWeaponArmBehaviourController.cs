using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Weapons;
using Debug = UnityEngine.Debug;

namespace Tests.Behaviours.Arms.Weapons
{

    internal class ArmedWeaponArmBehaviourController<T> : IArmedWeaponArmBehavioursController<T> where T : IArmedWeaponArmBehaviour
    {
        internal T[] behavioursCache;

        internal Dictionary<string, T> weaponBehavioursMapping;
        internal T[] activatedBehaviours;

        Action<IWeapon, T> _activatedAction;
        Action<IWeapon, T> _unactivatedAction;

        public Action<IWeapon, T> ActivatedAction { get => _activatedAction; set => _activatedAction = value; }
        public Action<IWeapon, T> UnactivatedAction { get => _unactivatedAction; set => _unactivatedAction = value; }
        public Func<bool> EntryFunc { get => EnterBehaviour; }
        public Func<bool> ExitFunc { get => ExitBehaviour; }
        internal T currentActivatedBehaviour
        {
            get
            {
                if (activatedBehaviours == null)
                    return default;
                return activatedBehaviours[0];
            }
        }

        IReadOnlyDictionary<string, T> IArmedWeaponArmBehavioursController<T>.Behaviours => weaponBehavioursMapping;

        public ArmedWeaponArmBehaviourController(WeaponCore weaponCore, IArmedWeaponArmDefinitions definitions, params T[] behaviours)
        {
            if (weaponCore == null)
                throw new ArgumentNullException(nameof(weaponCore));
            this.behavioursCache = behaviours;
            weaponBehavioursMapping = new();

            foreach (var od in definitions.Origins)
            {
                var name = od.Name;
                if (!weaponCore.TryGetWeaponDescription(name, out var description))
                {
                    Debug.LogWarning(new WeaponNotContainsException(weaponCore, name));
                    continue;
                }
                var type = description.Type;
                var b = behaviours.First(b => b.Type == type);
                //b.Activated = false;
                if (weaponBehavioursMapping.ContainsKey(name))
                    weaponBehavioursMapping[name] = b;
                else
                    weaponBehavioursMapping.Add(name, b);
            }
        }
        public void ActivateBehaviourBy(IWeapon weapon)
        {
            if (weapon == null)
                throw new ArgumentNullException(nameof(weapon));
            var name = weapon.Name;
            if (name == null || name.Length == 0)
                throw new Exception("The weapon name can'IArmedWeaponArmBehaviour_New be empty.");
            if (!weaponBehavioursMapping.TryGetValue(name, out var b))
                throw new CantFindBehaviourByNameException(name);
            if (activatedBehaviours == null)
            {
                activatedBehaviours = new T[] { b };
            }
            else
            {
                Array.Resize(ref activatedBehaviours, activatedBehaviours.Length + 1);
                activatedBehaviours[^1] = b;
            }
            b.Weapon = weapon;
            b.Activated = true;
            _activatedAction?.Invoke(weapon, b);
        }
        public void UnactivateBehaviourBy(IWeapon weapon)
        {
            if (weapon == null)
                throw new ArgumentNullException(nameof(weapon));
            var name = weapon.Name;
            if (name == null || name.Length == 0)
                throw new Exception("The name can'IArmedWeaponArmBehaviour_New be empty");
            if (!weaponBehavioursMapping.TryGetValue(name, out var b))
                throw new CantFindBehaviourByNameException(name);
            if (activatedBehaviours == null)
                return;



            for (int i = 0; i < activatedBehaviours.Length; i++)
            {
                var ab = activatedBehaviours[i];
                if (ab.Equals(b))
                {
                    var length = activatedBehaviours.Length;
                    if (length == 1)
                    {
                        activatedBehaviours = null;
                    }
                    else
                    {
                        if (i != length - 1)
                            Array.Copy(activatedBehaviours, i + 1, activatedBehaviours, i, length - i - 1);
                        Array.Resize(ref activatedBehaviours, length - 1);
                    }
                    break;
                }

            }
            b.Activated = false;
            _unactivatedAction?.Invoke(weapon, b);
        }
        bool EnterBehaviour()
        {
            return activatedBehaviours == null ? false : activatedBehaviours[0].EntryFunc();
        }
        bool ExitBehaviour()
        {
            return activatedBehaviours == null ? false : activatedBehaviours[0].ExitFunc();
        }

    }
}
