using System;
using Tests.Animations;
using Tests.States;
using Tests.TPhysics;
using Tests.TPhysics.Environment;
using Tests.TPhysics.Locomotion;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Tests.Behaviours.Arms.Weapons.Sword.Animations
{
    internal class ArmedSwordArmAnimator : IArmedWeaponArmAnimationPlayablePart
    {
        class WholeBody
        {
            internal ControllerPlayable controller;
            internal MixerPlayable mixer;
        }
        IArmedSwordArmBehaviourDefinitions _definitions;
        IArmedSwordArmAnimationDefinitions _animationDefinitions;
        ISword _sword;


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
            InitializeWholeBodyAnimation(graph, baseController, _wholeBody.controller);


            idle = new(_controller, locomotionCore, maxSpeed, accelerationSpeed, animationDefinitions.VelocityName_X, animationDefinitions.VelocityName_Y);
            boosting = new(_wholeBody.controller, _controller, _wholeBody.mixer, animationDefinitions.BoostingSwitchName, animationDefinitions.BoostingSpeedMultiplierName, animationDefinitions.BoostingClipLength, definitions.Boosting.MaxDuration);
            slash = new(_wholeBody.controller, _controller, _wholeBody.mixer, _animationDefinitions.SlashSwitchName, animationDefinitions.SlashSpeedMultiplierName, animationDefinitions.SlashClipLength, definitions.Slash.Duration);

            InitializeStatemachine();
        }

        void InitializeWholeBodyAnimation(PlayableGraph graph, ControllerPlayable baseController, ControllerPlayable controller)
        {
            var bnode = baseController.Node;
            var parent = bnode.Parent;
            parent.RemoveChild(bnode);

            var mixer = new MixerPlayable(graph, 2);
            mixer.OutputSetting.Weight = 1;

            var mnode = mixer.Node;
            parent.AddChild(mnode);

            mnode.AddChild(bnode);
            mnode.AddChild(controller.Node);

            _wholeBody.mixer = mixer;
        }
        void InitializeStatemachine()
        {
            statemachine = new("armed_sword_animation_statemachine");
            statemachine.AddState(idle);
            statemachine.AddState(boosting);
            statemachine.AddState(slash);

            var i_b = new BlendingTransition<object>(idle, boosting, () => _boostingHelper.EntryEvent, null, 0);
            statemachine.AddTransitionFor(i_b);

            var b_s = new BlendingTransition<object>(boosting, slash, () => _slashHelper.EntryEvent, null, 0);
            var b_i = new BlendingTransition<object>(boosting, idle, () => _boostingHelper.ExitEvent, null, 0, 0, -1, InterruptionSource.None);
            statemachine.AddTransitionFor(b_s);
            statemachine.AddTransitionFor(b_i);

            var s_i = new BlendingTransition<object>(slash, idle, () => _slashHelper.ExitEvent, null, 0, 0, -1, InterruptionSource.None);
            statemachine.AddTransitionFor(s_i);

            state = new(this);
        }
        public IOutputSetting OutputSetting { get => _controller.OutputSetting; set => _controller.OutputSetting = value; }
        public bool Enabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
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
            //Debug.Log(statemachine);
            //var p = (AnimatorControllerPlayable)_wholeBody.controller.PlayablePart;
            //var state = p.GetCurrentAnimatorStateInfo(0);
            //var t = p.GetAnimatorTransitionInfo(0);
            //Debug.Log($"{state.IsName("Idle")}, {state.IsName("baked_armed_sword_boosting_V0")}, {state.IsName("baked_armed_sword_slash_v0")}, {_wholeBody.controller.GetBool(_animationDefinitions.BoostingSwitchName)}, {_wholeBody.controller.GetBool(_animationDefinitions.SlashSwitchName)}, {t.IsName("Idle -> baked_armed_sword_boosting_V0")}, { t.IsName("baked_armed_sword_boosting_V0 -> baked_armed_sword_slash_v0")}");
        }
    }
}
