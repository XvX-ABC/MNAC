using UnityEngine;
namespace Tests.Characters.Interaction
{
    public class CharacterDefinitions_T0 : MonoBehaviour, ICharacterDefinitions_T0
    {
        [SerializeField]
        float _maxHP;

        public float MaxHP { get => _maxHP; }
    }
}
