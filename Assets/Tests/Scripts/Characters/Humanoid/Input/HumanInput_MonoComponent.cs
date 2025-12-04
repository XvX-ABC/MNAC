using Tests.Utilities.Blackboards;
using UnityEngine;

namespace Tests.Characters.Humanoid.Interaction.Input
{
    internal class HumanInput_MonoComponent : HumanoidComponent
    {
        [SerializeField]
        HumanInput _input;
        public static implicit operator HumanInput(HumanInput_MonoComponent mc)
        {
            return mc._input;
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Input_Main_Base, _input.BaseInput);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Input_Main, _input);
        }
        public override void Dispose()
        {
            blackboard.TryUnregisterField(CharacterBlackboardFields.Character_Input_Main_Base, _input.BaseInput);
            blackboard.TryUnregisterField(CharacterBlackboardFields.Character_Input_Main, _input);
            base.Dispose();
        }
    }
}
