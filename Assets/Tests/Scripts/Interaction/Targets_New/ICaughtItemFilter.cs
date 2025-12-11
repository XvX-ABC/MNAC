namespace Tests.Interaction
{
    public interface ICaughtItemFilter<T>
    {
        public bool CanCatch(T item);
        public bool CanRelease(T item);
    }
}
