using System;
using UnityEngine;

namespace Assets.Scripts
{
    internal abstract class CustomEvent<T0, R> : CustomEventBase
    {

        public abstract R Invoke(T0 param);
    }
}
