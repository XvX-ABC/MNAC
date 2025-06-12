using Assets.Scripts.Utilities.Timeline;
using Cinemachine.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Weapons;
using Tests.Weapons.Launcher;
using Tests.Weapons.MachineGuns;
using UnityEditorInternal;
using UnityEngine;

namespace Tests.Weapons.MachineGuns
{
    internal class CaseEjectingDevice : LauncherBase
    {
        [SerializeField]
        GameObject _definitionsObj;
        [SerializeField]
        Vector3 _direction;
        [SerializeField, Range(0, 50)]
        float _force = 10f;
        internal new IMachineGunDefinitions definitions { get => (IMachineGunDefinitions)base.definitions; }
        protected override void Awake()
        {
            actionsLock = new();
            base.definitions = _definitionsObj.GetComponent<IMachineGunDefinitions>() ?? throw new ComponentCantFindException(_definitionsObj, typeof(MachineGunDefinitions));
            var origin = definitions.CaseOrigin;
            if (!origin.TryGetComponent<Rigidbody>(out _))
                throw new ComponentCantFindException(origin, typeof(Rigidbody));
        }
        private void OnValidate()
        {
            _direction.x = Mathf.Clamp01(_direction.x);
            _direction.y = Mathf.Clamp01(_direction.y);
            _direction.z = Mathf.Clamp01(_direction.z);
        }
        protected override ITimeline CreateDelayLaunchTimeline()
        {
            return null;
        }
        protected override ITimeline CreateReloadTimeline()
        {
            return null;
        }
        protected override GameObject CreateAmmo()
        {
            var origin = definitions.CaseOrigin;
            var obj = Instantiate(origin, this.transform);
            obj.name = origin.name + "_" + ammoPool.CountAll;
            obj.SetActive(false);
            if (obj.TryGetComponent<IEjectable>(out var e))
            {
                e.SurvivalDurationEndAction += ReleaseAmmo;
            }
            return obj;
        }
        protected override void GetAmmo(GameObject obj)
        {
            obj.transform.position = this.transform.position;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.SetParent(null);
            obj.SetActive(true);
            if (obj.TryGetComponent<IProjectile>(out var p))
                p.Enabled = true;
        }
        protected override void ReleaseAmmo(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(this.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
        }
        internal virtual void EjectCase(GameObject obj)
        {
            var rb = obj.GetComponent<Rigidbody>();
            rb.AddForceAtPosition(this.transform.TransformDirection(_direction) * _force, obj.transform.position, ForceMode.Impulse);
        }
        internal override void Launch()
        {
            var obj = ammoPool.Get();
            EjectCase(obj);
            ammoInMagazineQuantity--;
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
