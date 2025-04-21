namespace Tests.Locomotion.Animation
{
    public interface IBoostingLocomotionAnimatorDefinitions
    {
        public string EnterParamName { get; }
        public string PreparationMultiplierParamName { get; }
        public string DurationMultiplierParamName { get; }
        public float BoostingClipLength { get; }
        public string ToJumpParamName { get; }
        public float PreparatoryProportion { get; }

    }
}