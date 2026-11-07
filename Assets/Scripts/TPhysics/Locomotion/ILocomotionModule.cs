namespace MNAC.TPhysics.Locomotion
{
    /// 一个可被 LocomotionCore 按 Start → Update → End 生命周期驱动的运动模块。
    public interface ILocomotionModule
    {
        bool Enabled { get; set; }
        /// 执行顺序：数值升序（小的先执行）；同值按装配列表顺序稳定排列。
        int Priority { get; }
        Context Start(Context context);
        Context Update(Context context);
        Context End(Context context);
    }
}
