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
            LocomotionCore = CharacterBlackboardFields.Character_Locomotion_Core;
        }
        public static readonly Guid Component_NavAgent;
        public static readonly Guid LocomotionCore;
        internal static object Character_Blackboard;
    }
}
