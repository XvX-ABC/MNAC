namespace Tests.TPhysics.Locomotion
{
    public abstract class EvaluationModuleBase : IEvaluationModule
    {
        protected bool enabled;
        protected World world;
        public bool Enabled { get => enabled; set => enabled = value; }
        protected EvaluationModuleBase()
        {
            world = World.Default;
        }
        public virtual World World
        {
            get => world;
            set
            {
                world = value == null ? World.Default : value;
            }
        }

        protected abstract Context OnUpdate(Context context);
        public Context Update(Context context)
        {
            if (!enabled)
                return context;
            return OnUpdate(context);
        }
    }
    public abstract class LocomotionModuleBase : ILocomotionModule
    {

        protected bool enabled;
        protected World world;
        public LocomotionModuleBase()
        {
            world = World.Default;
        }
        public bool Enabled { get => enabled; set => enabled = value; }
        public virtual World World
        {
            get => world;
            set
            {
                world = value == null ? World.Default : value;
            }
        }

        public abstract Context Start(Context context);
        public abstract Context Update(Context context);
        public abstract Context End(Context context);


    }
}