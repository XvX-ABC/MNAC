using Assets.Scripts.Arms.Actions;
using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests;
using Tests.Weapons;
using Tests.Weapons.Projectiles;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.SearchService;

namespace Assets.Tests.Scripts.Weapons
{
    public class MissileLauncher : LauncherBase
    {
        protected GameObject missileObj;
        public ITarget Target;
        protected override void Awake()
        {
            defines = GetComponent<IMissileLauncherDefines>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IMissileLauncherDefines));
            if (!Defines.AmmoOrigin.TryGetComponent<IMissile>(out _))
                throw new ComponentCantFindException(Defines.AmmoOrigin, typeof(IMissile));

        }
        protected override void OnEnable()
        {
            base.OnEnable();
            Reload();
        }
        protected override void GetAmmo(GameObject obj)
        {
            obj.transform.localPosition = defines.MagazinePosition;
            obj.transform.localRotation = Quaternion.identity;
            obj.SetActive(true);
            var missile = obj.GetComponent<IMissile>();
            missile.Enabled = false;
        }
        protected override Timeline CreateReloadTimeline()
        {
            var timeline = new Timeline(defines.ReloadDuration);
            timeline.AddPointEvent(0, _ => Reload());
            return timeline;
        }
        internal override void Reload()
        {
            base.Reload();
            if (missileObj == null)
                missileObj = ammoPool.Get();
        }

        public override void Launch()
        {
            if (!enabled || reloadTimeline.isRunning)
                return;

            if (Time.time - lastLaunchTime <= launchingInterval || ammoQuantityInMagazine <= 0)
                return;

            missileObj.transform.SetParent(null);
            var missile = missileObj.GetComponent<IMissile>();

            //missile.Target = Target;
            missile.Enabled = true;

            missileObj = null;

            ammoQuantityInMagazine--;
            lastLaunchTime = Time.time;
        }
    }
}
