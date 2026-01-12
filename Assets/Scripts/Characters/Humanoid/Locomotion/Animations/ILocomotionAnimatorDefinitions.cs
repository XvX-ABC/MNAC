namespace Tests.Characters.Humanoid.Locomotion.Animations
{
    public interface ILocomotionAnimatorDefinitions
    {
        public string Velocity_X { get; }
        public string Velocity_Y { get; }
        public string InAir { get; }
        public float Jump_Clip_Length { get; }
        public string Jump_Trigger { get; }

        public string Jump_Multiplier { get; }

        public string Descending { get; }
        public string Boosting_Trigger { get; }
        public string Boosting_Multiplier { get; }
        float Boosting_Clip_Length { get; }
    }
}
