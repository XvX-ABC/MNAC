using System;

namespace Tests.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PlayerComponentAttribute : Attribute
    {
        bool _dontDestroyOnLoad;

        public bool DontDestroyOnLoad { get => _dontDestroyOnLoad; set => _dontDestroyOnLoad = value; }
    }
}
