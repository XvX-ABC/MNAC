using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tests.Interaction
{
    [Serializable]
    public struct TeamMask
    {
        [SerializeField]
        public uint Value;
        public static bool operator ==(TeamMask a, TeamMask b)
        {
            return a.Value == b.Value;
        }
        public static bool operator !=(TeamMask a, TeamMask b)
        {
            return a.Value != b.Value;
        }

    }
}