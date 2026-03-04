using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MNAC.Interaction
{
    [Serializable]
    public struct TeamMask
    {
        public static explicit operator uint(TeamMask mask)
        {
            return mask.Value;
        }
        public static explicit operator TeamMask(uint value)
        {
            return new TeamMask { Value = value };
        }
        [SerializeField]
        public uint Value;
        public bool Contains(uint num)
        {
            return (Value == 0 && num == 0) || (Value & num) > 0;
        }
        public static bool operator ==(TeamMask a, TeamMask b)
        {
            return a.Value == b.Value;
        }
        public static bool operator !=(TeamMask a, TeamMask b)
        {
            return a.Value != b.Value;
        }
        public override string ToString()
        {
            return Value.ToString();
        }

    }
}