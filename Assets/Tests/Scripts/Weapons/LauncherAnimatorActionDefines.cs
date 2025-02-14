using UnityEngine;

namespace Assets.Tests.Scripts.Weapons
{
    public class LauncherAnimatorActionDefines : MonoBehaviour, ILauncherAnimatorActionDefines
    {
        [SerializeField]
        PrepareLaunch _prepareLaunch;
        [SerializeField]
        Reload _reload;

        public PrepareLaunch PrepareLaunch { get => _prepareLaunch; set => _prepareLaunch = value; }
        public Reload Reload { get => _reload; set => _reload = value; }

        public Cover Cover => throw new System.NotImplementedException();

        public MagazineModule Magazine => throw new System.NotImplementedException();
    }
}
