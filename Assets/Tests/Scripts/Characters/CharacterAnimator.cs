using System;
using Tests.Behaviours.Animations;
using Tests.Characters.Animations;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Tests.Characters
{
    internal partial class CharacterAnimator : CharacterComponentBase, IDisposable
    {
        ICharacterAnimationDefinitions _definitions;
        CharacterCore _core;
        Animator _animator;


        bool _enabled;


        internal PlayableGraph graph;
        AnimationPlayablePartTree _appt;
        ControllerPlayable _controller;


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
            _blackboard = core.Blackboard;
            InitializePlayableGraph();
        }

        void InitializePlayableGraph()
        {
            graph = PlayableGraph.Create(_core.name + "_animator");
            _appt = new(graph);
            var root = _appt.Root;
            _controller = new ControllerPlayable(_animator);
            var layersMixer = new LayersMixerPlayable(_definitions);




            root.AddChild(layersMixer.Node);
            layersMixer.Node.AddChild(_controller.Node);

            _controller.OutputSetting.Weight = 1;

            var leftArm = _core.leftArm;

            layersMixer.Node.AddChild(leftArm.acore_new.Node);



            var a = (AnimationLayerMixerPlayable)layersMixer.PlayablePart;
            a.SetLayerMaskFromAvatarMask(1, _definitions.LeftArmDefinitions.Mask);

            var output = AnimationPlayableOutput.Create(graph, "animation", _animator);
            output.SetSourcePlayable(layersMixer.PlayablePart);
        }

        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Animator_Main, _controller);
        }
        public override void Dispose()
        {
            base.Dispose();
            graph.Destroy();
        }
    }
}
