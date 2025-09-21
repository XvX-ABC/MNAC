using Locomotion.Animation;
using Tests.Environment;
using UnityEngine;
using PostureState = Tests.Locomotion.AirLocomotion.PostureState;

namespace Tests.Locomotion.Animation.States
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(LocomotionCore))]
    public class LocomotionAnimationCore : MonoBehaviour, IModule
    {
        ILocomotionAnimationDefinitions _definitions;
        IBonesDefinitions _bonesDefinitions;
        LocomotionCore _core;
        Animator _animator;

        HorizontalLocomotion _horizontalLocomotion;
        JumpLocomotion _jumpLocomotion;
        AirLocomotion _airLocomotion;
        BoostingLocomotion _boostingLocomotion;

        AnimationStateMachine _stateMachine;
        GroundLocomotionAnimationState _groundState;
        JumpLocomotionAnimationState _jumpState;
        AirDescendingAnimationState _airDescendingState;
        FlyingAnimationState _flyingState;
        BoostingLocomotionAnimationState _boostingState;


        bool _jumpEnter;
        bool _airDescendingEnter;
        bool _flyingEnter;
        bool _boostingEnter;
        private void Awake()
        {
            _bonesDefinitions = GetComponent<IBonesDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(IBonesDefinitions));
            _definitions = GetComponent<ILocomotionAnimationDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(ILocomotionAnimationDefinitions));
            _animator = GetComponent<Animator>();
            _stateMachine = new(this.name);
        }
        void Start()
        {
            _core = GetComponent<LocomotionCore>();
            _horizontalLocomotion = _core.horizontalLocomotion;
            _jumpLocomotion = _core.jumpLocomotion;
            _airLocomotion = _core.airLocomotion;
            _boostingLocomotion = _core.boostingLocomotion;


            _groundState = new(_animator, _definitions.Ground, _bonesDefinitions);
            _jumpState = new(_animator, _definitions.Jump, _jumpLocomotion);
            _airDescendingState = new(_animator, _definitions.Air, _airLocomotion);
            _flyingState = new(_animator, _definitions.Air, _airLocomotion);
            _boostingState = new(_animator, _definitions.Boosting, _boostingLocomotion);

            _stateMachine.AddState(_groundState);
            _stateMachine.AddState(_jumpState);
            _stateMachine.AddState(_flyingState);
            _stateMachine.AddState(_boostingState);

            _stateMachine.AddTransitionFor(_groundState, _jumpState, () => _jumpEnter);
            _stateMachine.AddTransitionFor(_groundState, _airDescendingState, () => _airDescendingEnter);

            _stateMachine.AddTransitionFor(_jumpState, _airDescendingState, () => _airDescendingEnter);
            _stateMachine.AddTransitionFor(_jumpState, _flyingState, () => _flyingEnter);

            _stateMachine.AddTransitionFor(_airDescendingState, _flyingState, () => _flyingEnter);
            _stateMachine.AddTransitionFor(_airDescendingState, _groundState, () => !_airDescendingEnter);

            _stateMachine.AddTransitionFor(_flyingState, _airDescendingState, () => !_flyingEnter);

            _stateMachine.AddTransitionFor(_boostingState, _groundState, () => !_airDescendingEnter);
            _stateMachine.AddTransitionFor(_boostingState, _airDescendingState, () => _airDescendingEnter);



            _jumpLocomotion.AscendingStartAction += (_, _) => _jumpEnter = true;
            _jumpLocomotion.AscendingEndAction += (_, _) => _jumpEnter = false;


            _airLocomotion.PostureChangedAction += (oldState, newState, ctx) =>
            {
                if (newState == PostureState.Unchanged)
                {
                    _airDescendingEnter = false;
                    _flyingEnter = false;
                    return;
                }
                _airDescendingEnter = newState == PostureState.Descending;
                _flyingEnter = newState == PostureState.Ascending;
            };

            _boostingLocomotion.StartAction += _ => { _stateMachine.ChangeStateTo(_boostingState); _boostingEnter = true; };
            _boostingLocomotion.EndAction += _ => _boostingEnter = false;
        }
        void UpdateHorizontalVelocity(Context context)
        {
            var rotation = context.OriginalLocomotion.Rotation;
            var velocity = context.Velocity;
            var v = Quaternion.Inverse(rotation) * velocity * 0.05f;
            _animator.SetFloat(_definitions.Ground.XParamName, v.x);
            _animator.SetFloat(_definitions.Ground.YParamName, v.z);
        }
        private void OnAnimatorIK(int layerIndex)
        {
            _stateMachine.OnAnimatorIK(layerIndex);
        }
        public void OnFixedUpdate(Context context)
        {
            _stateMachine.Context = context;
            UpdateHorizontalVelocity(context);
            _stateMachine.OnUpdate();
        }
    }
}
