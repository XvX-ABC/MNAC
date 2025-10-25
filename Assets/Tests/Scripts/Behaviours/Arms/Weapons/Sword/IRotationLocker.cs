namespace Tests.Behaviours.Arms.Weapons.Sword
{
    public interface IRotationLocker
    {
        public bool Locked { get; }
        public void Lock();
        public void UnLock();
    }
}
