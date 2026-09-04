using UnityEngine;

namespace MNAC.TPhysics.Locomotion
{
    /// 常开观察模块：把最近 <see cref="SampleQuantity"/> 帧速度（世界系）的竖直分量
    /// 均值分类为 Holding/Ascending/Descending，写入 Context.VerticalPosture。
    /// 以普通模块形式插入模块链：AddModule_InsertByPriority(module, enabled:true)，
    /// 需要时给最低优先级，确保先于行为模块读取姿态。
    public class VerticalPostureEvaluator : LocomotionModuleBase
    {
        Vector3[] _samples;
        int _head;

        public VerticalPostureEvaluator(int sampleQuantity, int priority = 0) : base(priority)
        {
            _samples = System.Array.Empty<Vector3>();
            SampleQuantity = sampleQuantity;
            Enabled = true;
        }

        public int SampleQuantity
        {
            get => _samples.Length;
            set
            {
                _samples = new Vector3[Mathf.Max(0, value)];
                _head = 0;
                Enabled = _samples.Length > 0;
            }
        }

        public override Context OnUpdate(Context context)
        {
            if (_samples.Length == 0)
                return context;
            var velocity = context.world.InverseTransformVector(context.CurrentVelocity);
            _samples[_head] = velocity;
            _head = (_head + 1) % _samples.Length;
            context.verticalPosture = Evaluate();
            return context;
        }

        VerticalPosture Evaluate()
        {
            var sum = 0f;
            for (int i = 0; i < _samples.Length; i++)
                sum += _samples[i].y;
            var mean = sum / _samples.Length;
            return mean switch
            {
                > 0f => VerticalPosture.Ascending,
                < 0f => VerticalPosture.Descending,
                _ => VerticalPosture.Holding
            };
        }
    }
}
