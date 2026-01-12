using System;

namespace Tests.Interaction.Targets
{
    [Obsolete]
    public interface ISphereTriggerTargetsCatcher_Obsolete : ITargetsCatcher
    {
        public float Radius { get; set; }
    }
}
