using Assets.Tests.Scripts.Weapons;
using System;
using System.Collections.Generic;
using Tests.BodyBehaviour.Arm.Animations;
using Tests.Input;
using Tests.States;
using Tests.Weapons;
using UnityEngine;
using UnityEngine.Playables;

namespace Tests.Behaviours.Arm
{

    [RequireComponent(typeof(ArmBehaviourDefinitions))]
    public class ArmCore : StateUComponentBase, IArmBehaviour
    {
        public static implicit operator StateBase<object>(ArmCore core)
        {
            return core._stateMachine;
        }
        [SerializeField]
        Animator _animator;
        [SerializeField]
        WeaponCore _weaponCore;
        [SerializeField]
        HybridInput _input;
        //[SerializeField]
        //GameObject[] _weaponBehaviourObjs;
        [SerializeField]
        MountPoint[] _mountPoints;
        [SerializeField]
        CustomPlayerInput _playerInput;

        internal IArmDefinitions definitions;
        internal IArmAnimationDefinitions animationDefinitions;

        //IArmWeaponBehaviour[] _behaviours;


        internal ArmWeaponSwitching weaponSwitching;
        internal ArmWeaponHoldingBehaviours holdingBehaviours;
        internal ArmWeaponAnimationCore animationCore;
        PlayableStateMachine _stateMachine;

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
        }

        void InitializeSwitchingBehaviour(IArmWeaponDefinitions definitions, MountPoint mountPoint)
        {
            weaponSwitching = new ArmWeaponSwitching(definitions, mountPoint, _weaponCore);
            weaponSwitching.ExitWhenEnd = true;
        }
        void InitializeHoldingBehaviours(IArmWeaponDefinitions definitions)
        {
            var blist = new List<(string, IArmWeaponHoldingBehaviour)>();
            foreach (var od in definitions.Origins)
            {
                if (!_weaponCore.TryGetWeaponOrigin(od.Name, out var origin))
                {
                    Debug.LogWarning(new WeaponOriginNotContainsException(_weaponCore, od.Name));
                    continue;
                }
                var behaviour = origin.GetComponent<IArmWeaponHoldingBehaviour>();
                if (behaviour == null)
                {
                    Debug.LogWarning(new ComponentCantFindException(origin, typeof(IArmWeaponHoldingBehaviour)));
                    continue;
                }
                blist.Add((od.Name, behaviour));
            }
            holdingBehaviours = new ArmWeaponHoldingBehaviours(this.gameObject, blist.ToArray());


            weaponSwitching.SwitchingEvent += (cobj, nobj) =>
            {
                if (cobj != null)
                {
                    var cweapon = cobj.GetComponent<IWeapon>();
                    holdingBehaviours.UnactivateBehaviourBy(cweapon);
                    cobj.SetActive(false);
                }
                if (nobj != null)
                {
                    var nweapon = nobj.GetComponent<IWeapon>();
                    holdingBehaviours.ActivateBehaviourBy(nweapon);
                    nobj.SetActive(true);
                }

                return nobj;
            };
        }
        void InitializeStateMachine()
        {
            _stateMachine = new(this.name);
            _stateMachine.AddState(holdingBehaviours);
            _stateMachine.AddState(weaponSwitching);
            //_stateMachine.AddTransitionFor(holdingBehaviours, () => _input.Supply, weaponSwitching);
            //_stateMachine.AddTransitionFor(weaponSwitching, () => weaponSwitching.NormalizedTime >= 1, holdingBehaviours);
            var length = definitions.Weapon.SwitchingToBehavioursDurationTime;
            _stateMachine.AddTransitionFor(holdingBehaviours, weaponSwitching, length, () => _input.Supply, (s, d, t) =>
            {
                animationCore.SwitchingWeight = t;
            });
            _stateMachine.AddTransitionFor(weaponSwitching, holdingBehaviours, length, (s, d, t) =>
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
            holdingBehaviours.ActivateBehaviourBy(weapon);
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
            //IArmBehaviour.TryBeginAllBehaviours(_behaviours);
        }

        public override void OnExit()
        {
            enabled = false;
            //IArmBehaviour.TryEndAllBehaviours(_behaviours);
        }
        void Update()
        {
            this.OnUpdate();
        }
    }
}
