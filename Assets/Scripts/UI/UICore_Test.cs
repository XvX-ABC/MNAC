using System;
using MNAC.Utilities.Blackboards;
using MNAC.Utilities.Composable;
using UnityEngine;

namespace MNAC.UI
{
    [Obsolete]
    [RequireComponent(typeof(UICore))]
    public class UICore_Test : MonoBehaviour
    {
        UICore _core;
        void Awake()
        {
            _core = GetComponent<UICore>();
        }
        private void Start()
        {
            _core.Initialize(new Blackboard());
        }
    }
}
