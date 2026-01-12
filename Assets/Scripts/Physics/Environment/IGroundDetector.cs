using System.Collections.Generic;
using UnityEngine;

namespace Tests.TPhysics.Environment
{
    public interface IGroundDetector
    {
        bool Enabled { get; set; }
        public Vector3 GroundsNormal { get; }
        IReadOnlyList<Ground> Grounds { get; }
        GroundVerticalProbe Probe { get; }

        void OnLateUpdate();
    }
}