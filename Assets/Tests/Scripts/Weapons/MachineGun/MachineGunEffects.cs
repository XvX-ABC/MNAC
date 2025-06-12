using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Assets.Scripts.Utilities.Timeline.Event;

namespace Tests.Weapons.MachineGuns
{
    [RequireComponent(typeof(MachineGun))]
    public class MachineGunEffects : MonoBehaviour
    {
        [SerializeField]
        MuzzleFlashEffect[] _flashEffects;
        MachineGun _gun;
        Random _random;
        IMachineGunDefinitions _definitions;
        ITimelineEvent _event;
        private void Awake()
        {
            _random = new Random((uint)gameObject.GetInstanceID());
            _definitions = GetComponent<IMachineGunDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IMachineGunDefinitions));
            _gun = GetComponent<MachineGun>() ?? throw new ComponentCantFindException(this.gameObject, typeof(MachineGun));
            _gun.InitializationAction += g =>
            {
                var timeline = g.LaunchDurationTimeline;
                timeline.AddPointEvent(0, _ => { PlayFlame(); });
            };
            if (_flashEffects.Length == 0)
            {
                Debug.LogWarning($"This game object '{this.gameObject.name}' is disabled, because the flash effects is empty.");
                this.enabled = false;
            }
        }
        private void Start()
        {
            if (_flashEffects.Length == 0)
                this.enabled = false;
            foreach (var effect in _flashEffects)
            {
                effect.gameObject.SetActive(true);
            }
        }
        float current;
        private void Update()
        {
            current++;
        }

        void PlayFlame()
        {
            if (!enabled)
                return;
            var effect = _flashEffects[_random.NextInt(0, _flashEffects.Length)];
            effect.Play();
        }
    }
}
