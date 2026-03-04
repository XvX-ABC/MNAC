using System;
using MNAC.Characters;
using MNAC.Characters.Humanoid.Locomotion;

namespace MNAC.AI
{
    internal class AIBlackboardFields
    {
        static AIBlackboardFields()
        {
            Component_NavAgent =  Guid.NewGuid();
            Character_LocomotionCore = CharacterBlackboardFields.Character_Locomotion_Core;
            Character_Obj_Main = CharacterBlackboardFields.Character_Obj_Main;
            Character_Blackboard = Guid.NewGuid();
            Character_TeamMask = CharacterBlackboardFields.Character_TeamMask;
            Character_Arm_Left_Controller = CharacterBlackboardFields.Character_Arm_Left_Controller;
            Character_Arm_Right_Controller = CharacterBlackboardFields.Character_Arm_Right_Controller;
        }
        public static readonly Guid Component_NavAgent;
        public static readonly Guid Character_LocomotionCore;
        public static readonly Guid Character_Obj_Main;
        public static readonly Guid Character_Blackboard;
        public static readonly Guid Character_TeamMask;
        public static readonly Guid Character_Arm_Left_Controller;
        public static readonly Guid Character_Arm_Right_Controller;
    }
}
