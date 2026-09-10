using System;
using UnityEngine;

namespace MNAC.TPhysics.Locomotion
{
    /// 常开观察模块：把最近 <see cref="SampleQuantity"/> 帧速度（世界系）的竖直分量
    /// 均值分类为 Holding/Ascending/Descending，写入 Context.VerticalPosture。
    /// 以普通模块形式插入模块链（SetModules 后置为启用），需要时排在行为模块之前。
    [Serializable]
    public class VerticalPostureEvaluator : LocomotionModuleBase
    {
        [SerializeField] int _sampleQuantity;
        [NonSerialized] Vector3[] _samples;
        [NonSerialized] int _head;

        public VerticalPostureEvaluator() : this(0, 0)
        {
        }

        public VerticalPostureEvaluator(int sampleQuantity, int priority = 0) : base(priority)
        {
            _samples = Array.Empty<Vector3>();
            _head = 0;
            SampleQuantity = sampleQuantity;
        }

        public int SampleQuantity
        {
            get => _sampleQuantity;
            set
            {
                _sampleQuantity = Mathf.Max(0, value);
                EnsureBuffer();
                Enabled = _sampleQuantity > 0;
            }
        }

        void EnsureBuffer()
        {
            if (_samples == null || _samples.Length != _sampleQuantity)
            {
                _samples = new Vector3[Mathf.Max(0, _sampleQuantity)];
                _head = 0;
            }
        }

        public override Context OnUpdate(Context context)
        {
            if (_sampleQuantity == 0)
                return context;
            EnsureBuffer(); // 反序列化后运行时缓冲不会恢复，需按序列化数量重建
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
