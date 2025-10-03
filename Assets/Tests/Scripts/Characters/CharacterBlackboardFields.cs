using System;
using Tests.Utilities.Composable;

namespace Tests.Characters
{
    public static class CharacterBlackboardFields
    {
        static CharacterBlackboardFields()
        {
            Character_Obj_Main = Guid.NewGuid();
            Character_Interaction_Main_Info = Guid.NewGuid();
            World = Guid.NewGuid();

            Character_Animation_Animator = Guid.NewGuid();
            Character_Animation_Graph = Guid.NewGuid();

            Character_Camera_Main = Guid.NewGuid();

            Character_Legs_Core = Guid.NewGuid();

            Character_Locomotion_Core = Guid.NewGuid();

            Character_Influence_Core = Guid.NewGuid();

            FieldChangeHandler = MiddlewareFields.FieldChangeHandler;
            TargetsCatcher = Guid.NewGuid();

            Character_Input_Main = Guid.NewGuid();

            Character_Weapon_Core = Guid.NewGuid();
            Character_Weapon_LeftArm_Armed = Guid.NewGuid();
            Character_Weapon_RightArm_Armed = Guid.NewGuid();

            AnimationPB = Guid.NewGuid();
            Rigidbody = Guid.NewGuid();
            GroundDetector = Guid.NewGuid();
        }
        public static readonly Guid Character_Obj_Main;
        public static readonly Guid Character_Interaction_Main_Info;
        public static readonly Guid Character_Animation_Animator;
        public static readonly Guid Character_Animation_Graph;
        public static readonly Guid Character_Camera_Main;
        public static readonly Guid Character_Legs_Core;
        public static readonly Guid Character_Influence_Core;
        public static readonly Guid FieldChangeHandler;
        public static readonly Guid TargetsCatcher;
        public static readonly Guid Character_Input_Main;
        public static readonly Guid Character_Weapon_Core;
        public static readonly Guid Character_Weapon_LeftArm_Armed;
        public static readonly Guid Character_Weapon_RightArm_Armed;
        public static readonly Guid AnimationPB;
        public static readonly Guid Rigidbody;
        public static readonly Guid GroundDetector;
        public static readonly Guid World;
        public static readonly Guid Character_Locomotion_Core;
    }

}
