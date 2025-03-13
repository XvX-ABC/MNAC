using FoundationStone.UI.Tests.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Weapons.MissileLauncher;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons.MVC
{
    public class MissileLauncherController : LauncherBaseController
    {
        protected new IMissileLauncher launcher;
        protected override void Awake()
        {
            base.Awake();
            launcher=ConvertLauncherOfBase<IMissileLauncher>();
        }
  
        [RequestMapping("/{c_url}/DelayLaunch/DurationEvent/Register")]
        public void RegisterDurationEventOfDelayLaunch(IRequest<Action<float>> request, IResponse response)
        {
            launcher.DelayLaunchTimeline.UpdateAction += request.Data;
            response.Code = (ushort)ResponseCode.Succeeded;
        }

    }
}
