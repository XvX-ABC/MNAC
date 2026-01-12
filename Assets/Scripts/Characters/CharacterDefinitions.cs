using UnityEngine;

namespace Tests.Characters
{
    public class CharacterDefinitions : MonoBehaviour, ICharacterDefinitions
    {

        [SerializeField]
        float _deathDurationTime;

        public float DeathDurationTime { get => _deathDurationTime; set => _deathDurationTime = value; }
    }
}
