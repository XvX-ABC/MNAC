using System;

namespace Tests.Locomotion_Obsolete.Animation
{
    public interface IBoostingLocomotionAnimatorDefinitions
    {
        public string EnterParamName { get; }
        [Obsolete]
        public string PreparationMultiplierParamName { get; }
        public string DurationMultiplierParamName { get; }
        public float BoostingClipLength { get; }
        [Obsolete]
        public string ToJumpParamName { get; }
        [Obsolete]
        public float PreparatoryProportion { get; }

    }
}