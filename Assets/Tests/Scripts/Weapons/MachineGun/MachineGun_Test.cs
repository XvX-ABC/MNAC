using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Input;
using UnityEngine;

namespace Tests.Weapons.MachineGuns
{
    [RequireComponent(typeof(CustomPlayerInput_Obsolete))]
    public class MachineGun_Test : MonoBehaviour
    {
        IInput_Obsolete _input;
        [SerializeField]
        MachineGun _machineGun;
        [SerializeField]
        float _speed;
        [SerializeField]
        float _moveValue;
        bool _toRight;
        float _distance;
        private void Awake()
        {
            _input = GetComponent<CustomPlayerInput_Obsolete>();
        }
        private void Update()
        {
            var d = _speed * Time.deltaTime;
            var ts = _machineGun.transform;
            ts.position += _toRight ? ts.right * d : -ts.right * d;
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
