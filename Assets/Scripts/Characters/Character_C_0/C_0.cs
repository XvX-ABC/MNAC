using System;
using MNAC.Animations;
using MNAC.Characters.Humanoid;
using MNAC.Characters.Humanoid.Locomotion;
using MNAC.Characters.Interaction.Influences;
using MNAC.Characters.UI;
using MNAC.Interaction;
using MNAC.Interaction.Influence;
using MNAC.Interaction.Influences;
using MNAC.Players.UI;
using MNAC.States;
using MNAC.UI;
using MNAC.Utilities.Blackboards;
using UnityEditor;
using UnityEngine;
using AnimationNormalState = MNAC.Characters.Humanoid.Animations.NormalState;
using NormalState = MNAC.Characters.Humanoid.NormalState;
using Stun = MNAC.Interaction.Influence.Stun;

namespace MNAC.Characters.C_0
{
    [DefaultExecutionOrder(1)]
    [Interactable]
    internal class C_0 : CharacterBase, IC_0
    {

        [SerializeField]
        Camera _camera;
        [SerializeField]
        HumanoidBodyParts _bodyParts;
        [SerializeField]
        Collider _lockCollider;
        internal HumanoidController _humanoidController;
        NormalState _normalState;
        AnimationNormalState _animationNormalState;
        ICharacterDefinitions_C_0 _definitions;
        ICharacterAnimationDefinitions_C_0 _animationDefinitions;
        HealthWithCallback _health;
        Knockback _knockbackInfluence;
        TeamMask _teamMask;
        Action<GameObject> _diedAction;

        DeathState _deathState;
        public TeamMask TeamMask { get => _teamMask; set => _teamMask = value; }

        public bool IsAlive => _health.IsAlive;

        public float MaxPoint => _health.MaxPoint;

        public float MinPoint => _health.MinPoint;

        public float Point => _health.Point;


        [Obsolete]
        public IHealth HP => _health;

        protected override Bounds bounds => _lockCollider.bounds;

        IHealthWithCallBack IDamageableWithCallback.HP => _health;

        public Action<GameObject> DiedAction { get => _diedAction; set => _diedAction = value; }

        public IKnockback Knockback => _knockbackInfluence;

        protected override void Awake()
        {
            _humanoidController = GetComponentInChildren<HumanoidController>() ?? throw new ComponentCantFindException(this.gameObject, typeof(HumanoidComponent));
            _definitions = GetComponentInChildren<ICharacterDefinitions_C_0>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ICharacterDefinitions_C_0));
            _animationDefinitions = GetComponentInChildren<ICharacterAnimationDefinitions_C_0>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ICharacterAnimationDefinitions_C_0));
            _teamMask = _definitions.TeamMask;

            base.Awake();
        }
        protected override void Start()
        {
            base.Start();

            if (blackboard.TryReadUIValue<HealthBar_QuantityBhv>(CharacterUIBlackboardFields.Health_Bar, out var healthBar))
            {
                healthBar.slider.CurrentAmount = healthBar.slider.MaxAmount = _health.MaxPoint;
                healthBar.slider.MinAmount = _health.MinPoint;
                _health.AddCallback(healthBar);
            }
        }
        protected override void OnDestroy()
        {
            if (blackboard.TryReadUIValue<HealthBar_QuantityBhv>(CharacterUIBlackboardFields.Health_Bar, out var healthBar))
                _health.RemoveCallback(healthBar);

            base.OnDestroy();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
        }
        GUIStyle _style;
        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
            if (_style == null)
            {
                _style = new();
                _style.onNormal.textColor = Color.black;
            }
            var pos = this.transform.position;
            Handles.Label(pos, $"{this.name}\ntm: {_teamMask.Value}\n hp: {_health.Point}", _style); ;
#endif
        }
        internal override Blackboard CreateBlackboard()
        {
            var blackboard = base.CreateBlackboard();
            blackboard.TryRegisterField(CharacterBlackboardFields.Player_Camera_Main, _camera);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Obj_Main, this.gameObject);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Weapon_Projectile_LayerMaskToHit, _definitions.ProjectilesLayerMaskToHit);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Weapon_Sword_LayerMaskToHit, _definitions.SwordLayerMaskToHit);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_TeamMask, _definitions.TeamMask);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_BodyParts, _bodyParts);
            return blackboard;
        }
        internal override InfluenceCore CreateInfluences()
        {
            var stun = new Stun();
            _health = new(this.gameObject, _definitions.Health.MaxPoint);
            _knockbackInfluence = new();
            return new(stun, _health, _knockbackInfluence);
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
        void DisableColliders()
        {
            foreach (var c in colliders)
                c.enabled = false;
        }
        internal override CharacterBehavioursStatemachine CreateStatemachine()
        {
            var health = influenceCore.FindInfluence<HealthWithCallback>();
            var context = new CharacterBehavioursStateContext();
            var deathStateHelper = new DeathStateHelper(this.gameObject, null, null, _definitions.Death.DelayDestroyDuration);
            var deathState = deathStateHelper.state;

            blackboard.TryReadValueOrThrowException<LocomotionCore>(CharacterBlackboardFields.Character_Locomotion_Core, out var locomotionCore);
            _knockbackInfluence.locomotionCore = locomotionCore;
            deathStateHelper.LocomotionCore = locomotionCore;

            var statemachine = new CharacterBehavioursStatemachine(context, this.gameObject.name + "_statemachine");
            statemachine.AddState(_normalState);
            statemachine.AddState(deathState);

            var n_d = new BlendingTransition<object>(_normalState, deathState, () => !_health.IsAlive, null, _definitions.GetTransitionOptions(BehavioursTransition.Normal_Death));
            statemachine.AddTransitionFor(n_d);

            var d_n = new BlendingTransition<object>(deathState, _normalState, () => _health.IsAlive, null, _definitions.GetTransitionOptions(BehavioursTransition.Normal_Death));
            statemachine.AddTransitionFor(d_n);
            _deathState = deathState;

            InitializeUIElementsForDeathState();

            return statemachine;
        }
        void InitializeUIElementsForDeathState()
        {
            if (blackboard.TryReadUIValue<DeathPanel>(CharacterUIBlackboardFields.Death_Panel, out var deathPanel))
            {
                _deathState.DeathPanel = deathPanel;
            }
            else
                blackboard.RegisterFieldChangeAction<DeathPanel>(CharacterUIBlackboardFields.Death_Panel, (t, o, n) => { _deathState.DeathPanel = n; });

            _deathState.MainPanel = MainPlane.Instance;
        }
        internal override AnimationPlayablePartBase GetMainAnimationPlayablePart()
        {
            return _humanoidController.animator.layersMixer;
        }
        internal override CharacterAnimationStateMachine CreateAnimationStatemachine(CAnimator animator)
        {
            var health = influenceCore.FindInfluence<HealthWithCallback>() ?? throw new InfluenceNotExistInCoreException<HealthWithCallback>();
            var deathState = new Humanoid.Animations.DeathState(_deathState.Timeline, animator.controller, _animationDefinitions.Death);


            var statemachine = new CharacterAnimationStateMachine(this.gameObject.name + "_animation_statemachine");
            statemachine.AddState(_animationNormalState);
            statemachine.AddState(deathState);

            var n_d = new BlendingTransition<object>(_animationNormalState, deathState, () => !_health.IsAlive, null, _definitions.GetTransitionOptions(BehavioursTransition.Normal_Death));
            statemachine.AddTransitionFor(n_d);

            var d_n = new BlendingTransition<object>(deathState, _animationNormalState, () => _health.IsAlive, null, _definitions.GetTransitionOptions(BehavioursTransition.Normal_Death));
            statemachine.AddTransitionFor(d_n);
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
        internal override void ResetStates()
        {
            base.ResetStates();
            _health.Reset();
        }
    }
}
