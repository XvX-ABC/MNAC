using Assets.Tests.Scripts.Weapons;
using Cinemachine;
using GBG.PlayableGraphMonitor.Editor.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Behaviours.Arm;
using Tests.Behaviours.Arm.Weapons;
using Tests.BodyBehaviour.Arm.Animations;
using Tests.Characters;
using Tests.Input;
using Tests.Utilities.MTrees;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Tests.Character
{
    public interface IPlayablePart : IDisposable
    {
        public bool Enabled { get; }
        public bool Initialize(PlayableGraph graph);
        public Playable PlayablePart { get; }
        public IOutputSetting OutputSetting { get; set; }
        public IPlayablePartNode Node { get; }
    }
    public abstract class PlayablePartBase : IPlayablePart
    {
        protected bool enabled;
        protected Playable playablePart;
        protected IOutputSetting outputSetting;
        protected AnimationPlayableNode node;
        public bool Enabled => enabled;

        public Playable PlayablePart => playablePart;
        public virtual IOutputSetting OutputSetting { get => outputSetting; set => outputSetting = value; }
        public virtual IPlayablePartNode Node { get => node; }
        protected PlayablePartBase()
        {
            node = new(this);
        }

        public virtual void Dispose()
        {
            playablePart.Destroy();
        }
        public abstract bool Initialize(PlayableGraph graph);
    }
    public interface IPlayablePartNode : IMTContainerNode<IPlayablePart>
    {
        public PlayableGraph Graph { get; set; }
    }
    public interface ICharacterArmAnimationDefinitions
    {
        AvatarMask Mask { get; }
    }
    [SerializeField]
    public interface ICharacterAnimationDefinitions
    {
        public ICharacterArmAnimationDefinitions LeftArmDefinitions { get; }
        public ICharacterArmAnimationDefinitions RightArmDefinitions { get; }
    }
    internal class CharacterAnimator_New : IDisposable
    {
        ICharacterAnimationDefinitions _definitions;
        CharacterCore _core;
        Animator _animator;
        bool _enabled;

        internal PlayableGraph graph;
        AnimationPlayablePartTree _appt;

        class LayersMixerPlayable : PlayablePartBase
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
                this.playablePart = mixer;
                return true;
            }
        }
        class ControllerPlayable : PlayablePartBase
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
                this.playablePart = AnimatorControllerPlayable.Create(graph, _animator.runtimeAnimatorController);
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
        public CharacterAnimator_New(CharacterCore core)
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
            var controller = new ControllerPlayable(_animator);
            var layersMixer = new LayersMixerPlayable(_definitions);


            root.AddChild(layersMixer.Node);
            layersMixer.Node.AddChild(controller.Node);

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
    internal class CharacterAnimator : IDisposable
    {
        ICharacterAnimationDefinitions _definitions;
        CharacterCore _core;
        Animator _animator;
        bool _enabled;

        internal PlayableGraph _graph;
        AnimationLayerMixerPlayable _layers;
        AnimatorControllerPlayable _bodyAnimator;
        AnimationPlayableOutput _output;
        AnimationPlayableNode _node;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                if (_enabled)
                {

                    Debug.Log("graph start playing: " + _graph.IsPlaying());
                    _graph.Play();
                }
                else
                {
                    Debug.Log("graph stop playing: " + _graph.IsPlaying());
                    _graph.Stop();
                }
            }
        }

        public float LeftArmWeight
        {
            get => _layers.GetInputWeight(1);
            set => _layers.SetInputWeight(1, Mathf.Clamp01(value));
        }
        public float RightArmWeight
        {
            get => _layers.GetInputWeight(2);
            set => _layers.SetInputWeight(2, Mathf.Clamp01(value));
        }
        public CharacterAnimator(CharacterCore core)
        {
            _definitions = core.GetComponent<ICharacterAnimationDefinitions>() ?? throw new ComponentCantFindException(core.gameObject, typeof(ICharacterAnimationDefinitions));
            _core = core ?? throw new ArgumentNullException(nameof(core));
            _animator = core.GetComponent<Animator>();

            InitializePlayableGraph();
        }
        void InitializePlayableGraph()
        {
            _graph = PlayableGraph.Create(_core.name + "_animator");



            _layers = AnimationLayerMixerPlayable.Create(_graph, 3);
            _layers.SetLayerMaskFromAvatarMask(1, _definitions.LeftArmDefinitions.Mask);
            _layers.SetInputWeight(0, 1);
            //_layers.SetLayerMaskFromAvatarMask(2, _definitions.RightArmDefinitions.Mask);

            _bodyAnimator = AnimatorControllerPlayable.Create(_graph, _animator.runtimeAnimatorController);

            var leftArm = _core.leftArm ?? throw new NullReferenceException(nameof(_core.leftArm));
            //var rightArm = _core.rightArm ?? throw new NullReferenceException(nameof(_core.rightArm));


            _graph.Connect(_bodyAnimator, 0, _layers, 0);
            _graph.Connect(leftArm.GetPlayablePart(_graph), 0, _layers, 1);
            //_graph.Connect(rightArm.GetPlayablePart(_graph), 0, _layers, 2);
            var outputSetting = new OutputSetting(_layers, 1);
            leftArm.OutputSetting = outputSetting;


            _output = AnimationPlayableOutput.Create(_graph, "animation", _animator);
            _output.SetSourcePlayable(_layers);

            leftArm.UpdateAction += p =>
            {
                _layers.DisconnectInput(1);
                _graph.Connect(p, 0, _layers, 1);
            };

        }

        public void Dispose()
        {
            _graph.Destroy();
        }
    }
    public class CharacterCore : CharacterMonoComponentBase, ICharacterComponent
    {
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
        //CharacterAnimator _animator;
        CharacterAnimator_New _animator_New;
        public override Blackboard Blackboard
        {
            get => base.Blackboard;
            set
            {
                var old = this.blackboard;
                base.Blackboard = value;
                if (old != null)
                {
                    old.TryUnregisterField(CharacterBlackboardFields.Input);
                    old.TryUnregisterField(CharacterBlackboardFields.WeaponCore);
                }
                if (value != null)
                {

                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Initialize(new Blackboard());
        }
        void OnEnable()
        {
            //if (_animator != null)
            //    _animator.Enabled = true;
            if (_animator_New != null)
                _animator_New.Enabled = true;
        }
        void Start()
        {
            InitializeChildNodes();
            InitializeAnimator();
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
            _animator_New.Enabled = false;
        }

        void OnDestroy()
        {
            //_animator.Dispose();
            _animator_New.Dispose();
        }
        void InitializeAnimator()
        {
            //_animator = new(this);
            //_animator.LeftArmWeight = 1;
            //_animator.Enabled = this.enabled;


            _animator_New = new(this);
            _animator_New.Enabled = this.enabled;
        }

        void InitializeChildNodes()
        {
            node.AddChild(leftArm.Node);
        }
        void InitializeBlackboard()
        {
            this.Blackboard = new Blackboard();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override void Initialize(Blackboard blackboard)
        {
            blackboard.TryRegisterField(CharacterBlackboardFields.Input, _input);
            blackboard.TryRegisterField(CharacterBlackboardFields.WeaponCore, _weaponCore);


            blackboard.TryReadValue<FieldChangeHandler>(CharacterBlackboardFields.FieldChangeHandler, out var handler);
            handler.RegisterAction(CharacterBlackboardFields.TargetsCatcher, (evt, ov, nv) =>
            {
            });
            this.blackboard = blackboard;
        }
    }
}
