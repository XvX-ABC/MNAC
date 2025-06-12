using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Input;
using UnityEngine;

namespace Tests.Weapons.MachineGuns
{
    [RequireComponent(typeof(MachineGun))]
    [RequireComponent(typeof(CustomPlayerInput))]
    public class MachineGun_Test : MonoBehaviour
    {
        IInput _input;
        MachineGun _machineGun;
        [SerializeField]
        float _speed;
        [SerializeField]
        float _moveValue;
        bool _toRight;
        float _distance;
        private void Awake()
        {
            _input = GetComponent<CustomPlayerInput>();
            _machineGun = GetComponent<MachineGun>();
        }
        private void Update()
        {
            var d = _speed * Time.deltaTime;
            this.transform.position += _toRight ? this.transform.right*d : -this.transform.right * d;
            _distance += d;
            if (_distance >= _moveValue)
            {
                _toRight = !_toRight;
                _distance = 0f;
            }
            if (_input.Fire)
            {
                _machineGun.StartLaunch();
            }
            if (_input.Reload)
            {
                _machineGun.StartReload();
            }
        }
    }
}
