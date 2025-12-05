using Tests.Characters.Humanoid;
using Tests.Characters.Interaction;
using Tests.Interaction;
using Tests.Interaction.Influence;
using Tests.States;
using Tests.Utilities.Blackboards;
using UnityEngine;
using Health = Tests.Interaction.Influence.Health;
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
        HumanoidController _humanoidController;


        protected override void Awake()
        {
            base.Awake();
            _humanoidController = GetComponentInChildren<HumanoidController>() ?? throw new ComponentCantFindException(this.gameObject, typeof(HumanoidComponent));
        }
        internal override Blackboard CreateBlackboard()
        {
            var blackboard = base.CreateBlackboard();
            blackboard.TryRegisterField(CharacterBlackboardFields.Player_Camera_Main, _camera);
            return blackboard;
        }
        internal override InfluenceCore CreateInfluenceCore()
        {
            var stun = new Stun();
            var health = new Health();
            return new(stun, health);
        }

        internal override NormalState InitializeController(Blackboard blackboard)
        {
            _humanoidController.Initialize(blackboard);
            return _humanoidController.normalState;
        }

        internal override CharacterBehavioursStatemachine CreateStatemachine(NormalState normalState, InfluenceCore influenceCore)
        {
            var stun = influenceCore.FindInfluence<Stun>();
            var health = influenceCore.FindInfluence<Health>();

            var stunningState = new StunningState(stun.Timeline);

            var context = new CharacterBehavioursStateContext();
            var statmachine = new CharacterBehavioursStatemachine(context, this.gameObject.name);
            statmachine.AddState(normalState);
            statmachine.AddState(stunningState);

            var n_s = new BlendingTransition<object>(normalState, stunningState, () => stun.Enabled, null, 0.5f);
            statmachine.AddTransitionFor(n_s);

            var s_n = normalState.CreateEntryTransition(stunningState, () => !stun.Enabled, null, 0.5f, 0, 1f);
            statmachine.AddTransitionFor(s_n);

            return statmachine;
        }
    }
}
