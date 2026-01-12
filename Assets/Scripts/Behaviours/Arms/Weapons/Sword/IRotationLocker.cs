namespace Tests.Behaviours.Arms.Weapons.Sword
{
    public interface IRotationLocker
    {
        public bool IsLocked { get; }
        public void Lock();
        public void UnLock();
    }
}
