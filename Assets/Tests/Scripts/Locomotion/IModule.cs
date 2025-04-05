namespace Tests.Locomotion
{
    public interface IModule
    {
        public void OnUpdate(Context context) { }
        public void OnFixedUpdate(Context context);
    }

}