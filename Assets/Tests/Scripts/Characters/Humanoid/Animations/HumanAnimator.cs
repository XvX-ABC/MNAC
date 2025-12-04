using System;
using Tests.Animations;
using Tests.Characters.Animations;
using Tests.Characters.Humanoid.Locomotion;
using Tests.Characters.Humanoid.Locomotion.Animations;
using Tests.Interaction.Influence;
using Tests.States;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using Tests.Utilities.MTrees;
using Tests.Utilities.Timeline;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
namespace Tests.Characters.Humanoid.Animations
{
    internal class HumanAnimationStatemachine : WithCallbackPlayableStatemachine<object>
    {
        public HumanAnimationStatemachine(bool enabled = true) : base("humanoid_statemachine", enabled)
        {
        }
    }
    internal class HumanAnimationStateBase : WithCallbackPlayableState<object>
    {
        public HumanAnimationStateBase(string name, float duration = 0, bool enabled = true) : base(name == null ? "humanoid_animation_state" : $"humanoid_animation_state_{name}", duration, enabled)
        {
        }
    }
    internal class StunningState : HumanAnimationStateBase
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
    internal class DiedState : HumanAnimationStateBase
    {
        ControllerPlayable _controller;
        IDeathAnimationDefinitions _definitions;
        public DiedState(ITimeline timeline, ControllerPlayable controller, IDeathAnimationDefinitions definitions, bool enabled = true) : base("stunning", 0, enabled)
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
    internal partial class HumanAnimator : ComponentBase, IDisposable
    {
        IHumanAnimationDefinitions _definitions;
        HumanoidController _core;
        Animator _animator;


        bool _enabled;


        internal PlayableGraph graph;
        AnimationPlayablePartTree _appt;
        ControllerPlayable _controller;
        LayersMixerPlayable _layersMixer;
        LayerPlayable _baseLayer;
        LayerPlayable _leftArmLayer;
        LayerPlayable _rightArmLayer;


        HumanAnimationStatemachine _statemachine;


        class LayersMixerPlayable : AnimationPlayablePartBase
        {
            IHumanAnimationDefinitions _definitions;
            public LayersMixerPlayable(PlayableGraph graph, IHumanAnimationDefinitions definitions) : base(graph)
            {
                _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
                var mixer = AnimationLayerMixerPlayable.Create(graph, 3);
                //mixer.SetLayerMaskFromAvatarMask(1, _definitions.LeftArmDefinitions.Mask);

                mixer.SetInputWeight(0, 1);
                playablePart = mixer;
            }
            public LayerPlayable CreateLayer()
            {
                var layer = new LayerPlayable(graph, (AnimationLayerMixerPlayable)playablePart, node.Children.Count);
                node.Children.Add(layer.Node);
                layer.Node.Parent = node;
                return layer;
            }
        }
        class LayerNode : AnimationPlayableNode
        {
            int _num;
            public LayerNode(IAnimationPlayablePart part, int num) : base(part)
            {
                _num = num;
            }
            protected override void SetOutputSettingForNode(IAnimationPlayablePartNode node)
            {
                var p = value.PlayablePart;
                var idx = _num;
                var outputSetting = node.Value.OutputSetting ?? throw new NullReferenceException(nameof(node.Value.OutputSetting));
                outputSetting.PortNum = idx;
                outputSetting.Parent = value;
            }
        }
        class LayerPlayable : AnimationPlayablePartBase
        {
            int _num;

            internal LayerPlayable(PlayableGraph graph, AnimationLayerMixerPlayable mixer, int num) : base(graph)
            {
                _num = Mathf.Max(0, num);
                node = new LayerNode(this, _num);
                this.playablePart = mixer;
            }
            protected override AnimationPlayableNode CreateNode()
            {
                return null;
            }
            public void SetLayerMaskFromAvatarMask(AvatarMask mask)
            {
                var p = (AnimationLayerMixerPlayable)playablePart;
                p.SetLayerMaskFromAvatarMask((uint)_num, mask);
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

                    graph.Play();
                }
                else
                {
                    graph.Stop();
                }
            }
        }

        public override string Name => "character_animator";

        public HumanAnimator(HumanoidController core)
        {
            _definitions = core.GetComponent<IHumanAnimationDefinitions>() ?? throw new ComponentCantFindException(core.gameObject, typeof(IHumanAnimationDefinitions));
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
            //_baseLayer = new LayerPlayable(graph, (AnimationLayerMixerPlayable)_layersMixer.PlayablePart, 0);
            //_leftArmLayer = new LayerPlayable(graph, (AnimationLayerMixerPlayable)_layersMixer.PlayablePart, 1);
            //_rightArmLayer = new LayerPlayable(graph, (AnimationLayerMixerPlayable)_layersMixer.PlayablePart, 2);
            _baseLayer = _layersMixer.CreateLayer();
            _leftArmLayer = _layersMixer.CreateLayer();
            _rightArmLayer = _layersMixer.CreateLayer();


            root.AddChild(_layersMixer.Node);
            //_layersMixer.Node.AddChild(_controller.Node);
            _baseLayer.Node.AddChild(_controller.Node);

            _controller.OutputSetting.Weight = 1;


            var output = AnimationPlayableOutput.Create(graph, "animation", _animator);
            output.SetSourcePlayable(_layersMixer.PlayablePart);
        }

        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Animation_Whole_Body_Animator, _controller);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Animation_Graph, graph);
        }
        public void InitializeArmsAnimation()
        {
            var leftArm = _core.leftArm;
            if (leftArm != null)
            {
                _leftArmLayer.Node.AddChild(leftArm.animatorCore.Node);
                _leftArmLayer.SetLayerMaskFromAvatarMask(_definitions.LeftArmDefinitions.Mask);

                //_layersMixer.Node.AddChild(leftArm.animatorCore.Node);
                //var a = (AnimationLayerMixerPlayable)_layersMixer.PlayablePart;
                //a.SetLayerMaskFromAvatarMask(1, _definitions.LeftArmDefinitions.Mask);
            }

            var rightArm = _core.rightArm;
            if (rightArm != null)
            {
                _rightArmLayer.Node.AddChild(rightArm.animatorCore.Node);
                _rightArmLayer.SetLayerMaskFromAvatarMask(_definitions.RightArmDefinitions.Mask);

                //_layersMixer.Node.AddChild(rightArm.animatorCore.Node);
                //var a = (AnimationLayerMixerPlayable)_layersMixer.PlayablePart;
                //a.SetLayerMaskFromAvatarMask(1, _definitions.LeftArmDefinitions.Mask);
            }

        }
        LocomotionAnimator lanimator;
        public void InitializeStatemachine()
        {
            if (!blackboard.TryReadValue<LocomotionCore>(CharacterBlackboardFields.Character_Locomotion_Core, out var locomotionCore))
                throw new Exception();
            if (!blackboard.TryReadValue<InfluenceCore>(CharacterBlackboardFields.Character_Influence_Core, out var influenceCore))
                throw new Exception();
            var stun = influenceCore.FindInfluence<Stun>() ?? throw new ArgumentNullException("stun");
            var health = influenceCore.FindInfluence<Health>() ?? throw new ArgumentNullException("health");

            var stunningState = new StunningState(stun.Timeline, _controller, _definitions.Stunning);
            var diedState = new DiedState(_core.diedState.Timeline, _controller, _definitions.Death);

            lanimator = locomotionCore.animator;
            var groundedMovement = new SubStatemachineState<object>(lanimator.statemachine, lanimator.groundedMovement, "groundMovement");

            _statemachine = new();
            _statemachine.AddState(groundedMovement);
            _statemachine.AddState(stunningState);
            _statemachine.AddState(diedState);

            {
                var g_s = new BlendingTransition<object>(groundedMovement, stunningState, () => stun.Enabled, null, 0);
                var g_d = new BlendingTransition<object>(groundedMovement, diedState, () => !health.IsAlive, null, 0);
                _statemachine.AddTransitionFor(g_s);
                //_statemachine.AddTransitionFor(g_d);
            }
            {
                var s_g = new BlendingTransition<object>(stunningState, groundedMovement, () => !stun.Enabled, null, 0);
                var s_d = new BlendingTransition<object>(stunningState, diedState, () => !health.IsAlive, null, 0);
                _statemachine.AddTransitionFor(s_g);
                _statemachine.AddTransitionFor(s_d);
            }


        }
        public void Update()
        {

            _statemachine.OnUpdate();
            //Debug.Log(lanimator.statemachine);
            //Debug.Log("character animator statemahcine: " + _statemachine);
        }
        public override void Dispose()
        {
            graph.Destroy();
            base.Dispose();
        }
    }
}
