using Tests.Locomotion;

namespace Tests.Environment
{
    public interface IModule
    {
        public void OnUpdate(Context context) { }
        public void OnFixedUpdate(Context context);
    }

}