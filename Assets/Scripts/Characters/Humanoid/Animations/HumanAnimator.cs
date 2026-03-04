using System;
using MNAC.Animations;
using MNAC.Characters.Humanoid.Locomotion;
using MNAC.Characters.Humanoid.Locomotion.Animations;
using MNAC.Interaction.Influence;
using MNAC.States;
using MNAC.Utilities.Blackboards;
using MNAC.Utilities.Composable;
using MNAC.Utilities.MTrees;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
namespace MNAC.Characters.Humanoid.Animations
{
    internal partial class HumanAnimator : ComponentBase, IDisposable
    {
        HumanoidController _humanoidController;

        internal PlayableGraph graph;
        CharacterBaseControllerPlayable _controller;
        internal LayersMixerPlayable layersMixer;
        LayerPlayable _baseLayer;
        LayerPlayable _leftArmLayer;
        LayerPlayable _rightArmLayer;


        HumanAnimationStatemachine _statemachine;
        internal NormalState normalState;


        internal class LayersMixerPlayable : AnimationPlayablePartBase
        {
            public LayersMixerPlayable(PlayableGraph graph) : base(graph)
            {
                var mixer = AnimationLayerMixerPlayable.Create(graph, 3);

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
        internal class LayerPlayable : AnimationPlayablePartBase
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

        public override string Name => "character_animator";

        public HumanAnimator(HumanoidController controller)
        {
            _humanoidController = controller ?? throw new ArgumentNullException(nameof(controller));
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryReadValueOrThrowException(CharacterBlackboardFields.Character_Animation_Graph, out graph);
            blackboard.TryReadValueOrThrowException(CharacterBlackboardFields.Character_Animation_Whole_Body_Animator, out _controller);

            layersMixer = new LayersMixerPlayable(graph);
            _baseLayer = layersMixer.CreateLayer();
            _leftArmLayer = layersMixer.CreateLayer();
            _rightArmLayer = layersMixer.CreateLayer();

            _baseLayer.Node.AddChild(_controller.Node);

            _controller.OutputSetting.Weight = 1;
        }
        public void InitializeArmsAnimation(AvatarMask leftArmMask, AvatarMask rightArmMask)
        {
            var leftArm = _humanoidController.leftArm;
            if (leftArm != null)
            {
                _leftArmLayer.Node.AddChild(leftArm.animatorCore.Node);
                _leftArmLayer.SetLayerMaskFromAvatarMask(leftArmMask);

                //_layersMixer.Node.AddChild(leftArm.animatorCore.Node);
                //var a = (AnimationLayerMixerPlayable)_layersMixer.PlayablePart;
                //a.SetLayerMaskFromAvatarMask(1, _definitions.LeftArmDefinitions.Mask);
            }

            var rightArm = _humanoidController.rightArm;
            if (rightArm != null)
            {
                _rightArmLayer.Node.AddChild(rightArm.animatorCore.Node);
                _rightArmLayer.SetLayerMaskFromAvatarMask(rightArmMask);

                //_layersMixer.Node.AddChild(rightArm.animatorCore.Node);
                //var a = (AnimationLayerMixerPlayable)_layersMixer.PlayablePart;
                //a.SetLayerMaskFromAvatarMask(1, _definitions.LeftArmDefinitions.Mask);
            }

        }
        public void InitializeNormalState()
        {
            blackboard.TryReadValueOrThrowException<LocomotionCore>(CharacterBlackboardFields.Character_Locomotion_Core, out var locomotionCore);
            var animator = locomotionCore.animator;
            normalState = new(animator.statemachine, animator.groundedMovement, "groundMovement");
        }
    }
}
