using System;

namespace MNAC.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DontDestroyOnLoadAttribute : Attribute
    {

    }
}
