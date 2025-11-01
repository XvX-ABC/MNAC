using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Tests.Utilities.Timeline
{
    internal class RandomTimeGeneration : ITimeGenerator
    {
        Vector2 _range;
        Random _random;
        public float Time
        {
            get
            {
                if (_range.x != _range.y)
                    return _random.NextFloat(_range.x, _range.y);
                else
                    return _range.x;
            }
        }
        public RandomTimeGeneration(Vector2 range)
        {
            UpdateRange(range);
            _random = new((uint)GetHashCode());
        }
        public void UpdateRange(Vector2 range)
        {
            _range = new Vector2(Mathf.Min(range.x, range.y), Mathf.Max(range.x, range.y));
        }

        public void UpdateRange(System.Numerics.Vector2 range)
        {
            throw new System.NotImplementedException();
        }
    }
}
