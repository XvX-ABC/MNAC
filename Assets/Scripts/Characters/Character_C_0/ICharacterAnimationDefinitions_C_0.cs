using MNAC.Characters.Animations;
using MNAC.Characters.Humanoid.Animations;

namespace MNAC.Characters.C_0
{
    internal interface ICharacterAnimationDefinitions_C_0
    {
        IHumanAnimationDefinitions HumanoidDefinitions { get; }
        IStunningAnimationDefinitions Stunning { get; }
        IDeathAnimationDefinitions Death { get; }
    }
}
