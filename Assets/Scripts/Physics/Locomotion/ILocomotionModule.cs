using System;
using System.Reflection;

namespace Tests.TPhysics.Locomotion
{
    public interface ILocomotionModule
    {
        public bool Enabled { get; set; }
        [Obsolete]
        public World World { get; set; }
        public LocomotionModuleState State { get; }
        public Context Start(Context context);
        public Context Update(Context context);
        public Context End(Context context);
    }
}