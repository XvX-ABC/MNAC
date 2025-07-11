using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Behaviours.Arm;
using UnityEngine;

namespace Assets.Tests.Scripts.BodyBehaviour.Arm
{
    public class MountPoints : MonoBehaviour
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
