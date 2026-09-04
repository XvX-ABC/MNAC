namespace MNAC.TPhysics.Locomotion
{
    /// 一个可被 LocomotionCore 按 Start → Update → End 生命周期驱动的运动模块。
    public interface ILocomotionModule
    {
        bool Enabled { get; set; }
        int Priority { get; }
        Context Start(Context context);
        Context Update(Context context);
        Context End(Context context);
    }
}
