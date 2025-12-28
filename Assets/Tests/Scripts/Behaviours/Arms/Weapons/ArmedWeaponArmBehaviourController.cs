using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Weapons;
using Tests.Weapons_New;
using Debug = UnityEngine.Debug;

namespace Tests.Behaviours.Arms.Weapons
{

    internal class ArmedWeaponArmBehaviourController<T> : IArmedWeaponArmBehavioursController<T> where T : IArmedWeaponArmBehaviour
    {
        internal T[] behavioursCache;

        internal Dictionary<string, T> weaponBehavioursMapping;
        internal T activatedBehaviour;

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
                if (activatedBehaviour == null)
                    return default;
                return activatedBehaviour;
            }
        }

        IReadOnlyDictionary<string, T> IArmedWeaponArmBehavioursController<T>.Behaviours => weaponBehavioursMapping;

        [Obsolete]
        public ArmedWeaponArmBehaviourController(Weapons_New.WeaponCore_Obsolete weaponCore, IArmedWeaponArmDefinitions definitions, params T[] behaviours)
        {
            if (weaponCore == null)
                throw new ArgumentNullException(nameof(weaponCore));
            behaviours = behaviours.Where(b => b != null).ToArray();
            this.behavioursCache = behaviours;
            foreach (var b in behavioursCache)
                b.Activated = false;
            //weaponBehavioursMapping = new();

            //foreach (var od in definitions.Origins)
            //{
            //    var name = od.Name;
            //    if (!weaponCore.TryGetWeaponDescription(name, out var description))
            //    {
            //        Debug.LogWarning(new WeaponNotContainsException(weaponCore, name));
            //        continue;
            //    }
            //    var type = description.Type;
            //    var b = behaviours.FirstOrDefault(b => b.Type == type);
            //    if (b == null)
            //        continue;
            //    b.Activated = false;
            //    if (weaponBehavioursMapping.ContainsKey(name))
            //        weaponBehavioursMapping[name] = b;
            //    else
            //        weaponBehavioursMapping.Add(name, b);
            //}
        }

        public ArmedWeaponArmBehaviourController(IArmedWeaponArmDefinitions definitions, params T[] behaviours)
        {
            behaviours = behaviours.Where(b => b != null).ToArray();
            this.behavioursCache = behaviours;
            foreach (var b in behavioursCache)
                b.Activated = false;
            //weaponBehavioursMapping = new();

            //foreach (var od in definitions.Origins)
            //{
            //    var name = od.Name;
            //    if (!weaponCore.TryGetWeaponDescription(name, out var description))
            //    {
            //        Debug.LogWarning(new WeaponNotContainsException(weaponCore, name));
            //        continue;
            //    }
            //    var type = description.Type;
            //    var b = behaviours.FirstOrDefault(b => b.Type == type);
            //    if (b == null)
            //        continue;
            //    b.Activated = false;
            //    if (weaponBehavioursMapping.ContainsKey(name))
            //        weaponBehavioursMapping[name] = b;
            //    else
            //        weaponBehavioursMapping.Add(name, b);
            //}
        }
        [Obsolete]
        public void ActivateBehaviourBy_Obsolete(IWeapon weapon)
        {
            if (weapon == null)
                throw new ArgumentNullException(nameof(weapon));
            var name = weapon.Name;
            if (name == null || name.Length == 0)
                throw new Exception("The weapon name can'IArmedWeaponArmBehaviour_New be empty.");
            if (!weaponBehavioursMapping.TryGetValue(name, out var b))
                throw new CantFindBehaviourByNameException(name);
            //if (activatedBehaviour == null)
            //{
            //    activatedBehaviour = new T[] { b };
            //}
            //else
            //{
            //    Array.Resize(ref activatedBehaviour, activatedBehaviour.Length + 1);
            //    activatedBehaviour[^1] = b;
            //}
            b.Weapon = weapon;
            b.Activated = true;
            _activatedAction?.Invoke(weapon, b);
        }
        public void ActivateBehaviourBy(IWeapon weapon)
        {
            Debug.Log("activated weapon name: " + weapon.Name);
            if (weapon == null)
                throw new ArgumentNullException(nameof(weapon));
            var type = weapon.Type;
            var b = behavioursCache.FirstOrDefault(b => b.Type == type);
            if (b == null)
                return;
            //if (!weaponBehavioursMapping.TryGetValue(name, out var b))
            //    throw new CantFindBehaviourByNameException(name);
            activatedBehaviour = b;
            b.Weapon = weapon;
            b.Activated = true;
            _activatedAction?.Invoke(weapon, b);
        }
        [Obsolete]
        public void UnactivateBehaviourBy_Obsolete(IWeapon weapon)
        {
            if (weapon == null)
                throw new ArgumentNullException(nameof(weapon));
            var name = weapon.Name;
            if (name == null || name.Length == 0)
                throw new Exception("The name can'IArmedWeaponArmBehaviour_New be empty");
            if (!weaponBehavioursMapping.TryGetValue(name, out var b))
                throw new CantFindBehaviourByNameException(name);
            if (activatedBehaviour == null)
                return;



            //for (int i = 0; i < activatedBehaviour.Length; i++)
            //{
            //    var ab = activatedBehaviour[i];
            //    if (ab.Equals(b))
            //    {
            //        var length = activatedBehaviour.Length;
            //        if (length == 1)
            //        {
            //            activatedBehaviour = null;
            //        }
            //        else
            //        {
            //            if (i != length - 1)
            //                Array.Copy(activatedBehaviour, i + 1, activatedBehaviour, i, length - i - 1);
            //            Array.Resize(ref activatedBehaviour, length - 1);
            //        }
            //        break;
            //    }

            //}
            b.Activated = false;
            _unactivatedAction?.Invoke(weapon, b);
        }
        public void UnactivateBehaviourBy(IWeapon weapon)
        {
            if (weapon == null)
                throw new ArgumentNullException(nameof(weapon));
            //var name = weapon.Name;
            //if (name == null || name.Length == 0)
            //    throw new Exception("The name can'IArmedWeaponArmBehaviour_New be empty");
            //if (!weaponBehavioursMapping.TryGetValue(name, out var b))
            //    throw new CantFindBehaviourByNameException(name);
            var type = weapon.Type;
            if (activatedBehaviour == null || activatedBehaviour.Type != type)
                return;
            var b = activatedBehaviour;


            b.Activated = false;
            _unactivatedAction?.Invoke(weapon, b);
        }
        bool EnterBehaviour()
        {
            return activatedBehaviour == null ? false : activatedBehaviour.EntryFunc();
        }
        bool ExitBehaviour()
        {
            return activatedBehaviour == null ? false : activatedBehaviour.ExitFunc();
        }
        public void Update()
        {
            if (activatedBehaviour != null)
                activatedBehaviour.BehaviourOnUpdate();
        }
        public void LateUpdate()
        {
            if (activatedBehaviour != null)
                activatedBehaviour.BehaviourOnLateUpdate();
        }
        public void FixedUpdate()
        {
            if (activatedBehaviour != null)
                activatedBehaviour.BehaviourOnFixedUpdate();
        }
    }
}
