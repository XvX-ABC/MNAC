using System;
using Tests.Assets;
using UnityEngine;
using static Tests.Locomotion.Animation.IAirLocomotionAnimationDefinitions;

namespace Tests.Locomotion.Animation
{
    [Serializable]
    class AirLocomotionAnimationDefinitions : IAirLocomotionAnimationDefinitions
    {
        [SerializeField]
        string _enterParamName;
        [SerializeField]
        string _descendingEntryParamName;
        [SerializeField]
        string _xParamName;
        [SerializeField]
        string _yParamName;
        [SerializeField]
        string _descentClipName;
        [SerializeField]
        string _nextStateClipName;
        [SerializeField]
        float _v0;
        [SerializeField]
        JsonAssetAgent_Managed<DescendingDefinitions> _descending;
        [SerializeField]
        JsonAssetAgent_Managed<FlyingDefinitions> _flying;

        public DescendingDefinitions Descending { get => _descending.Asset; }
        public FlyingDefinitions Flying { get => _flying.Asset; }

        [SerializeField]
        public string EnterParamName => _enterParamName;

        public string XParamName => _xParamName;

        public string YParamName => _yParamName;

        public string DescentClipName => _descentClipName;

        public string NextStateClipName => _nextStateClipName;

        public float V0 => _v0;

        public string DescendingEntryParamName { get => _descendingEntryParamName; }
    }
}