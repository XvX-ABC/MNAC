using Assets.Tests.Scripts.Weapons;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Tests.BodyBehaviour.Arm.Animations;
using Tests.Characters;
using Tests.Input;
using Tests.Locomotion;
using Tests.Weapons;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UInput = UnityEngine.Input;

namespace Tests.Behaviours.Arm
{
    internal class ArmedWeaponArmBehavioursController : ArmBehaviourPlayableState
    {
        internal IArmedWeaponArmBehaviour[] behaviours;

        internal Dictionary<string, IArmedWeaponArmBehaviour> weaponBehavioursMapping;
        internal IArmedWeaponArmBehaviour[] activatedBehaviours;

        Action<IWeapon, IArmedWeaponArmBehaviour> _activatedAction;
        Action<IWeapon, IArmedWeaponArmBehaviour> _unactivatedAction;

        ArmAnimationCore_New _animationCore;

        public Action<IWeapon, IArmedWeaponArmBehaviour> ActivatedAction { get => _activatedAction; set => _activatedAction = value; }
        public Action<IWeapon, IArmedWeaponArmBehaviour> UnactivatedAction { get => _unactivatedAction; set => _unactivatedAction = value; }
        public override bool Enabled
        {
            get => activatedBehaviours == null ? false : activatedBehaviours[0].Enabled;
            set
            {
                if (activatedBehaviours == null)
                    return;
                activatedBehaviours[0].Enabled = value;
            }
        }

        internal ArmAnimationCore_New AnimationCore { get => _animationCore; set => _animationCore = value; }

        public ArmedWeaponArmBehavioursController(WeaponCore weaponCore, IArmWeaponDefinitions definitions, params IArmedWeaponArmBehaviour[] behaviours) : base("behaviours")
        {
            if (weaponCore == null)
                throw new ArgumentNullException(nameof(weaponCore));
            this.behaviours = behaviours;
            weaponBehavioursMapping = new();

            foreach (var od in definitions.Origins)
            {
                if (!weaponCore.TryGetWeaponDescription(od.Name, out var description))
                {
                    Debug.LogWarning(new WeaponNotContainsException(weaponCore, od.Name));
                    continue;
                }
                var type = description.Type;
                var b = behaviours.First(b => b.Type == type);
                weaponBehavioursMapping.Add(od.Name, b);
            }
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            foreach (var v in behaviours)
            {
                this.node.AddChild(v.Node);
            }
        }
        public void ActivateBehaviourBy(IWeapon weapon)
        {
            if (weapon == null)
                throw new ArgumentNullException(nameof(weapon));
            var name = weapon.Name;
            Debug.Log("Activated  weapon name: " + weapon.Name);
            if (name == null || name.Length == 0)
                throw new Exception("The weapon name can't be empty.");
            if (!weaponBehavioursMapping.TryGetValue(name, out var b))
                throw new CantFindBehaviourByNameException(name);
            if (activatedBehaviours == null)
            {
                activatedBehaviours = new IArmedWeaponArmBehaviour[] { b };
            }
            else
            {
                Array.Resize(ref activatedBehaviours, activatedBehaviours.Length + 1);
                activatedBehaviours[^1] = b;
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
            if (!weaponBehavioursMapping.TryGetValue(name, out var b))
                throw new CantFindBehaviourByNameException(name);
            if (activatedBehaviours == null)
                return;

            for (int i = 0; i < activatedBehaviours.Length; i++)
            {
                var ab = activatedBehaviours[i];
                if (ab == b)
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
            _unactivatedAction?.Invoke(weapon, b);
        }
        public override void OnEnter()
        {
            base.OnEnter();
            if (_animationCore != null)
                _animationCore.StatusNum = 1;
        }
        public override void OnExit()
        {
            base.OnExit();
            if (_animationCore != null)
                _animationCore.StatusNum = 2;
        }
    }
}
