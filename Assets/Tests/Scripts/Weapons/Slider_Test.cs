using FoundationStone.UI.Tests.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Tests.Scripts.Weapons
{
    public class Slider_Test : MonoBehaviour
    {
        [SerializeField]
        Slider _slider;
        VRequest<Action<float>> _request;
        private void Start()
        {
            _request = new("/Cube/Reload/DurationEvent/Register", RequestMethod.GET);
            //_request.Send((true, v => _slider.value = v), null, null);
            _request.Send(v => _slider.value = v, null, null);
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Send");
            }
        }
    }
}
