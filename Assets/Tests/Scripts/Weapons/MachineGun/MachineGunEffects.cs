using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using Tests.Utilities.Timeline.Events.Point;
using Tests.Weapons.Launcher;
using Tests.Utilities.Timeline.Events;

namespace Tests.Weapons.MachineGuns
{
    public class MachineGunEffects : MonoBehaviour, ILauncherEffector
    {
        [SerializeField]
        MuzzleFlashEffect[] _flashEffects;
        MachineGun _gun;

        ILauncher_Obsolete _owner;
        Random _random;
        ILauncherDefinitions _definitions;
        ITimelineEvent _event;
        private void Awake()
        {
            _random = new Random((uint)gameObject.GetInstanceID());
            //_definitions = GetComponent<IMachineGunDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IMachineGunDefinitions));
            //_gun = GetComponent<MachineGun>() ?? throw new ComponentCantFindException(this.gameObject, typeof(MachineGun));
            //_gun.InitializationAction += g =>
            //{
            //    var timeline = g.LaunchDurationTimeline;
            //    timeline.AddPointEvent(0, _ => { PlayFlame(); });
            //};
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

        public ILauncher_Obsolete Owner => _owner;


        void PlayFlame()
        {
            if (!enabled)
                return;
            var effect = _flashEffects[_random.NextInt(0, _flashEffects.Length)];
            effect.Play();
        }
        void FollowOwnerPlay(ILauncher_Obsolete launcher)
        {
            PlayFlame();
        }
        public void Initialize(ILauncher_Obsolete owner)
        {
            _owner = owner;
            _definitions = _owner.Definitions;
            _owner.LaunchAction += FollowOwnerPlay;
        }

        public void Dispose()
        {
            _owner.LaunchAction -= FollowOwnerPlay;
        }
    }
}
