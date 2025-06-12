using Assets.Scripts.Utilities.Timeline;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tests.Weapons.Launcher;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Tests.Weapons.MachineGuns
{
    public interface IMachineGunDefinitions : ILauncherDefinitions
    {

        public float PRS { get; }
        public GameObject CaseOrigin { get; }
    }
    public interface IMachineGun : ILauncher
    {
        public new IMachineGunDefinitions Definitions { get; }
    }

    public class MachineGun : LauncherBase, IMachineGun
    {
        public new IMachineGunDefinitions Definitions { get => (IMachineGunDefinitions)definitions; }

        protected override ITimeline CreateDelayLaunchTimeline()
        {
            return null;
        }
        protected override GameObject CreateAmmo()
        {
            var origin = definitions.AmmoOrigin;
            var obj = Instantiate(origin, MagazinePosition, this.transform.rotation);
            obj.name = origin.name + "_" + ammoPool.CountAll;
            obj.SetActive(false);
            if (obj.TryGetComponent<IBullet>(out var p))
                p.DisableAction += ReleaseAmmo;
            return obj;
        }
        protected override void GetAmmo(GameObject obj)
        {
            obj.transform.position = MuzzlePosition;
            obj.transform.rotation = this.transform.rotation;
            obj.transform.SetParent(null);
            obj.SetActive(true);
            if (obj.TryGetComponent<IBullet>(out var p))
            {
                p.Enabled = true;
                p.ShootingRay = new Ray(MuzzlePosition, this.transform.forward);
            }
        }
        protected override void DestroyAmmo(GameObject obj)
        {
            if (obj.TryGetComponent<IBullet>(out var p))
            {
                p.DisableAction -= ReleaseAmmo;
            }
        }
        public override bool StartLaunch()
        {
            if (!enabled || actionsLock.StartLaunchLocked())
                return false;
            if (ammoInMagazineQuantity <= 0)
                return false;
            Launch();
            launchDurationTimeline.Start();
            return true;
        }
        public override bool EndLaunch()
        {
            if (!enabled || actionsLock.EndLaunchLocked())
                return false;
            if (launchDurationTimeline.IsRunning)
                launchDurationTimeline.Stop();
            return true;
        }
    }
}
