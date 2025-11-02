using System;
using Tests.Characters.MountPoints;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms
{
    [Serializable]
    internal class MountPoint : Tests.Characters.MountPoints.MountPoint
    {
        public override GameObject LoadObj
        {
            get => base.LoadObj;
            set
            {
                var s = field switch
                {
                    MountPointFields.Enum.Left_Arm_Hand_Weapon => "_left",
                    MountPointFields.Enum.Right_Arm_Hand_Weapon => "_right",
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
