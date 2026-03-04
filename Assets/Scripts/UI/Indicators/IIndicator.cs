using UnityEngine;

namespace MNAC.UI
{
    public interface IIndicator
    {
        float Width { get; set; }
        Vector3 LocalPosition { get; set; }
    }
}