using UnityEngine;
namespace Tests.Locomotion
{

    public class Gravity : IModule
    {
        IAirModule[] dependencies;
        public Gravity(params IAirModule[] modules)
        {
            dependencies = modules;
        }
        (bool, IAirModule) Check()
        {
            foreach (var d in dependencies)
                if (d.CurrentState != IAirModule.State.Descending)
                    return (false, d);
            return (true, default);
        }
        public void Update(Context context)
        {
            var (passed, module) = Check();
            if (!passed)
            {
                Debug.LogWarning($"There is a dependent module of type '{module.GetType().Name}' that has not entered 'Descending' state, so the gravity will not work.");
                return;
            }
            context.Velocity += Physics.gravity * context.DeltaTime;
        }
    }
}