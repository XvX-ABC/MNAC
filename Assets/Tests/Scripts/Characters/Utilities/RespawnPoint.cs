using System;
using Tests.Characters.Interaction;
using Tests.Interaction;
using Tests.Utilities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Tests.Characters.Utilities
{
    [Serializable]
    internal class CharacterPool<T> : MonoComponentPool<T> where T : Component, ICharacter
    {
    }
    internal class CharacterRespawnController<T> : MonoBehaviour where T : CharacterBase
    {
        [SerializeField]
        CharacterPool<T> _pool;
        [SerializeField]
        RespawnPoint[] _respawnPoints;
        [SerializeField]
        KeyCode _respawnKey;
        Random _random;
        Func<bool> _triggerFuncs;
        private void Awake()
        {
            _random = new((uint)this.gameObject.GetInstanceID());
            _triggerFuncs = () => UnityEngine.Input.GetKeyDown(_respawnKey);
        }
        private void Update()
        {
            if (_triggerFuncs?.Invoke() ?? false)
            {
                Respawn();
            }
        }
        RespawnPoint GetRespawnPoint()
        {
            if (_respawnPoints == null)
                return null;
            var v = _random.NextInt(0, _respawnPoints.Length);
            return _respawnPoints[v];
        }
        public void Respawn()
        {
            var respawnPoint = GetRespawnPoint();
            _pool.Parent = respawnPoint.transform;
            _pool.Get();

        }
    }
    internal class RespawnPoint : MonoBehaviour
    {

    }
}
