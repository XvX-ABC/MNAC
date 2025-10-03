using System;

namespace Tests.UI
{
    public static class UIBlackboardFields
    {
        static UIBlackboardFields()
        {
            Input = Guid.NewGuid();
            Camera_Main = Guid.NewGuid();
            Catcher_Ring = Guid.NewGuid();
        }
        public static readonly Guid Input;
        public static readonly Guid Camera_Main;
        public static readonly Guid Catcher_Ring;
    }
}
