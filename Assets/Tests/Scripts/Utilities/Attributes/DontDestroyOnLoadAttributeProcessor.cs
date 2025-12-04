using System;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.Utilities.Attributes
{
    internal class DontDestroyOnLoadAttributeProcessor : IAttributeProcessor
    {
        public Type AttributeType => typeof(DontDestroyOnLoadAttribute);

        public void Process(object obj, object attr)
        {
            if (obj is not MonoBehaviour mcomp)
                return;
            UnityEngine.Object.DontDestroyOnLoad(mcomp);
        }
    }
}
