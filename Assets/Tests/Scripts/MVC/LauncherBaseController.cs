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
    [DisallowMultipleComponent]
    public class LauncherBaseController : MonoBehaviour, IController
    {
        [SerializeField]
        protected string url;
        protected ILauncher launcher;
        public string URL => url;

        protected virtual void Awake()
        {
            this.launcher = GetComponent<ILauncher>();
        }


        protected void OnEnable()
        {
            MVCCore.RegisterController(this);
        }
        protected void OnDisable()
        {
            MVCCore.UnregisteredController(this);
        }
        internal T ConvertLauncherOfBase<T>() where T : ILauncher
        {
            if (this.launcher is T mlauncher)
                return mlauncher;
            else
                throw new Exception($"The launcher of base must is the '{typeof(T)}' type or is the derived type of that");
        }
        [RequestMapping("/{c_url}/Reload/DurationEvent/Register")]
        public void RegisterDurationEventOfReload(IRequest<Action<float>> request, IResponse response)
        {
            launcher.ReloadTimeline.UpdateAction += request.Data;
            response.Code = (ushort)ResponseCode.Succeeded;
        }
    }
}
