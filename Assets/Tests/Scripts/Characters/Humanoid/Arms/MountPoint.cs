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
                var s = place switch
                {
                    MountPointPlace.Left_Hand_Weapon => "_left",
                    MountPointPlace.Right_Hand_Weapon => "_right",
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
