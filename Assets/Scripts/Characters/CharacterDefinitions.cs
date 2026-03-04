using UnityEngine;

namespace MNAC.Characters
{
    public class CharacterDefinitions : MonoBehaviour, ICharacterDefinitions
    {

        [SerializeField]
        float _deathDurationTime;

        public float DeathDurationTime { get => _deathDurationTime; set => _deathDurationTime = value; }
    }
}
