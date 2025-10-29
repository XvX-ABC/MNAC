using System;

namespace Tests.Locomotion_Obsolete.Animation
{

    public interface IAirLocomotionAnimationDefinitions
    {
        [Serializable]
        public class DescendingDefinitions
        {
            public string EnterParamName;
        }
        [Serializable]
        public class FlyingDefinitions
        {
            public string EnterParamName;
        }
        public DescendingDefinitions Descending { get; }
        public FlyingDefinitions Flying { get; }
        [Obsolete]
        public string EnterParamName { get; }
        [Obsolete]
        public string DescendingEntryParamName { get; }
        [Obsolete]
        public string XParamName { get; }
        [Obsolete]
        public string YParamName { get; }


        [Obsolete]
        public string DescentClipName { get; }
        [Obsolete]
        public string NextStateClipName { get; }
        [Obsolete]
        public float V0 { get; }
    }
}