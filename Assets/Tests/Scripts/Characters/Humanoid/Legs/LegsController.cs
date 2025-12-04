using Tests.TPhysics;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.Characters.Humanoid.Legs
{
    internal class LegsController : HumanoidComponent
    {
        [SerializeField]
        LegController _leftLeg;
        [SerializeField]
        LegController _rightLeg;
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
            Node.AddChild(_leftLeg.Node);
            Node.AddChild(_rightLeg.Node);
            this.Weight = 1;
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Legs_Core, this);
        }
    }
}
