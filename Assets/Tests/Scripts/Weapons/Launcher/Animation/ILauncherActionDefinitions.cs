namespace Assets.Tests.Scripts.Weapons
{
    public interface ILauncherActionDefinitionsEditor
    {
#if UNITY_EDITOR
        public float CoverOpenOrCloseDuration { get; set; }
        public float MagazineFullOrEmptyDuration { get; set; }
        public void Save();
        public void Load();
#endif
    }
    public interface ILauncherActionDefinitions
    {
        public float CoverOpenOrCloseDuration { get; }
        public float MagazineFullOrEmptyDuration { get; }
        //public Reload Reload { get; }
        //public Cover Cover { get; }
        //public MagazineModule Magazine { get; }
    }
}
