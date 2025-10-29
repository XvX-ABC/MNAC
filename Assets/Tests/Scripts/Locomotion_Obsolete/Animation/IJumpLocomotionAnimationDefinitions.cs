using System;
using System.Reflection;

namespace Tests.Locomotion_Obsolete.Animation
{
    public interface IJumpLocomotionAnimationDefinitions
    {
        public float AscendingClipLength { get; }
        public string AscendingMultiplierName { get; }
        [Obsolete]
        public float LandingClipLength { get; }
        [Obsolete]
        public string LandingMultiplierName { get; }
        //public string LandingValueParamName { get; }
        public string DescendingClipName { get; }
        [Obsolete]
        public float DescendingClipLength { get; }
        public string EnterParamName { get; }
        public string StateHoldingParamName { get; }
    }
}