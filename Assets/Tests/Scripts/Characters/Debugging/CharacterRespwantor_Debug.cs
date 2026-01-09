using UnityEngine;

namespace Tests.Characters
{
    internal class CharacterRespwantor_Debug : MonoBehaviour
    {
        [SerializeField]
        CharacterBase _character;
        [SerializeField]
        KeyCode _key;
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(_key))
            {
                CharactersHelper.RespawnCharacter(_character);
            }
        }
    }
}
