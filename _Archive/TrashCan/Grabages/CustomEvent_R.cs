using System;
using UnityEngine;

namespace Assets.Scripts
{
    internal abstract class CustomEvent<R> : CustomEventBase
    {
        public abstract R Invoke();
    }
}
