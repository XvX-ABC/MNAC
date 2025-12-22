using System;
using Tests.Characters;
using Tests.Characters.Humanoid.Locomotion;

namespace Tests.AI
{
    internal class AIBlackboardFields
    {
        static AIBlackboardFields()
        {
            Component_NavAgent = Guid.NewGuid();
            Character_LocomotionCore = CharacterBlackboardFields.Character_Locomotion_Core;
            Character_Obj_Main = CharacterBlackboardFields.Character_Obj_Main;
            Character_Blackboard = Guid.NewGuid();
            Character_TeamMask = CharacterBlackboardFields.Character_TeamMask;
        }
        public static readonly Guid Component_NavAgent;
        public static readonly Guid Character_LocomotionCore;
        public static readonly Guid Character_Obj_Main;
        public static readonly Guid Character_Blackboard;
        public static readonly Guid Character_TeamMask;
    }
}
