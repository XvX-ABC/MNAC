namespace Tests.Locomotion
{
    public interface ICollisionDetector
    {
        void OnFixedUpdate();
        public void OnColliderEnter(CollisionContext context);
        public void OnColliderStay(CollisionContext context);
        public void OnColliderExit(CollisionContext context);
    }
}