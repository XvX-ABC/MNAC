using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tests.Utilities
{
    internal class GameObjectDestroyer : MonoBehaviour
    {
        [SerializeField]
        KeyCode _keyCode;
        [SerializeField]
        GameObject _obj;

        public GameObject Obj { get => _obj; set => _obj = value; }
        public KeyCode KeyCode { get => _keyCode; set => _keyCode = value; }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(_keyCode) && _obj != null)
                GameObject.Destroy(_obj);
        }
    }
}
