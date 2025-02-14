using FoundationStone.UI.Tests.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Weapons;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons.MVC
{
    public class MultiMissileLauncherController : MissileLauncherController
    {
        protected new MultiMissileLauncher launcher;
        protected override void Awake()
        {
            base.Awake();
            launcher = ConvertLauncherOfBase<MultiMissileLauncher>();
        }
        [RequestMapping("/{c_url}/SingleLauncher/Reload/DurationEvent/Register")]
        public void RegisterDurationEventOfReloadForSingleLauncher(IRequest<(byte, Action<float>)> request, IResponse response)
        {
            var num = request.Data.Item1;
            var l = launcher.subLaunchers[num];
            l.ReloadTimeline.UpdateAction += request.Data.Item2;
            response.Code = (ushort)ResponseCode.Succeeded;
        }
    }
}
