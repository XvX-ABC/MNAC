using Tests.Interaction;
using UnityEngine;

namespace Tests.UI
{
    public interface ICursorIndicator : ICursorController
    {
        Camera Camera { get; set; }
        float Width { get; set; }
    }
}