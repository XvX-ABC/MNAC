using Tests.TPhysics;
using UnityEngine;

namespace Tests.Characters.Legs
{
    public class LegsCore : CharacterComponentBase_MonoComponent
    {
        [SerializeField]
        LegCore _leftLeg;
        [SerializeField]
        LegCore _rightLeg;
        public float Weight
        {
            get => _leftLeg.Weight;
            set
            {
                _leftLeg.Weight = value;
                _rightLeg.Weight = value;
            }
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            this.node.AddChild(_leftLeg.node);
            this.node.AddChild(_rightLeg.node);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Legs_Core, this);
        }
    }
}
