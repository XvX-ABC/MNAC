using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Input;
using UnityEngine;

namespace Tests.Weapons.MachineGuns
{
    [RequireComponent(typeof(CaseEjectingDevice))]
    public class CaseEjectingDevice_Test : MonoBehaviour
    {
        [SerializeField]
        GameObject _inputObj;
        IInput _input;
        CaseEjectingDevice _device;
        private void Awake()
        {
            _device = GetComponent<CaseEjectingDevice>();
            _input = _inputObj.GetComponent<IInput>() ?? throw new ComponentCantFindException(_inputObj, typeof(IInput));
        }
        private void Update()
        {
            if (_input.Fire)
            {
                _device.StartLaunch();
            }
        }

    }
}
