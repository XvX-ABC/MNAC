using Tests.Locomotion_Obsolete;

namespace Tests.Environment
{
    public interface IModule
    {
        public void OnUpdate(Context context) { }
        public void OnFixedUpdate(Context context);
    }

}