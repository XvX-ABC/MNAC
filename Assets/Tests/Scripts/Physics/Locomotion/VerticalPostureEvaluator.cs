using System;
using UnityEngine;

namespace Tests.TPhysics.Locomotion
{
    public class VerticalPostureEvaluator : EvaluationModuleBase
    {
        int _sampleQuantity;
        RingArray<Vector3> _data;
        VerticalPosture _posture;
        class RingArray<T>
        {
            T[] _array;
            int _headIndex;

            public int HeadIndex
            {
                get => _headIndex;
                set
                {
                    if (value < 0 || value > _array.Length)
                        throw new IndexOutOfRangeException();
                    _headIndex = value % _array.Length;
                }
            }

            public T this[int index]
            {
                get
                {
                    if (index < 0 || index >= _array.Length)
                        throw new IndexOutOfRangeException();
                    var findex = (_headIndex + index) % _array.Length;
                    return _array[findex];
                }
                set
                {
                    if (index < 0 || index >= _array.Length)
                        throw new IndexOutOfRangeException();
                    var findex = (_headIndex + index) % _array.Length;
                    _array[findex] = value;
                }
            }
            public int Length
            {
                get => _array.Length;
                set
                {
                    Array.Resize(ref _array, value);
                }
            }
            public RingArray(int length)
            {
                if (length < 0)
                    throw new Exception();
                _array = new T[length];
                _headIndex = 0;
            }

        }
        public VerticalPostureEvaluator(int sampleQuantity)
        {
            _data = new RingArray<Vector3>(0);
            SampleQuantity = sampleQuantity;
        }

        public int SampleQuantity
        {
            get => _sampleQuantity;
            set
            {
                _sampleQuantity = Mathf.Max(0, value);
                this.enabled = _sampleQuantity > 0;
                _data.Length = _sampleQuantity;
            }
        }
        VerticalPosture Evaluate()
        {
            var sum = Vector3.zero;
            for (int i = 0; i < _sampleQuantity; i++)
            {
                sum += _data[i];
            }
            sum /= _sampleQuantity;
            var y = sum.y;
            return GetPosture(y);
            VerticalPosture GetPosture(float y) => y switch
            {
                0 => VerticalPosture.Holding,
                > 0 => VerticalPosture.Ascending,
                < 0 => VerticalPosture.Descending,
                _ => throw new NotImplementedException()
            };
        }
        public override Context Update(Context context)
        {
            var v = this.world.InverseTransformVector(context.CurrentVelocity);
            _data[_sampleQuantity - 1] = v;
            _data.HeadIndex++;
            context.VerticalPosture = Evaluate();
            return context;
        }
    }
}