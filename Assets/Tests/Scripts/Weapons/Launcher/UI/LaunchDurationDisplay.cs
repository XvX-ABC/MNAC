using FoundationStone.UI.Tests.MVC;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Tests.Scripts.Weapons.Launcher.UI
{
    public class LaunchDurationDisplay : MonoBehaviour
    {
        [SerializeField]
        Slider _slider;
        VRequest<Action<float>> _request;
        private void Start()
        {
            _request = new("/Launcher/LaunchDurationTimeline/DurationEvent/Register", RequestMethod.POST);
            _request.Send(v => _slider.value = v, null, null);
        }
    }
}
