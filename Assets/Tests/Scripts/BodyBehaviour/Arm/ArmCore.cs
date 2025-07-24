using Assets.Tests.Scripts.Weapons;
using System;
using System.Collections.Generic;
using Tests.BodyBehaviour.Arm.Animations;
using Tests.Characters;
using Tests.Input;
using Tests.States;
using Tests.Utilities.MTrees;
using Tests.Weapons;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Playables;

namespace Tests.Behaviours.Arm
{

    [RequireComponent(typeof(ArmBehaviourDefinitions))]
    public class ArmCore : StateUComponentBase, IArmBehaviour, IState
    {
        public static implicit operator StateBase<object>(ArmCore core)
        {
            return core._stateMachine;
        }
        WeaponCore _weaponCore;
        IInput _input;
        [SerializeField]
        MountPoint[] _mountPoints;
        [SerializeField]
        CustomPlayerInput _playerInput;

        internal IArmDefinitions definitions;
        internal IArmAnimationDefinitions animationDefinitions;


        internal ArmWeaponSwitching weaponSwitching;
        internal ArmedWeaponArmBehaviours armedBehaviours;
        internal ArmAnimationCore animationCore;
        Blackboard _blackboard;
        ComponentNode _node;
        PlayableStateMachine _stateMachine;
        public Blackboard Blackboard
        {
            get => _blackboard;
            set
            {
                if (value != null)
                {
                    value.TryReadValue(CharacterBlackboardFields.Input, out _input);
                    value.TryReadValue(CharacterBlackboardFields.WeaponCore, out _weaponCore);
                }
                _blackboard = value;
                _node.UpdateBlackboardForChildren();
            }
        }
        public ICharacterComponentNode Node
        {
            get => _node;
        }
        public MountPoint FindMountPoint(string name)
        {
            foreach (var m in _mountPoints)
            {
                if (m.Name == name)
                    return m;
            }
            return null;
        }

        protected override void Awake()
        {
            base.Awake();
            definitions = GetComponent<ArmBehaviourDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ArmBehaviourDefinitions));
            var weaponDefinitions = definitions.Weapon;
            var weaponMountPoint = FindMountPoint(weaponDefinitions.MountPointName) ?? throw new CantFindMountPointByNameException(weaponDefinitions.MountPointName);

            InitializeSwitchingBehaviour(weaponDefinitions, weaponMountPoint);

            InitializeHoldingBehaviours(weaponDefinitions);

            InitializeStateMachine();

            InitializePlayableGraph();

            InitializeNode();
        }
        void InitializeNode()
        {
            this._node = new(this.ID, this);
            _node.AddChild(weaponSwitching.Node);
            _node.AddChild(armedBehaviours.Node);
        }

        void InitializeSwitchingBehaviour(IArmWeaponDefinitions definitions, MountPoint mountPoint)
        {
            weaponSwitching = new ArmWeaponSwitching(definitions, mountPoint, _weaponCore);
            weaponSwitching.ExitWhenEnd = true;
        }
        void InitializeHoldingBehaviours(IArmWeaponDefinitions definitions)
        {
            var blist = new List<(string, IArmedWeaponArmBehaviour)>();
            foreach (var od in definitions.Origins)
            {
                if (!_weaponCore.TryGetWeaponOrigin(od.Name, out var origin))
                {
                    Debug.LogWarning(new WeaponOriginNotContainsException(_weaponCore, od.Name));
                    continue;
                }
                var behaviour = origin.GetComponent<IArmedWeaponArmBehaviour>();
                if (behaviour == null)
                {
                    Debug.LogWarning(new ComponentCantFindException(origin, typeof(IArmedWeaponArmBehaviour)));
                    continue;
                }
                blist.Add((od.Name, behaviour));
            }
            armedBehaviours = new ArmedWeaponArmBehaviours(this.gameObject, blist.ToArray());


            weaponSwitching.SwitchingEvent += (cobj, nobj) =>
            {
                if (cobj != null)
                {
                    var cweapon = cobj.GetComponent<IWeapon>();
                    armedBehaviours.UnactivateBehaviourBy(cweapon);
                    cobj.SetActive(false);
                }
                if (nobj != null)
                {
                    var nweapon = nobj.GetComponent<IWeapon>();
                    armedBehaviours.ActivateBehaviourBy(nweapon);
                    nobj.SetActive(true);
                }

                return nobj;
            };
        }
        void InitializeStateMachine()
        {
            _stateMachine = new(this.name);
            _stateMachine.AddState(armedBehaviours);
            _stateMachine.AddState(weaponSwitching);
            var length = definitions.Weapon.SwitchingToBehavioursDurationTime;
            _stateMachine.AddTransitionFor(armedBehaviours, weaponSwitching, length, () => _input.Supply, (s, d, t) =>
            {
                animationCore.SwitchingWeight = t;
            });
            _stateMachine.AddTransitionFor(weaponSwitching, armedBehaviours, length, (s, d, t) =>
            {
                animationCore.SwitchingWeight = 1 - t;
            });
        }
        void InitializePlayableGraph()
        {
            var graph = PlayableGraph.Create("Arm_Animation");
            animationCore = new(this, graph);
        }
        private void Start()
        {
            var mp = FindMountPoint(definitions.Weapon.MountPointName);
            var n = definitions.Weapon.Origins[0].Name;
            SetDefaultWeapon(mp, n, _weaponCore);
            this.OnEnter();
        }

        void SetDefaultWeapon(MountPoint weaponMountPoint, string weaponName, WeaponCore weaponCore)
        {
            if (!weaponCore.TryGetWeaponObj(weaponName, out var obj))
                throw new Exception();
            var weapon = obj.GetComponent<IWeapon>();
            armedBehaviours.ActivateBehaviourBy(weapon);
            weaponMountPoint.LoadObj = obj;
        }

        public override void OnUpdate()
        {
            _stateMachine.OnUpdate();
        }
        public void OnAnimatorIK(int layerIndex)
        {

        }

        public override void OnEnter()
        {
            enabled = true;
        }

        public override void OnExit()
        {
            enabled = false;
        }
        void Update()
        {
            this.OnUpdate();
        }
    }
}
