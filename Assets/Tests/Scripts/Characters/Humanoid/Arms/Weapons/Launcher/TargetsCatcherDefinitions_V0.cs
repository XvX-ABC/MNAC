using System;
using UnityEngine;

namespace Tests.Characters.Arms.Weapons.Launchers
{
    [Serializable]
    public class TargetsCatcherDefinitions_V0 : ITargetsCatcherDefinitions_V0
    {
        [SerializeField]
        string _tag;
        [SerializeField]
        ushort _filterCountOneFrame;
        [SerializeField]
        LayerMask _targetsMask;
        [SerializeField]
        float _selectionViewPortRadius;
        public string CatchingObjsTag => _tag;

        public ushort FilterAmountOneFrame => _filterCountOneFrame;

        public LayerMask TargetsMask => _targetsMask;

        public float CatchingViewPortRadius { get => _selectionViewPortRadius; }
    }
}
