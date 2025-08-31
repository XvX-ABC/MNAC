using System;
using Tests.Behaviours.Animations;
using Tests.Characters.Animations;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Tests.Characters
{
    internal class CharacterAnimator : IDisposable
    {
        ICharacterAnimationDefinitions _definitions;
        CharacterCore _core;
        Animator _animator;
        bool _enabled;

        internal PlayableGraph graph;
        AnimationPlayablePartTree _appt;
        Blackboard _blackboard;
        class LayersMixerPlayable : AnimationPlayablePartBase
        {
            ICharacterAnimationDefinitions _definitions;

            public LayersMixerPlayable(ICharacterAnimationDefinitions definitions)
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
        internal class ControllerPlayable : AnimationPlayablePartBase
        {
            Animator _animator;
            public override IOutputSetting OutputSetting
            {
                get => base.OutputSetting;
                set
                {
                    base.OutputSetting = value;
                    outputSetting.Weight = 1;
                }
            }
            public ControllerPlayable(Animator animator)
            {
                _animator = animator ?? throw new ArgumentNullException(nameof(animator));
            }

            public override bool Initialize(PlayableGraph graph)
            {
                playablePart = AnimatorControllerPlayable.Create(graph, _animator.runtimeAnimatorController);
                return true;
            }
        }
        public bool Enabled
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
        public CharacterAnimator(CharacterCore core)
        {
            _definitions = core.GetComponent<ICharacterAnimationDefinitions>() ?? throw new ComponentCantFindException(core.gameObject, typeof(ICharacterAnimationDefinitions));
            _core = core ?? throw new ArgumentNullException(nameof(core));
            _animator = core.GetComponent<Animator>();
            _blackboard = core.Blackboard;
            InitializePlayableGraph();
        }

        void InitializePlayableGraph()
        {
            graph = PlayableGraph.Create(_core.name + "_animator");
            _appt = new(graph);
            var root = _appt.Root;
            var controller = new ControllerPlayable(_animator);
            var layersMixer = new LayersMixerPlayable(_definitions);




            root.AddChild(layersMixer.Node);
            layersMixer.Node.AddChild(controller.Node);

            controller.OutputSetting.Weight = 1;

            var leftArm = _core.leftArm;

            layersMixer.Node.AddChild(leftArm.acore_new.Node);



            var a = (AnimationLayerMixerPlayable)layersMixer.PlayablePart;
            a.SetLayerMaskFromAvatarMask(1, _definitions.LeftArmDefinitions.Mask);

            var output = AnimationPlayableOutput.Create(graph, "animation", _animator);
            output.SetSourcePlayable(layersMixer.PlayablePart);
        }

        public void Dispose()
        {
            graph.Destroy();
        }
    }
}
