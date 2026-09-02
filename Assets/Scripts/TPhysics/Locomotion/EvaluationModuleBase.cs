namespace MNAC.TPhysics.Locomotion
{
    public abstract class EvaluationModuleBase : IEvaluationModule
    {
        protected bool enabled;
        protected int priority;
        public virtual bool Enabled { get => enabled; set => enabled = value; }


        int ILocomotionModule.Priority { get => priority; }

        public LocomotionModuleState State => throw new System.NotImplementedException();

        protected EvaluationModuleBase()
        {
        }


        public virtual Context Update(Context context)
        {
            return context;
        }

        public Context Start(Context context)
        {
            throw new System.NotImplementedException();
        }

        public Context End(Context context)
        {
            throw new System.NotImplementedException();
        }
    }
}