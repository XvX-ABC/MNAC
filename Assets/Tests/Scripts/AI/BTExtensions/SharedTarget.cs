using BehaviorDesigner.Runtime;

namespace Tests.AI
{
    internal class SharedTarget : SharedVariable<Target>
    {
        public static implicit operator SharedTarget(Target target)
        {
            return new SharedTarget() { Value = target };
        }
    }
}
