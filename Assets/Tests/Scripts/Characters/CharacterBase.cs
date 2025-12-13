using System;
using System.Linq;
using Tests.Animations;
using Tests.Characters.Humanoid;
using Tests.Interaction;
using Tests.Interaction.Influence;
using Tests.Utilities.Blackboards;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Rendering;
namespace Tests.Characters.Interaction
{
    using AnimationNormalState = Tests.Characters.Humanoid.Animations.NormalState;
    [DefaultExecutionOrder(0)]
    public abstract class CharacterBase : MonoBehaviour, ICharacter
    {   
        internal class CharacterAnimator : IDisposable
        {
            internal PlayableGraph graph;
            internal AnimationPlayablePartTree appt;
            internal CharacterBaseControllerPlayable controller;
            internal AnimationPlayableOutput _output;
            Animator _animator;
            Blackboard _blackboard;
            public CharacterAnimator(GameObject obj, Animator animator, Blackboard blackboard)
            {
                _blackboard = blackboard ?? throw new ArgumentNullException(nameof(blackboard));
                _animator = animator ?? throw new ArgumentNullException(nameof(animator));
                graph = PlayableGraph.Create(obj.name + "_running_graph");
                appt = new(graph);
                controller = new(graph, animator);

                blackboard.TryRegisterField(CharacterBlackboardFields.Character_Animation_Graph, graph);
                blackboard.TryRegisterField(CharacterBlackboardFields.Character_Animation_Whole_Body_Animator, controller);
            }
            public void Dispose()
            {
                _blackboard.TryUnregisterField(CharacterBlackboardFields.Character_Animation_Graph, graph);
                _blackboard.TryUnregisterField(CharacterBlackboardFields.Character_Animation_Whole_Body_Animator, controller);
            }
            public void InitializeOutput(AnimationPlayablePartBase p)
            {
                if (p == null)
                    throw new ArgumentNullException(nameof(p));
                appt.Root.AddChild(p.Node);
                _output = AnimationPlayableOutput.Create(graph, "animation", _animator);
                _output.SetSourcePlayable(p.PlayablePart);
            }
            public void Play()
            {
                graph.Play();
            }
            public void Stop()
            {
                graph.Stop();
            }
            ~CharacterAnimator()
            {
                Dispose();
            }
        }
        Guid _id;
        InteractableItem _item;
        internal Blackboard blackboard;
        internal InfluenceCore influenceCore;
        internal CharacterComponent[] components;
        CharacterBehavioursStatemachine _statemachine;
        CharacterAnimationStateMachine _animationStatemachine;
        CharacterAnimator _animator;
        internal bool allowAnimationInitialization { get => _animator != null; }
        public CharacterBase()
        {
            _id = Guid.NewGuid();

        }
        protected virtual void Awake()
        {
            influenceCore = CreateInfluenceCore();
            blackboard = CreateBlackboard();
            components = GetComponents();
            if (TryGetComponent<Animator>(out var animator))
            {
                _animator = new(this.gameObject, animator, blackboard);
            }

        }
        protected virtual void Start()
        {
            InitializeComponents(blackboard);
            _statemachine = CreateStatemachine();
            _animator?.InitializeOutput(GetMainAnimationPlayablePart());

            if (allowAnimationInitialization)
                _animationStatemachine = CreateAnimationStatemachine(_animator);
        }

        protected virtual void OnEnable()
        {
            TryRegisterToInteractionManager();
            if (_statemachine != null)
                _statemachine.Enabled = true;
            if (_animationStatemachine != null)
                _animationStatemachine.Enabled = true;
            EnableComponents();
            _animator?.Play();
        }
        protected virtual void OnDisable()
        {
            _animator?.Stop();
            if (_statemachine != null)
                _statemachine.Enabled = false;
            if (_animationStatemachine != null)
                _animationStatemachine.Enabled = false;
            UnregisterFromInteractionManager();
            DisableComponents();
        }
        protected virtual void Update()
        {
            _statemachine.OnUpdate();
            _animationStatemachine?.OnUpdate();
        }
        protected virtual void OnDestroy()
        {
            ComponentsDispose();
        }
        protected void TryRegisterToInteractionManager()
        {
            var attrs = this.GetType().GetCustomAttributes(true);
            var a = attrs.FirstOrDefault(a => a is InteractableAttribute);
            if (a != null)
            {
                _item = new InteractableItem(this.ID, this.gameObject);
                InteractionManager.AddItem(_item);
            }
        }
        protected void UnregisterFromInteractionManager()
        {
            InteractionManager.RemoveItem(_item);
        }
        public Guid ID => _id;

        public string Name => this.name;
        internal virtual Blackboard CreateBlackboard()
        {
            var blackboard = new Blackboard();
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Influence_Core, influenceCore);
            return blackboard;
        }
        internal abstract InfluenceCore CreateInfluenceCore();
        internal abstract CharacterComponent[] GetComponents();
        internal abstract void InitializeComponents(Blackboard blackboard);
        internal abstract void ComponentsDispose();
        internal virtual void EnableComponents()
        {
            foreach (var comp in components)
            {
                if (comp == null)
                    continue;
                comp.enabled = true;
            }
        }
        internal virtual void DisableComponents()
        {
            foreach (var comp in components)
            {
                if (comp == null)
                    continue;
                comp.enabled = false;
            }
        }
        internal abstract CharacterBehavioursStatemachine CreateStatemachine();
        internal abstract AnimationPlayablePartBase GetMainAnimationPlayablePart();
        internal virtual CharacterAnimationStateMachine CreateAnimationStatemachine(CharacterAnimator animator) { return null; }
    }
}
