using System;

namespace Tests.Characters
{
    public static class CharacterBlackboardFields
    {
        static CharacterBlackboardFields()
        {
            FieldChangeHandler = Guid.NewGuid();
            TargetsCatcher = Guid.NewGuid();
            Input = Guid.NewGuid();
            WeaponCore = Guid.NewGuid();
        }
        public static readonly Guid FieldChangeHandler;
        public static readonly Guid TargetsCatcher;
        public static readonly Guid Input;
        public static readonly Guid WeaponCore;
    }

}
