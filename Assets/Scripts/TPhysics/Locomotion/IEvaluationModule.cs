namespace MNAC.TPhysics.Locomotion
{
    /// 观察/评估模块：每帧在模块链前读取 Context 并写回标记，不参与 Start/End 生命周期。
    public interface IEvaluationModule
    {
        bool Enabled { get; set; }
        Context Update(Context context);
    }
}
