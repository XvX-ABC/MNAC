using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tests.Interaction;
using Tests.Weapons.MissileLauncher;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace Tests.Weapons.Launcher
{
    public class LauncherActionController : MonoBehaviour
    {
        [SerializeField]
        GameObject _target;
        [SerializeField]
        KeyCode _launchCode;
        [SerializeField]
        KeyCode _reloadCode;
        [SerializeField]
        KeyCode _supplyCode;
        [SerializeField]
        int _supplyQuantity;
        IMissileLauncher _launcher;
        private void Awake()
        {
            _launcher = GetComponent<IMissileLauncher>();
        }
        private void Start()
        {
            _launcher.Target = _target.GetComponent<IGameObjTarget>();
        }
        private void Update()
        {
            if (UInput.GetKeyDown(_launchCode))
            {
                Debug.Log("Launch start");
                _launcher.StartLaunch();
            }
            else if (UInput.GetKeyDown(_reloadCode))
            {
                Debug.Log("Start reload");
                _launcher.StartReload();

            }
            else if (UInput.GetKeyDown(_supplyCode))
            {
                Debug.Log("Start supply");
                _launcher.Fill(_supplyQuantity);
            }
        }
    }
}
