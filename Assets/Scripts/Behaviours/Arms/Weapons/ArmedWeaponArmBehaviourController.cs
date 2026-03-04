using System;
using System.Collections.Generic;
using System.Linq;
using MNAC.Weapons;
using MNAC.Weapons;
using Debug = UnityEngine.Debug;

namespace MNAC.Behaviours.Arms.Weapons
{

    internal class ArmedArmBehaviourController<T> : IArmedArmBehavioursController<T> where T : IArmedArmBehaviour
    {
        internal T[] behavioursCache;

        internal Dictionary<string, T> weaponBehavioursMapping;
        internal T activatedBehaviour;

        Action<IWeapon, T> _activatedAction;
        Action<IWeapon, T> _unactivatedAction;

        public Action<IWeapon, T> ActivatedAction { get => _activatedAction; set => _activatedAction = value; }
        public Action<IWeapon, T> UnactivatedAction { get => _unactivatedAction; set => _unactivatedAction = value; }
        public Func<bool> ActivationTrigger { get => ActivateBehaviour; }
        public Func<bool> UnactivationTrigger { get => UnactivateBehaviour; }
        internal T currentActivatedBehaviour
        {
            get
            {
                if (activatedBehaviour == null)
                    return default;
                return activatedBehaviour;
            }
        }

        IReadOnlyDictionary<string, T> IArmedArmBehavioursController<T>.Behaviours => weaponBehavioursMapping;


        public ArmedArmBehaviourController(IArmedArmDefinitions definitions, params T[] behaviours)
        {
            behaviours = behaviours.Where(b => b != null).ToArray();
            this.behavioursCache = behaviours;
            foreach (var b in behavioursCache)
                b.Activated = false;
        }
        public void ActivateBehaviourBy(IWeapon weapon)
        {
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
        public void UnactivateBehaviourBy(IWeapon weapon)
        {
            if (weapon == null)
                throw new ArgumentNullException(nameof(weapon));
            var type = weapon.Type;
            if (activatedBehaviour == null || activatedBehaviour.Type != type)
                return;
            var b = activatedBehaviour;


            b.Activated = false;
            _unactivatedAction?.Invoke(weapon, b);
        }
        bool ActivateBehaviour()
        {
            return activatedBehaviour == null ? false : activatedBehaviour.ActivationTrigger();
        }
        bool UnactivateBehaviour()
        {
            return activatedBehaviour == null ? false : activatedBehaviour.UnactivationTrigger();
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
