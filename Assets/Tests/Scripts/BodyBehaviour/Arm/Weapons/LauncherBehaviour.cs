using BehaviorDesigner.Runtime;
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Behaviours.Arm;
using Tests.Behaviours.Arm.Weapons;
using Tests.BT;
using Tests.Input;
using Tests.Weapons;
using Tests.Weapons.Launcher;
using UnityEditorInternal;
using UnityEngine;

namespace Tests.Behaviours.Arm.Weapons
{
    public abstract class ArmWeaponHoldingBehaviourBase : MonoBehaviour, IArmWeaponHoldingBehaviour
    {
        [SerializeField]
        protected WeaponType type;
        protected IInput input;
        protected Animator animator;
        public WeaponType Type { get => type; }
        public abstract IWeapon Weapon { get; set; }
        public abstract IArmWeaponHoldingBehavioursAnimator Animator { get; set; }
        public IInput Input { set => input = value; }
        public virtual string Name { get => this.name; }
        public bool Enabled { get => this.enabled; set => this.enabled = value; }
        public object Context { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Guid ID => throw new NotImplementedException();

        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnUpdate();
    }
}
[RequireComponent(typeof(BehaviorTree))]
public class LauncherBehaviour : ArmWeaponHoldingBehaviourBase
{
    ILauncher _launcher;
    BehaviorTree _bTree;
    ArmAimer _aimer;
    public override IWeapon Weapon
    {
        get => _launcher;
        set
        {
            if (value is ILauncher launcher)
            {
                _launcher = launcher;
            }
            else
                throw new Exception("Weapon");
        }
    }
    public override IArmWeaponHoldingBehavioursAnimator Animator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    private void Awake()
    {
        _bTree = GetComponent<BehaviorTree>();
    }
    public override void OnEnter()
    {
        throw new NotImplementedException();
    }

    public override void OnExit()
    {
        throw new NotImplementedException();
    }

    public override void OnUpdate()
    {
        throw new NotImplementedException();
    }
}
//    [RequireComponent(typeof(AimIK))]
//    public class LauncherBehaviour : MonoBehaviour, IArmWeaponBehaviour, IAimer, IArmWeaponAction
//    {
//        ArmAimer _aimer;
//        [SerializeField]
//        ArmAimingAndReloadTransition _transition;
//        [SerializeField]
//        ArmReloadAnimation _reloadAnimation;
//        ILauncher _launcher;
//        IInput _input;

//        Selector _selector;
//        class Selector : Tests.BT.Selector
//        {
//            public Selector(params IArmAction[] actions)
//            {
//                children.AddRange(actions);
//            }
//        }
//        public WeaponType Type => WeaponType.Launcher;

//        public IWeapon Weapon
//        {
//            get => _launcher;

//            set
//            {
//                if (value is ILauncher launcher)
//                {
//                    _launcher = launcher;
//                    _transition.Launcher = launcher;
//                }
//                else
//                    throw new InvalidCastException($"This weapon '{value.Name}' is not a launcher.");
//            }
//        }
//        public IInput Input
//        {
//            set
//            {
//                _aimer.Input = value;
//                //_transition.Input = value;
//            }
//        }

//        public bool Continuing => _aimer.Continuing;
//        public ITarget Target
//        {
//            get => _aimer.Target;
//            set => _aimer.Target = value;
//        }


//        public TaskState State => _selector.State;

//        void Awake()
//        {
//            _aimer = new(GetComponent<AimIK>());
//            Target = GetComponent<ITarget>();
//            _transition.Initialize(_aimer, _reloadAnimation);
//            //_selector = new Selector(_transition, _aimer);
//        }
//        public void OnUpdate()
//        {
//            if (!this.enabled)
//                return;
//            if (_input.Reload)
//            {
//                _transition.BStart();
//            }

//            _transition.OnUpdate();
//        }
//        void LateUpdate()
//        {

//            _aimer.OnUpdate();
//        }
//        public bool BEnd()
//        {
//            if (_transition.Continuing)
//                _transition.BEnd();
//            _aimer.BEnd();
//            enabled = false;
//            return true;
//        }

//        public bool BStart()
//        {
//            this.enabled = true;
//            return _aimer.BStart();
//        }

//        public TaskState Work()
//        {
//            if (!enabled)
//                return TaskState.Failure;
//            var state = _selector.Work();
//            Debug.Log(_selector.ToString());
//            return state;
//        }
//    }
//}
