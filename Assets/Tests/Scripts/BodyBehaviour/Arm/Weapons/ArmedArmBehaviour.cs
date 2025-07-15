using Tests.States;
using Tests.Weapons;
using UnityEngine;

namespace Tests.Behaviours.Arm.Weapons
{
    public abstract class ArmedArmBehaviour : PlayableStateUComponentBase, IArmWeaponHoldingBehaviour
    {
        [SerializeField]
        protected WeaponType type;
        public WeaponType Type { get => type; }
        public abstract IWeapon Weapon { get; set; }
        public abstract IArmWeaponHoldingBehavioursAnimator Animator { get; }

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
