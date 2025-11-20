using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace Tests.Weapons_New.Launcher
{
    [RequireComponent(typeof(MachineGun))]
    internal class MachineGun_Test : MonoBehaviour
    {
        MachineGun _gun;
        [SerializeField]
        KeyCode _fireKey;
        [SerializeField]
        KeyCode _reloadKey;
        GUIStyle _guiStyle;
        private void Awake()
        {
            _gun = GetComponent<MachineGun>();
            _gun.FireTrigger = () => UInput.GetKey(_fireKey);
            _gun.ReloadTrigger = () => UInput.GetKeyDown(_reloadKey);
        }
        private void Update()
        {
        }
        private void OnGUI()
        {
            if (_guiStyle == null)
            {
                _guiStyle = new();
                _guiStyle.normal.textColor = Color.red;
            }
            GUILayout.BeginVertical();
            GUILayout.Label("Magazine Amount: " + _gun.MagazineAmmoAmount, _guiStyle);
            GUILayout.Label("Reserve Amount: " + _gun.ReserveAmmoAmount, _guiStyle);
            GUILayout.EndVertical();
        }
    }
}
