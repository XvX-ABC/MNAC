using FoundationStone.UI.Tests.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Weapons.Launcher;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons.MVC
{
    [DisallowMultipleComponent]
    public class LauncherBaseController : MonoBehaviour, IController
    {
        [SerializeField]
        protected string url;
        protected ILauncher_Obsolete launcher;
        public string URL => url;

        protected virtual void Awake()
        {
            this.launcher = GetComponent<ILauncher_Obsolete>();
            if (launcher == null)
            {
                throw new Exception();
            }
        }


        protected void OnEnable()
        {
            MVCCore.RegisterController(this);
        }
        protected void OnDisable()
        {
            MVCCore.UnregisteredController(this);
        }
        internal T ConvertLauncherOfBase<T>() where T : ILauncher_Obsolete
        {
            if (this.launcher is T mlauncher)
                return mlauncher;
            else
                throw new Exception($"The launcher of base must is the '{typeof(T)}' type or is the derived type of that");
        }
        [RequestMapping("/{c_url}/ReloadTimeline/DurationEvent/Register", RequestMethod.POST)]
        public void RegisterDurationEventToReloadTimeline(IRequest<Action<float>> request, IResponse response)
        {
            var timeline = launcher.ReloadTimeline;
            if (timeline == null)
                launcher.InitializationAction += l => l.ReloadTimeline.UpdateAction += request.Data;
            else
                timeline.UpdateAction += request.Data;
            response.Code = (ushort)ResponseCode.Succeeded;
        }
        [RequestMapping("/{c_url}/DelayLaunchTimeline/DurationEvent/Register", RequestMethod.POST)]
        public void RegisterDurationEventToDelayLaunchTimeline(IRequest<Action<float>> request, IResponse response)
        {
            var timeline = launcher.DelayLaunchTimeline;
            if (timeline == null)
                launcher.InitializationAction += l => l.DelayLaunchTimeline.UpdateAction += request.Data;
            else
                timeline.UpdateAction += request.Data;
            response.Code = (ushort)ResponseCode.Succeeded;
        }
        [RequestMapping("/{c_url}/LaunchDurationTimeline/DurationEvent/Register", RequestMethod.POST)]
        public void RegisterDurationEventToLaunchDurationTimeline(IRequest<Action<float>> request, IResponse response)
        {
            var timeline= launcher.LaunchDurationTimeline;
            if (timeline == null)
                launcher.InitializationAction += l => l.LaunchDurationTimeline.UpdateAction += request.Data;
            else
                timeline.UpdateAction += request.Data;
            response.Code = (ushort)ResponseCode.Succeeded;
        }
        [RequestMapping("/{c_url}/Ammo/SpareQuantity")]
        public void GetSpareQuantity(IRequest request, IResponse<ushort> response)
        {
            response.Data = launcher.ReservesAmmoCount;
            response.Code = (ushort)ResponseCode.Succeeded;
        }
        [RequestMapping("/{c_url}/Ammo/MagazineQuantity")]
        public void GetMagazineQuantity(IRequest request, IResponse<ushort> response)
        {
            response.Data = launcher.MagazineAmmoCount;
            response.Code = (ushort)ResponseCode.Succeeded;
        }
    }
}
