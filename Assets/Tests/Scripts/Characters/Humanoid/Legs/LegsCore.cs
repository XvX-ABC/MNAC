using Tests.TPhysics;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.Characters.Humanoid.Legs
{
    public class LegsCore : ComponentBase_MonoComponent
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
            node.AddChild(_leftLeg.node);
            node.AddChild(_rightLeg.node);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Legs_Core, this);
        }
    }
}
