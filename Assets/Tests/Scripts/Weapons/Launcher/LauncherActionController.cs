using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tests;
using Tests.Weapons;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons.Launcher
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
            _launcher.Target = _target.GetComponent<ITarget>();
        }
        private void Update()
        {
            if (Input.GetKeyDown(_launchCode))
            {
                Debug.Log("Launch start");
                _launcher.StartLaunch();
            }
            else if (Input.GetKeyDown(_reloadCode))
            {
                Debug.Log("Start reload");
                _launcher.StartReload();

            }
            else if (Input.GetKeyDown(_supplyCode))
            {
                Debug.Log("Start supply");
                _launcher.Supply(_supplyQuantity);
            }
        }
    }
}
