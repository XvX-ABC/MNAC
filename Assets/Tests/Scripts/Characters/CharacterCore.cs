using System;
using Tests.Characters.Arms;
using Tests.Characters.Locomotion;
using Tests.Input;
using Tests.TPhysics.Environment;
using Tests.Weapons;
using TMPro;
using UnityEngine;
using EnvironmentCore = Tests.Characters.Environment.EnvironmentCore;

namespace Tests.Characters
{
    public class CharacterCore : CharacterComponentBase_MonoComponent, ICharacterComponent
    {
        [SerializeField]
        Camera _camera;
        [SerializeField]
        internal ArmCore leftArm;
        //[SerializeField]
        //internal ArmCore rightArm;
        [SerializeField]
        WeaponCore _weaponCore;
        [SerializeField]
        CustomPlayerInput _input;
        [SerializeField]
        Target _target;

        Blackboard _blackboard;
        CharacterAnimator _animator;

        LocomotionCore _locomotionCore;
        EnvironmentCore _environmentCore;
        //public override Blackboard Blackboard
        //{
        //    get => base.Blackboard;
        //    set
        //    {
        //        var old = blackboard;
        //        base.Blackboard = value;
        //        if (old != null)
        //        {
        //            old.TryUnregisterField(CharacterBlackboardFields.Input);
        //            old.TryUnregisterField(CharacterBlackboardFields.WeaponCore);
        //        }
        //        if (value != null)
        //        {

        //        }
        //    }
        //}

        protected override void Awake()
        {
            base.Awake();
            _environmentCore = GetComponent<EnvironmentCore>() ?? throw new ComponentCantFindException(this.gameObject, typeof(EnvironmentCore));
            _locomotionCore = GetComponent<LocomotionCore>() ?? throw new ComponentCantFindException(this.gameObject, typeof(EnvironmentCore));
            Initialize(new Blackboard());
        }
        void OnEnable()
        {
            if (_animator != null)
                _animator.Enabled = true;
        }
        void Start()
        {
            InitializeEnvironmentCore();
            //InitializeChildNodes();
            InitializeLocomotionCore();
            //InitializeAnimator();
        }
        void Update()
        {
            UpdateTarget();
        }
        void UpdateTarget()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                if (blackboard.TryReadValue<ITargetsCatcher>(CharacterBlackboardFields.TargetsCatcher, out var catcher))
                {
                    if (catcher.Targets.Count > 0)
                        catcher.RemoveTarget(_target);
                    else
                        catcher.AddTarget(_target);
                }
            }
        }
        void OnDisable()
        {
            //_animator.Enabled = false;
            if (_animator != null)
                _animator.Enabled = false;
        }

        void OnDestroy()
        {
            if (_animator != null)
                _animator.Dispose();
        }
        void InitializeAnimator()
        {

            _animator = new(this);
            _animator.Enabled = enabled;
        }

        void InitializeChildNodes()
        {
            node.AddChild(leftArm.Node);
        }
        void InitializeEnvironmentCore()
        {
            node.AddChild(_environmentCore.Node);
        }
        void InitializeLocomotionCore()
        {
            node.AddChild(_locomotionCore.Node);
        }
        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override void Initialize(Blackboard blackboard)
        {
            blackboard.TryRegisterField(CharacterBlackboardFields.Input, _input);
            blackboard.TryRegisterField(CharacterBlackboardFields.WeaponCore, _weaponCore);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Camera_Main, _camera);


            blackboard.TryReadValue<FieldChangeHandler>(CharacterBlackboardFields.FieldChangeHandler, out var handler);

            var rbody = GetComponent<Rigidbody>() ?? throw new ComponentCantFindException(this.gameObject, typeof(Rigidbody));
            blackboard.TryRegisterField(CharacterBlackboardFields.Rigidbody, rbody);

            this.blackboard = blackboard;
        }
    }
}
