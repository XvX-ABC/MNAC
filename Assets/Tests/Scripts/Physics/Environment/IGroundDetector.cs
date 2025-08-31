using System.Collections.Generic;
using UnityEngine;

namespace Tests.TPhysics.Environment
{
    public interface IGroundDetector
    {
        bool Enabled { get; set; }
        IReadOnlyList<Ground> Grounds { get; }
        GroundVerticalProbe Probe { get; }
    }
}