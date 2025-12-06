using System;
using Tests.Animations;
using Tests.Characters.Humanoid;
using Tests.Characters.Interaction;
using Tests.Interaction;
using Tests.Interaction.Influence;
using Tests.States;
using Tests.Utilities.Blackboards;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using AnimationNormalState = Tests.Characters.Humanoid.Animations.NormalState;
using Health = Tests.Interaction.Influence.Health;
using NormalState = Tests.Characters.Humanoid.NormalState;
using Stun = Tests.Interaction.Influence.Stun;

namespace Tests.Characters.C_0
{
    public interface IC_0 : ICharacter
    {

    }
    [Interactable]
    internal class C_0 : CharacterBase
    {

        [SerializeField]
        Camera _camera;
        internal HumanoidController _humanoidController;
        NormalState _normalState;
        AnimationNormalState _animationNormalState;
        ICharacterDefinitions_C_0 _definitions;
        ICharacterAnimationDefinitions_C_0 _animationDefinitions;

        protected override void Awake()
        {
            base.Awake();
            _humanoidController = GetComponentInChildren<HumanoidController>() ?? throw new ComponentCantFindException(this.gameObject, typeof(HumanoidComponent));
            _definitions = GetComponentInChildren<ICharacterDefinitions_C_0>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ICharacterDefinitions_C_0));
            _animationDefinitions = GetComponentInChildren<ICharacterAnimationDefinitions_C_0>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ICharacterAnimationDefinitions_C_0));
        }
        internal override Blackboard CreateBlackboard()
        {
            var blackboard = base.CreateBlackboard();
            blackboard.TryRegisterField(CharacterBlackboardFields.Player_Camera_Main, _camera);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Obj_Main, this.gameObject);
            return blackboard;
        }
        internal override InfluenceCore CreateInfluenceCore()
        {
            Debug.Log("influence core initialized: ");
            var stun = new Stun();
            var health = new Health();
            return new(stun, health);
        }
        internal override CharacterComponent[] GetComponents()
        {
            return new CharacterComponent[] { _humanoidController };
        }
        internal override void InitializeComponents(Blackboard blackboard)
        {
            _humanoidController.Initialize(blackboard);
            _normalState = _humanoidController.normalState;
            _animationNormalState = _humanoidController.animator.normalState;
        }
        internal override void ComponentsDispose()
        {
            _humanoidController.Dispose();
        }

        internal override CharacterBehavioursStatemachine CreateStatemachine()
        {
            var stun = influenceCore.FindInfluence<Stun>();
            var health = influenceCore.FindInfluence<Health>();

            var stunningState = new StunningState(stun.Timeline);

            var context = new CharacterBehavioursStateContext();
            var statmachine = new CharacterBehavioursStatemachine(context, this.gameObject.name);
            statmachine.AddState(_normalState);
            statmachine.AddState(stunningState);

            var n_s = new BlendingTransition<object>(_normalState, stunningState, () => stun.Enabled, null, 0.5f);
            statmachine.AddTransitionFor(n_s);

            var s_n = _normalState.CreateEntryTransition(stunningState, () => !stun.Enabled, null, 0.5f, 0, 1f);
            statmachine.AddTransitionFor(s_n);

            return statmachine;
        }
        internal override AnimationPlayablePartBase GetMainAnimationPlayablePart()
        {
            return _humanoidController.animator.layersMixer;
        }
        internal override CharacterAnimationStateMachine CreateAnimationStatemachine(CharacterAnimator animator)
        {

            var stun = influenceCore.FindInfluence<Stun>() ?? throw new ArgumentNullException("stun");
            var health = influenceCore.FindInfluence<Health>() ?? throw new ArgumentNullException("health");

            var stunningState = new Humanoid.Animations.StunningState(stun.Timeline, animator.controller, _animationDefinitions.Stunning);
            var deathState = default(Humanoid.Animations.DiedState);

            var statemachine = new CharacterAnimationStateMachine(this.gameObject.name);
            statemachine.AddState(_animationNormalState);
            statemachine.AddState(stunningState);
            //statemachine.AddState(deathState);

            {
                var g_s = new BlendingTransition<object>(_animationNormalState, stunningState, () => stun.Enabled, null, 0);
                //var g_d = new BlendingTransition<object>(_animationNormalState, deathState, () => !health.IsAlive, null, 0);
                statemachine.AddTransitionFor(g_s);
                //statemachine.AddTransitionFor(g_d);
            }
            {
                var s_g = new BlendingTransition<object>(stunningState, _animationNormalState, () => !stun.Enabled, null, 0);
                //var s_d = new BlendingTransition<object>(stunningState, deathState, () => !health.IsAlive, null, 0);
                statemachine.AddTransitionFor(s_g);
                //statemachine.AddTransitionFor(s_d);
            }

            return statemachine;
        }
    }
}
