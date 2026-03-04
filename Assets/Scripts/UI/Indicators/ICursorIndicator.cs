using MNAC.Interaction;
using UnityEngine;

namespace MNAC.UI
{
    public interface ICursorIndicator : ICursorController
    {
        Camera Camera { get; set; }
        float Width { get; set; }
    }
}