using Tests.Characters.Humanoid.Animations;
using Tests.Interaction.Influence;
using Tests.Utilities.Blackboards;
using UnityEngine;
namespace Tests.Characters.Interaction
{
    [DefaultExecutionOrder(1)]
    public abstract class CharacterAnimatorBase : MonoBehaviour
    {
        CharacterAnimationStateMachine _statemachine;
        Blackboard _blackboard;
        internal virtual void Awake()
        {
            _blackboard = new();
        }
        internal virtual void Start()
        {
            InitializeAnimator(_blackboard);
            _statemachine = CreateStatemachine();
        }
        internal void OnEnable()
        {
            if (_statemachine != null)
                _statemachine.Enabled = true;
        }
        internal void OnDisable()
        {
            if (_statemachine != null)
                _statemachine.Enabled = false;
        }
        internal void Update()
        {
            _statemachine.OnUpdate();
        }
        internal abstract void InitializeAnimator(Blackboard blackboard);
        internal abstract CharacterAnimationStateMachine CreateStatemachine();

    }
}
