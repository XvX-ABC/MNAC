using Tests.Characters.Animations;
using Tests.Characters.Humanoid.Animations;

namespace Tests.Characters.C_0
{
    internal interface ICharacterAnimationDefinitions_C_0
    {
        IHumanAnimationDefinitions HumanoidDefinitions { get; }
        IStunningAnimationDefinitions Stunning { get; }
        IDeathAnimationDefinitions Death { get; }
    }
}
