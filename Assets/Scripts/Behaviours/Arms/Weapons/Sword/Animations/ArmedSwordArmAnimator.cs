using System;
using MNAC.Animations;
using MNAC.Characters.Humanoid;
using MNAC.States;
using MNAC.TPhysics.Locomotion;
using UnityEngine;
using UnityEngine.Playables;
using Transition = MNAC.Behaviours.Arms.Weapons.Sword.Animations.IArmedSwordArmAnimationDefinitions.Transition;

namespace MNAC.Behaviours.Arms.Weapons.Sword.Animations
{
    internal class ArmedSwordArmAnimator : IArmedArmAnimationPlayablePart
    {
        class WholeBody
        {
            internal WholeBodyControllerPlayable controller;
            internal WholeBodyMixerPlayable mixer;
        }
        IArmedSwordArmBehaviourDefinitions _definitions;
        IArmedSwordArmAnimationDefinitions _animationDefinitions;
        HumanBodyPart _bodyPart;

        bool _enabled;


        WholeBody _wholeBody;
        ArmControllerPlayable _armController;

        BoostingHelper _boostingHelper;
        SlashHelper _slashHelper;

        internal Idle idle;
        internal Boosting boosting;
        internal Slash slash;
        internal WithCallbackPlayableStatemachine<object> statemachine;
        internal ArmedSwordAnimationState state;

        public ArmedSwordArmAnimator(
            HumanBodyPart bodyPart,
            PlayableGraph graph,
            ControllerPlayable baseController,
            WholeBodyMixerPlayable mixer,
            LocomotionCore locomotionCore,
            float maxSpeed,
            float accelerationSpeed,
            BoostingHelper boostingHelper,
            SlashHelper slashHelper,
            IArmedSwordArmBehaviourDefinitions definitions,
            IArmedSwordArmAnimationDefinitions animationDefinitions)
        {
            _bodyPart = bodyPart;
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _animationDefinitions = animationDefinitions ?? throw new ArgumentNullException(nameof(animationDefinitions));
            _boostingHelper = boostingHelper ?? throw new ArgumentNullException(nameof(_boostingHelper));
            _slashHelper = slashHelper ?? throw new ArgumentNullException(nameof(slashHelper));

            _armController = new ArmControllerPlayable(graph, _animationDefinitions.VelocityName_X, _animationDefinitions.VelocityName_Y, _animationDefinitions.ArmController);

            _wholeBody = new()
            {
                controller = new(
                    graph,
                    _animationDefinitions.Boost.SwitchName,
                    _animationDefinitions.Boost.MultiplierName,
                    _animationDefinitions.Boost.ClipLength,
                    _animationDefinitions.Slash.SwitchName,
                    _animationDefinitions.Slash.MultiplierName,
                    _animationDefinitions.Slash.ClipLength,
                    _animationDefinitions.WholeBodyController),
            };

            var baseWholeBodyController = baseController;

            var wholeBodyController = _wholeBody.controller;
            var isLeft = _bodyPart switch
            {
                Characters.Humanoid.HumanBodyPart.LeftArm => true,
                Characters.Humanoid.HumanBodyPart.RightArm => false,
                _ => throw new Exception("The body part must be one of the arms.")
            };
            wholeBodyController.SetBool(_animationDefinitions.MirrorSwitch, isLeft);

            var armController = _armController;

            InitializeWholeBodyAnimation(graph, baseController, mixer ?? throw new ArgumentNullException(nameof(mixer)), wholeBodyController);


            idle = new(armController, baseWholeBodyController, wholeBodyController, locomotionCore, maxSpeed, accelerationSpeed);
            boosting = new(baseWholeBodyController, wholeBodyController, armController, definitions.Boosting.MaxDuration);
            slash = new(baseWholeBodyController, wholeBodyController, armController, definitions.Slash.Duration, definitions.Slash.RecoveryDuration);

            InitializeStatemachine();
        }
        void InitializeWholeBodyAnimation(PlayableGraph graph, ControllerPlayable baseController, WholeBodyMixerPlayable mixer, ControllerPlayable wholeBodyController)
        {
            wholeBodyController.OutputSetting.Weight = 0;
            mixer.Node.AddChild(wholeBodyController.Node);
            _wholeBody.mixer = mixer;
        }
        void InitializeStatemachine()
        {
            statemachine = new("armed_sword_animation_statemachine");
            statemachine.AddState(idle);
            statemachine.AddState(boosting);
            statemachine.AddState(slash);

            var i_b = new BlendingTransition<object>(idle, boosting, () => _boostingHelper.EntryEvent, null, _animationDefinitions.GetTransitionOptions(Transition.Idle_Boosting));
            statemachine.AddTransitionFor(i_b);

            var b_s = new BlendingTransition<object>(boosting, slash, () => _slashHelper.EntryEvent, null, 0);
            var b_i = new BlendingTransition<object>(boosting, idle, () => _boostingHelper.ExitEvent, null, _animationDefinitions.GetTransitionOptions(Transition.Boosting_Idle));
            statemachine.AddTransitionFor(b_s);
            statemachine.AddTransitionFor(b_i);

            //BUG：slash到idle的过渡会偶尔不工作，直接跳转到idle状态
            var s_i = new BlendingTransition<object>(slash, idle, () => _slashHelper.ExitEvent, null, _animationDefinitions.GetTransitionOptions(Transition.Slash_Idle));
            statemachine.AddTransitionFor(s_i);

            state = new(this);
        }
        public IOutputSetting OutputSetting { get => _armController.OutputSetting; set => _armController.OutputSetting = value; }
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
            }
        }

        public Playable GetPlayablePart(PlayableGraph graph)
        {
            return _armController.PlayablePart;
        }
    }
}
