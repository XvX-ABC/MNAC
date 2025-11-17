using System;
using Tests.Animations;
using Tests.States;
using Tests.TPhysics;
using Tests.TPhysics.Environment;
using Tests.TPhysics.Locomotion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Transition = Tests.Behaviours.Arms.Weapons.Sword.IArmedSwordArmAnimationDefinitions.Transition;

namespace Tests.Behaviours.Arms.Weapons.Sword.Animations
{
    internal class ArmedSwordArmAnimator : IArmedWeaponArmAnimationPlayablePart
    {
        class WholeBody
        {
            internal ControllerPlayable controller;
            internal WholeBodyMixerPlayable mixer;
        }
        IArmedSwordArmBehaviourDefinitions _definitions;
        IArmedSwordArmAnimationDefinitions _animationDefinitions;
        ISword _sword;

        bool _enabled;


        WholeBody _wholeBody;
        ControllerPlayable _controller;

        BoostingHelper _boostingHelper;
        SlashHelper _slashHelper;

        internal Idle idle;
        internal Boosting boosting;
        internal Slash slash;
        internal WithCallbackPlayableStatemachine<object> statemachine;
        internal ArmedSwordAnimationState state;

        public ArmedSwordArmAnimator(
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
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _animationDefinitions = animationDefinitions ?? throw new ArgumentNullException(nameof(animationDefinitions));
            _boostingHelper = boostingHelper ?? throw new ArgumentNullException(nameof(_boostingHelper));
            _slashHelper = slashHelper ?? throw new ArgumentNullException(nameof(slashHelper));

            _controller = new(graph, _animationDefinitions.ArmController);
            _wholeBody = new()
            {
                controller = new(graph, _animationDefinitions.WholeBodyController),
            };

            var baseWholeBodyController = baseController;
            var wholeBodyController = _wholeBody.controller;
            var armController = _controller;
            InitializeWholeBodyAnimation(graph, baseController, mixer ?? throw new ArgumentNullException(nameof(mixer)), wholeBodyController);


            idle = new(wholeBodyController, locomotionCore, maxSpeed, accelerationSpeed, animationDefinitions.VelocityName_X, animationDefinitions.VelocityName_Y);
            boosting = new(baseWholeBodyController, wholeBodyController, armController, animationDefinitions.BoostingSwitchName, animationDefinitions.BoostingSpeedMultiplierName, animationDefinitions.BoostingClipLength, definitions.Boosting.MaxDuration);
            slash = new(baseWholeBodyController, wholeBodyController, armController, _animationDefinitions.SlashSwitchName, animationDefinitions.SlashSpeedMultiplierName, animationDefinitions.SlashClipLength, definitions.Slash.Duration);

            InitializeStatemachine();
        }
        //DONE: 双手同时加载时，会出现抢占输入端口情况
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

            //var i_b = new BlendingTransition<object>(idle, boosting, () => _boostingHelper.EntryEvent, null, 0);
            var i_b = new BlendingTransition<object>(idle, boosting, () => _boostingHelper.EntryEvent, null, _animationDefinitions.GetTransitionOptions(Transition.Idle_Boosting));
            statemachine.AddTransitionFor(i_b);

            //var b_s = new BlendingTransition<object>(boosting, slash, () => _slashHelper.EntryEvent, null, 0);
            //var b_i = new BlendingTransition<object>(boosting, idle, () => _boostingHelper.ExitEvent, null, 0, 0, -1, InterruptionSource.None);
            var b_s = new BlendingTransition<object>(boosting, slash, () => _slashHelper.EntryEvent, null, 0);
            var b_i = new BlendingTransition<object>(boosting, idle, () => _boostingHelper.ExitEvent, null, _animationDefinitions.GetTransitionOptions(Transition.Boosting_Idle));
            statemachine.AddTransitionFor(b_s);
            statemachine.AddTransitionFor(b_i);

            //var s_i = new BlendingTransition<object>(slash, idle, () => _slashHelper.ExitEvent, null, 0, 0, -1, InterruptionSource.None);
            var s_i = new BlendingTransition<object>(slash, idle, () => _slashHelper.ExitEvent, null, _animationDefinitions.GetTransitionOptions(Transition.Slash_Idle));
            statemachine.AddTransitionFor(s_i);

            state = new(this);
        }
        public IOutputSetting OutputSetting { get => _controller.OutputSetting; set => _controller.OutputSetting = value; }
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                _controller.OutputSetting.Weight = _wholeBody.controller.OutputSetting.Weight = value ? 1 : 0;
            }
        }
        public ISword Sword
        {
            get => _sword;
            set
            {
                _sword = value;
            }
        }

        public Playable GetPlayablePart(PlayableGraph graph)
        {
            return _controller.PlayablePart;
        }
        public void Update()
        {
            statemachine.OnUpdate();
            var p = (AnimatorControllerPlayable)_wholeBody.controller.PlayablePart;
            var state = p.GetCurrentAnimatorStateInfo(0);
            var t = p.GetAnimatorTransitionInfo(0);
            //Debug.Log($"idle: {state.IsName("Idle")}, boosting: {state.IsName("baked_armed_sword_boosting_V0")}, slash: {state.IsName("baked_armed_sword_slash_v0")}, boosting_sw:{_wholeBody.controller.GetBool(_animationDefinitions.BoostingSwitchName)}, slash_sw: {_wholeBody.controller.GetBool(_animationDefinitions.SlashSwitchName)}, {t.IsName("Idle -> baked_armed_sword_boosting_V0")}, {t.IsName("baked_armed_sword_boosting_V0 -> baked_armed_sword_slash_v0")}");
        }
    }
}
