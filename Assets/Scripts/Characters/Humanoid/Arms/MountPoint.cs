using System;
using MNAC.Characters.MountPoints;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Arms
{
    [Serializable]
    internal class MountPoint : MNAC.Characters.MountPoints.MountPoint
    {
        public override GameObject LoadObj
        {
            get => base.LoadObj;
            set
            {
                var s = place switch
                {
                    MountPointLocation.Left_Hand_Weapon => "_left",
                    MountPointLocation.Right_Hand_Weapon => "_right",
                    _ => ""
                };
                if (value != null)
                    value.name += s;
                else if (LoadObj != null)
                {
                    var obj = LoadObj;
                    var index = obj.name.LastIndexOf(s);
                    obj.name.Remove(index, s.Length);
                }
                base.LoadObj = value;
            }
        }
    }
}
