using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNAC.Utilities.MountPoints;
using UnityEngine;

namespace MNAC.Behaviours.Arms
{
    public class Obsolete_MountPoints : MonoBehaviour
    {
        public MountPoint[] Points;
        public MountPoint GetPoint(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            return Points.FirstOrDefault(p => p.Name.Equals(name));
        }
    }
}
