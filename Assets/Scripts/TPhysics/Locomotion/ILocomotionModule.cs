using System;
using System.Reflection;

namespace MNAC.TPhysics.Locomotion
{
    public interface ILocomotionModule
    {
        public bool Enabled { get; set; }
        public int Priority { get; }
        public LocomotionModuleState State { get; }
        public Context Start(Context context);
        public Context Update(Context context);
        public Context End(Context context);
    }
}