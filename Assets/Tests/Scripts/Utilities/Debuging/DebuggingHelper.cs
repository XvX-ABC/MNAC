using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Tests.Utilities
{
#if UNITY_EDITOR
    public class DebuggingHelper
    {
        public static string GetCurrentCallerInfo([CallerMemberName] string callerMemberName = "")
        {
            return callerMemberName;
        }
        
    }
#endif
}
