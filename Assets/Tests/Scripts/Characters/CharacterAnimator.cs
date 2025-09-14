using Microsoft.Win32.SafeHandles;
using System;
using Tests.Behaviours.Animations;
using Tests.Characters.Locomotion;
using Tests.Characters.Locomotion.Animations;
using Tests.States;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Utilities.Timeline;

namespace Tests.Characters.Animations
{
    internal class CharacterAnimationStatemachine : WithCallbackPlayableStatemachine<object>
    {
        public CharacterAnimationStatemachine(bool enabled = true) : base("character_statemachine", enabled)
        {
        }
    }
    internal class CharacterAnimationStateBase : WithCallbackPlayableState<object>
    {
        public CharacterAnimationStateBase(string name, float duration = 0, bool enabled = true) : base(name == null ? "character_animation_state" : $"character_animation_state_{name}", duration, enabled)
        {
        }
    }
    internal class StunningState : CharacterAnimationStateBase
    {
        ControllerPlayable _controller;
        IStunningAnimationDefinitions _definitions;
        public StunningState(ITimeline timeline, ControllerPlayable controller, IStunningAnimationDefinitions definitions, bool enabled = true) : base("stunning", 0, enabled)
        {
            this.timeline = timeline ?? throw new ArgumentNullException(nameof(timeline));
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _definitions = definitions;
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            var clipLength = _definitions.ClipLength;
            var m = clipLength / (timeline.Length <= 0 ? 1 : timeline.Length);
            _controller.SetFloat(_definitions.Multiplier, m);
            _controller.SetTrigger(_definitions.Trigger);

        }
    }
    internal partial class CharacterAnimator : CharacterComponentBase, IDisposable
    {
        ICharacterAnimationDefinitions _definitions;
        CharacterCore _core;
        Animator _animator;


        bool _enabled;


        internal PlayableGraph graph;
        AnimationPlayablePartTree _appt;
        ControllerPlayable _controller;
        LayersMixerPlayable _layersMixer;


        CharacterAnimationStatemachine _statemachine;



        class LayersMixerPlayable : AnimationPlayablePartBase
        {
            ICharacterAnimationDefinitions _definitions;

            public LayersMixerPlayable(PlayableGraph graph, ICharacterAnimationDefinitions definitions) : base(graph)
            {
                _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            }

            public override bool Initialize(PlayableGraph graph)
            {
                var mixer = AnimationLayerMixerPlayable.Create(graph, 3);
                mixer.SetLayerMaskFromAvatarMask(1, _definitions.LeftArmDefinitions.Mask);

                mixer.SetInputWeight(0, 1);
                playablePart = mixer;
                return true;
            }
        }
        public new bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                if (_enabled)
                {

                    Debug.Log("graph start playing: " + graph.IsPlaying());
                    graph.Play();
                }
                else
                {
                    Debug.Log("graph stop playing: " + graph.IsPlaying());
                    graph.Stop();
                }
            }
        }

        public override string Name => "character_animator";

        public CharacterAnimator(CharacterCore core)
        {
            _definitions = core.GetComponent<ICharacterAnimationDefinitions>() ?? throw new ComponentCantFindException(core.gameObject, typeof(ICharacterAnimationDefinitions));
            _core = core ?? throw new ArgumentNullException(nameof(core));
            _animator = core.GetComponent<Animator>();
            InitializePlayableGraph();
        }

        void InitializePlayableGraph()
        {
            graph = PlayableGraph.Create(_core.name + "_animator");
            _appt = new(graph);
            var root = _appt.Root;
            _controller = new ControllerPlayable(graph, _animator);
            _layersMixer = new LayersMixerPlayable(graph, _definitions);




            root.AddChild(_layersMixer.Node);
            _layersMixer.Node.AddChild(_controller.Node);

            _controller.OutputSetting.Weight = 1;


            var output = AnimationPlayableOutput.Create(graph, "animation", _animator);
            output.SetSourcePlayable(_layersMixer.PlayablePart);
        }

        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Animation_Animator, _controller);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Animation_Graph, graph);
        }
        public void InitializeArmsAnimation()
        {
            var leftArm = _core.leftArm;

            if (leftArm != null)
            {

                _layersMixer.Node.AddChild(leftArm.animatorCore.Node);
                var a = (AnimationLayerMixerPlayable)_layersMixer.PlayablePart;
                a.SetLayerMaskFromAvatarMask(1, _definitions.LeftArmDefinitions.Mask);
            }

        }
        LocomotionAnimator lanimator;
        public void InitializeStatemachine()
        {
            if (!blackboard.TryReadValue<LocomotionCore>(CharacterBlackboardFields.Character_Locomotion_Core, out var locomotionCore))
                throw new Exception();
            if (!blackboard.TryReadValue<InfluenceReceivingCore>(CharacterBlackboardFields.Character_Influence_Receiving_Core, out var influenceCore))
                throw new Exception();
            var stunReceptor = influenceCore.FindReceptor<StunReceptor>();
            var stunningState = new StunningState(stunReceptor.timeline, _controller, _definitions.Stunning);

            lanimator = locomotionCore.animator;
            var groundedMovement = new SubStatemachineState<object>(lanimator.statemachine, lanimator.groundedMovement, "groundMovement");

            _statemachine = new();
            _statemachine.AddState(groundedMovement);
            _statemachine.AddState(stunningState);

            var g_s = new BlendingTransition<object>(groundedMovement, stunningState, () => stunReceptor.Enabled, null, 0);
            _statemachine.AddTransitionFor(g_s);

            var s_g = new BlendingTransition<object>(stunningState, groundedMovement, () => !stunReceptor.Enabled, null, 0);
            _statemachine.AddTransitionFor(s_g);


        }
        public void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.X))
                Debug.Log("debug point");

            _statemachine.OnUpdate();
            //Debug.Log(lanimator.statemachine);
            //Debug.Log("character animator statemahcine: " + _statemachine);
        }
        public override void Dispose()
        {
            base.Dispose();
            graph.Destroy();
        }
    }
}
