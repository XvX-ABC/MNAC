using UnityEngine;

namespace Assets.Tests.Scripts.Weapons
{
    public class LauncherAnimatorActionDefines : MonoBehaviour, ILauncherAnimatorActionDefines
    {
        [SerializeField]
        PrepareLaunch _prepareLaunch;
        [SerializeField]
        Reload _reload;
        public float CoverOpenOrCloseDuration => throw new System.NotImplementedException();

        public float MagazineFullOrEmptyDuration => throw new System.NotImplementedException();
    }
}
