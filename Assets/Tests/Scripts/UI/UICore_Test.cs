using System;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.UI
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
