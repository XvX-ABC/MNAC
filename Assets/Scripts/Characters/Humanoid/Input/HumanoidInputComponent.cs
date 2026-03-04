using System.ComponentModel;
using MNAC.Utilities.Blackboards;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Input
{
    internal abstract class HumanoidInputComponent : HumanoidComponent
    {
        protected abstract IHumanoidInput humanInput { get; }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Input_Main_Base, humanInput.BaseInput);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Input_Main, humanInput);
        }
        public override void Dispose()
        {
            blackboard.TryUnregisterField(CharacterBlackboardFields.Character_Input_Main_Base, humanInput.BaseInput);
            blackboard.TryUnregisterField(CharacterBlackboardFields.Character_Input_Main, humanInput);
            base.Dispose();
        }
    }
}
