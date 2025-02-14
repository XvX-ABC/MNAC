using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Assets.Scripts.Utilities.Timeline
{
    internal class RandomTimeGeneration : ITimeGeneration
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
            _range = new Vector2(Mathf.Min(range.x, range.y), Mathf.Max(range.x, range.y));
            _random = new((uint)this.GetHashCode());
        }

    }
}
