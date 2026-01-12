using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Interaction;
using UnityEditor;
using UnityEngine;

namespace Tests.Characters
{
    internal class CharacterHurter_Debug : MonoBehaviour
    {
        [SerializeField]
        CharacterBase _character;
        [SerializeField]
        float _point;
        [SerializeField]
        KeyCode _key;
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(_key))
            {
                CharactersHelper.DamageCharacter(_character, _point);
            }
        }
    }
}
