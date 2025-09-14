using System;

namespace Tests.Characters
{
    public static class CharacterBlackboardFields
    {
        static CharacterBlackboardFields()
        {
            Character_Animator_Main = Guid.NewGuid();
            Character_Camera_Main = Guid.NewGuid();
            Character_Legs_Core = Guid.NewGuid();
            Character_Locomotion_Core = Guid.NewGuid();
            FieldChangeHandler = Guid.NewGuid();
            TargetsCatcher = Guid.NewGuid();
            Input = Guid.NewGuid();
            WeaponCore = Guid.NewGuid();
            AnimationPB = Guid.NewGuid();
            Rigidbody = Guid.NewGuid();
            GroundDetector = Guid.NewGuid();
        }
        public static readonly Guid Character_Animator_Main;
        public static readonly Guid Character_Camera_Main;
        public static readonly Guid Character_Legs_Core;
        public static readonly Guid FieldChangeHandler;
        public static readonly Guid TargetsCatcher;
        public static readonly Guid Input;
        public static readonly Guid WeaponCore;
        public static readonly Guid AnimationPB;
        public static readonly Guid Rigidbody;
        public static readonly Guid GroundDetector;
        public static readonly Guid World;
        public static readonly Guid Character_Locomotion_Core;
    }

}
