using System;
using System.Collections.Generic;

namespace MNAC.Interaction

{
    public interface ITargetsCatcher_New<T> : ICatcher<T> where T : ITarget_New
    {
    }
}
