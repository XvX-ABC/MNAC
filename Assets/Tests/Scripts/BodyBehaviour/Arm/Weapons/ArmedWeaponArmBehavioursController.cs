using Assets.Tests.Scripts.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using Tests.BodyBehaviour.Arm.Animations;
using Tests.Characters;
using Tests.States;
using Tests.Weapons;
using Debug = UnityEngine.Debug;

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
            get => activatedBehaviours == null ? false : ((IState<object>)activatedBehaviours[0]).Enabled;
            set
            {
                if (activatedBehaviours == null)
                    return;
                ((IState<object>)activatedBehaviours[0]).Enabled = value;
            }
        }
        internal ArmAnimationCore_New AnimationCore { get => _animationCore; set => _animationCore = value; }
        public byte StatusNum
        {
            get => activatedBehaviours == null ? (byte)0 : activatedBehaviours[0].StatusNum;
        }

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
                b.Enabled = false;
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
            b.Enabled = true;
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
            b.Enabled = false;
            _unactivatedAction?.Invoke(weapon, b);
        }
        public override void OnEnter()
        {
            base.OnEnter();
            if (_animationCore != null)
                _animationCore.StatusNum = 1;
            if (activatedBehaviours != null)
            {
                var b = activatedBehaviours[0];
                b.State.OnEnter();
            }
        }
        public override void OnExit()
        {
            base.OnExit();
            if (activatedBehaviours != null)
            {
                var b = activatedBehaviours[0];
                b.State.OnExit();
            }
        }
        public override void TransitionRunningWhichOfPreviousState(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.TransitionRunningWhichOfPreviousState(currentTransition);
            if (activatedBehaviours != null)
            {
                var b = activatedBehaviours[0];
                b.State.TransitionRunningWhichOfPreviousState(currentTransition);
            }
        }
        public override void TransitionRunningWhichToNextState(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.TransitionRunningWhichToNextState(currentTransition);
            if (activatedBehaviours != null)
            {
                var b = activatedBehaviours[0];
                b.State.TransitionRunningWhichToNextState(currentTransition);
            }
        }
    }
}
