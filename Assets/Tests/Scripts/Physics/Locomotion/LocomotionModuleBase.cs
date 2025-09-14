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
        protected LocomotionModuleState state;
        public LocomotionModuleBase()
        {
            world = World.Default;
            state = LocomotionModuleState.Ready;
        }
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }
        public virtual World World
        {
            get => world;
            set
            {
                world = value == null ? World.Default : value;
            }
        }

        public LocomotionModuleState State { get => state; }

        public abstract Context OnStart(Context context);
        public abstract Context OnUpdate(Context context);
        public abstract Context OnEnd(Context context);

        public Context Start(Context context)
        {
            context = OnStart(context);
            state = LocomotionModuleState.Started;
            return context;
        }

        public Context Update(Context context)
        {
            context = OnUpdate(context);
            state = LocomotionModuleState.Updating;
            return context;
        }

        public Context End(Context context)
        {
            context = OnEnd(context);
            state = LocomotionModuleState.Ended;
            return context;
        }
    }
}