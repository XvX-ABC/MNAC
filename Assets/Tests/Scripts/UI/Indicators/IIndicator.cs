using UnityEngine;

namespace Tests.UI
{
    public interface IIndicator
    {
        float Width { get; set; }
        Vector3 LocalPosition { get; set; }
    }
}