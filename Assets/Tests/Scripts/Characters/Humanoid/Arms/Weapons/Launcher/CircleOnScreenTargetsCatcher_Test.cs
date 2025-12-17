using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Player;
using Tests.UI;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    public class CircleOnScreenTargetsCatcher_Test : MonoBehaviour
    {
        [SerializeField]
        Camera _camera;
        [SerializeField]
        CircleOnScreenTargetsCatcherDefinitions _definitions;
        [SerializeField]
        GameObject _ownerObj;
        [SerializeField]
        RingCatcher _ringCatcher;
        [SerializeField]
        IndicatorsManager _targetsLockManager;
        [SerializeField]
        PlayerBaseInput _input;


        CircleOnScreenTargetsCatcher _catcher;
        private void Awake()
        {
            _catcher = new CircleOnScreenTargetsCatcher(_camera, _input, _ownerObj, _ringCatcher, _targetsLockManager, _definitions);
        }
        private void Start()
        {
            StartCoroutine(_catcher.FilterUpdateWithCoroutine());
            StartCoroutine(_catcher.CatcherUpdateWithCoroutine());
        }
        private void LateUpdate()
        {
            _catcher.LateUpdate();
        }

    }
}
