using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Behaviours.Arms.Animations;
using Tests.Characters;
using Tests.Characters.Arms.Animations;
using Tests.States;
using Tests.Weapons;
using ArmAnimationCore_Obsolete = Tests.Behaviours.Arms.Animations.ArmAnimationCore_Obsolete;
using Debug = UnityEngine.Debug;

namespace Tests.Behaviours.Arms.Weapons
{
    [Obsolete]
    internal class ArmedWeaponArmBehavioursController_Obsolete : ArmPlayableState_Obsolete
    {
        internal IArmedWeaponArmBehaviour_Obsolete[] behaviours;

        internal Dictionary<string, IArmedWeaponArmBehaviour_Obsolete> weaponBehavioursMapping;
        internal IArmedWeaponArmBehaviour_Obsolete[] activatedBehaviours;

        Action<IWeapon, IArmedWeaponArmBehaviour_Obsolete> _activatedAction;
        Action<IWeapon, IArmedWeaponArmBehaviour_Obsolete> _unactivatedAction;

        ArmAnimationCore_Obsolete _animationCore;

        public Action<IWeapon, IArmedWeaponArmBehaviour_Obsolete> ActivatedAction { get => _activatedAction; set => _activatedAction = value; }
        public Action<IWeapon, IArmedWeaponArmBehaviour_Obsolete> UnactivatedAction { get => _unactivatedAction; set => _unactivatedAction = value; }
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
        internal ArmAnimationCore_Obsolete AnimationCore { get => _animationCore; set => _animationCore = value; }
        public IReadOnlyDictionary<string, IArmedWeaponArmBehaviour_Obsolete> Behaviours { get => weaponBehavioursMapping; }
        public Func<bool> EntryFunc { get => EnterBehaviour; }
        public Func<bool> ExitFunc { get => ExitBehaviour; }
        public ArmedWeaponArmBehavioursController_Obsolete(WeaponCore weaponCore, IArmWeaponDefinitions definitions, params IArmedWeaponArmBehaviour_Obsolete[] behaviours) : base("armed_weapon")
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
                node.AddChild(v.Node);
            }
        }
        public void ActivateBehaviourBy(IWeapon weapon)
        {
            if (weapon == null)
                throw new ArgumentNullException(nameof(weapon));
            var name = weapon.Name;
            if (name == null || name.Length == 0)
                throw new Exception("The weapon name can't be empty.");
            if (!weaponBehavioursMapping.TryGetValue(name, out var b))
                throw new CantFindBehaviourByNameException(name);
            if (activatedBehaviours == null)
            {
                activatedBehaviours = new IArmedWeaponArmBehaviour_Obsolete[] { b };
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
        bool EnterBehaviour()
        {
            return activatedBehaviours == null ? false : activatedBehaviours[0].EntryFunc();
        }
        bool ExitBehaviour()
        {
            return activatedBehaviours == null ? false : activatedBehaviours[0].ExitFunc();
        }
        public override void OnEnter()
        {
            base.OnEnter();
            if (_animationCore != null)
                _animationCore.StatusNum = 1;
            if (activatedBehaviours != null)
            {
                var b = activatedBehaviours[0];
                b.StateNode.OnEnter();
            }
        }
        public override void OnExit()
        {
            if (activatedBehaviours != null)
            {
                var b = activatedBehaviours[0];
                b.StateNode.OnExit();

                base.OnExit();
            }
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (activatedBehaviours != null)
            {
                var b = activatedBehaviours[0];
                b.StateNode.OnUpdate();
            }
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            if (activatedBehaviours != null)
            {
                var b = activatedBehaviours[0];
                b.StateNode.FromPreviousStateTransitionBegin(currentTransition);
            }
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            if (activatedBehaviours != null)
            {
                var b = activatedBehaviours[0];
                b.StateNode.FromPreviousStateTransitionRunning(currentTransition);
            }
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            if (activatedBehaviours != null)
            {
                var b = activatedBehaviours[0];
                b.StateNode.ToNextStateTransitionRunning(currentTransition);
            }
        }
    }
}
