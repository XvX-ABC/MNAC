using System;
using Tests.Animations;
using Tests.Characters.Humanoid;
using Tests.Characters.Humanoid.Locomotion;
using Tests.Characters.Interaction;
using Tests.Interaction;
using Tests.Interaction.Influence;
using Tests.States;
using Tests.Utilities.Blackboards;
using Unity.VisualScripting;
using UnityEngine;
using AnimationNormalState = Tests.Characters.Humanoid.Animations.NormalState;
using Health = Tests.Interaction.Health;
using NormalState = Tests.Characters.Humanoid.NormalState;
using Stun = Tests.Interaction.Influence.Stun;

namespace Tests.Characters.C_0
{
    public interface IC_0 : ICharacter, IDamageable, ITeamMember, ICompositeItems
    {

    }
    [Interactable]
    internal class C_0 : CharacterBase, IC_0
    {

        [SerializeField]
        Camera _camera;
        [SerializeField]
        HumanoidBodyParts _bodyParts;
        [SerializeField]
        Collider _movementCollider;
        internal HumanoidController _humanoidController;
        NormalState _normalState;
        AnimationNormalState _animationNormalState;
        ICharacterDefinitions_C_0 _definitions;
        ICharacterAnimationDefinitions_C_0 _animationDefinitions;
        Health _health;
        TeamMask _teamMask;
        Action<GameObject> _destroyedCallBack;

        DeathState _deathState;
        public TeamMask TeamMask { get => _teamMask; set => _teamMask = value; }

        public bool IsAlive => _health.IsAlive;

        public float MaxPoint => _health.MaxPoint;

        public float MinPoint => _health.MinPoint;

        public float Point => _health.Point;

        public Action<GameObject> DestroyedCallback { get => _destroyedCallBack; set => _destroyedCallBack = value; }

        public IHealth HP => _health;

        protected override Bounds bounds => _movementCollider.bounds;

        protected override void Awake()
        {
            _humanoidController = GetComponentInChildren<HumanoidController>() ?? throw new ComponentCantFindException(this.gameObject, typeof(HumanoidComponent));
            _definitions = GetComponentInChildren<ICharacterDefinitions_C_0>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ICharacterDefinitions_C_0));
            _animationDefinitions = GetComponentInChildren<ICharacterAnimationDefinitions_C_0>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ICharacterAnimationDefinitions_C_0));
            _teamMask = _definitions.TeamMask;

            base.Awake();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _destroyedCallBack?.Invoke(this.gameObject);
        }
        internal override Blackboard CreateBlackboard()
        {
            var blackboard = base.CreateBlackboard();
            blackboard.TryRegisterField(CharacterBlackboardFields.Player_Camera_Main, _camera);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Obj_Main, this.gameObject);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Weapon_Projectile_LayerMaskToHit, _definitions.ProjectilesLayerMaskToHit);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Weapon_Sword_LayerMaskToHit, _definitions.SwordLayerMaskToHit);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_TeamMask, _definitions.TeamMask);
            return blackboard;
        }
        internal override InfluenceCore CreateInfluenceCore()
        {
            var stun = new Stun();
            _health = new(_definitions.Health.MaxPoint);
            return new(stun, _health);
        }
        internal override CharacterComponent[] GetComponents()
        {
            return new CharacterComponent[] { _humanoidController };
        }
        internal override void InitializeComponents(Blackboard blackboard)
        {
            base.InitializeComponents(blackboard);
            _humanoidController.animator.InitializeArmsAnimation(
                _animationDefinitions.HumanoidDefinitions.LeftArmDefinitions.Mask,
                _animationDefinitions.HumanoidDefinitions.RightArmDefinitions.Mask);
            _humanoidController.animator.InitializeNormalState();

            _normalState = _humanoidController.normalState;
            _animationNormalState = _humanoidController.animator.normalState;
        }
        internal override void ComponentsDispose()
        {
            _humanoidController.Dispose();
        }
        internal override CharacterBehavioursStatemachine CreateStatemachine()
        {
            var health = influenceCore.FindInfluence<Health>();
            var context = new CharacterBehavioursStateContext();
            var deathStateHelper = new DeathStateHelper(this.gameObject, null, 0);
            var deathState = deathStateHelper.state;

            blackboard.TryReadValueOrThrowException<LocomotionCore>(CharacterBlackboardFields.Character_Locomotion_Core, out var locomotionCore);
            deathStateHelper.LocomotionCore = locomotionCore.internalCore;

            var statemachine = new CharacterBehavioursStatemachine(context, this.gameObject.name + "_statemachine");
            statemachine.AddState(_normalState);
            statemachine.AddState(deathState);

            var n_d = new BlendingTransition<object>(_normalState, deathState, () => !_health.IsAlive, null, _definitions.GetTransitionOptions(BehavioursTransition.Normal_Death));
            statemachine.AddTransitionFor(n_d);
            _deathState = deathState;
            return statemachine;
        }
        internal CharacterBehavioursStatemachine CreateStatemachine_Obsolete()
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
        internal override CharacterAnimationStateMachine CreateAnimationStatemachine(CAnimator animator)
        {
            var health = influenceCore.FindInfluence<Health>() ?? throw new InfluenceNotExistInCoreException<Health>();
            var deathState = new Humanoid.Animations.DeathState(_deathState.Timeline, animator.controller, _animationDefinitions.Death);


            var statemachine = new CharacterAnimationStateMachine(this.gameObject.name + "_animation_statemachine");
            statemachine.AddState(_animationNormalState);
            statemachine.AddState(deathState);

            var n_d = new BlendingTransition<object>(_animationNormalState, deathState, () => !_health.IsAlive, null, _definitions.GetTransitionOptions(BehavioursTransition.Normal_Death));
            statemachine.AddTransitionFor(n_d);

            return statemachine;
        }
        internal CharacterAnimationStateMachine CreateAnimationStatemachine_Obsolete(CAnimator animator)
        {

            var stun = influenceCore.FindInfluence<Stun>() ?? throw new ArgumentNullException("stun");
            var health = influenceCore.FindInfluence<Health>() ?? throw new ArgumentNullException("health");

            var stunningState = new Humanoid.Animations.StunningState(stun.Timeline, animator.controller, _animationDefinitions.Stunning);
            var deathState = default(Humanoid.Animations.DeathState);

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

        public GameObject GetItem(uint key)
        {
            return _bodyParts.GetItem(key);
        }

        internal override CharacterAccessor SetAccessorToObj(GameObject obj)
        {
            return obj.AddComponent<C_0Accessor>();
        }
    }
}
