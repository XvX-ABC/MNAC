using UnityEngine;
namespace Utilities.Timeline
{
    public interface ITimeGenerator
    {
        public float Time { get; }
        public void UpdateRange(Vector2 range);
    }
}
