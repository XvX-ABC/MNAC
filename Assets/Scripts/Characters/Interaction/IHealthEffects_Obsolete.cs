using System;

namespace Tests.Characters.Interaction
{
    [Obsolete]
    public interface IHealthEffects_Obsolete
    {
        public float MaxPoint { get; set; }
        public float MinPoint { get; set; }
        public float Point { get; set; }
        public bool Enabled { get; set; }
    }
}
