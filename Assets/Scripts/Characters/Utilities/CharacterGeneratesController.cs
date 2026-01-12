using System;
using System.Collections.Generic;
using Tests.Interaction;
using Tests.Utilities;
using UnityEngine;
using Utils;
using Random = Unity.Mathematics.Random;

namespace Tests.Characters.Utilities
{

    internal class CharacterGeneratesController<T> : MonoBehaviour where T : CharacterBase, IDamageableWithCallback
    {
        #region internal classed
        internal class HealthCallback : IHealthCallback
        {
            List<T> _generatedCharacters;

            public HealthCallback(List<T> generatedCharacters)
            {
                _generatedCharacters = generatedCharacters;
            }

            public float TriggerProportion => 0;

            public bool RepetitiveExecution => false;

            public void Execute(GameObject obj, IHealth health)
            {
                _generatedCharacters.Remove(obj.GetComponent<T>());
            }
        }
        [Serializable]
        internal class GenerationPointsCircularManager
        {
            [SerializeField]
            internal float _radius = 50;
            [SerializeField]
            internal Transform _center;
            [SerializeField]
            uint _amount = 10;
            [SerializeField]
            Vector3 _offset;
            GenerationPoint[] _respawnPoints;
            Random _random;
            internal GameObject ownerObj;
            Vector3 _centerPos { get => (_center == null ? ownerObj.transform.position : _center.position) + _offset; }
            public GenerationPointsCircularManager()
            {
                _random = new Random((uint)this.GetHashCode());
            }
            public void GeneratePoints(Transform parent)
            {
                if (_respawnPoints == null)
                    _respawnPoints = new GenerationPoint[_amount];
                for (int i = 0; i < _amount; i++)
                {
                    var obj = new GameObject("RespawnPoint_" + i);
                    obj.transform.parent = parent?.transform;
                    obj.transform.PutInParent(parent);
                    var point = obj.AddComponent<GenerationPoint>();
                    _respawnPoints[i] = point;
                }
            }

            public GenerationPoint GetPoint()
            {
                if (_respawnPoints == null || _respawnPoints.Length == 0)
                    return null;

                var v = _random.NextInt(0, _respawnPoints.Length);
                var chosenRespawnPoint = _respawnPoints[v];

                float angle = _random.NextFloat(0f, 2 * Mathf.PI);
                var center = _centerPos;
                float x = center.x + _radius * Mathf.Cos(angle);
                float z = center.z + _radius * Mathf.Sin(angle);
                float y = center.y;

                chosenRespawnPoint.transform.position = new Vector3(x, y, z);

                return chosenRespawnPoint;
            }
            internal void OnDrawGizmosSelected()
            {
                if (ownerObj != null)
                {
                    Gizmos.color = Color.white;
                    var pos = _centerPos;
                    GizmosExtensions.DrawWireCircle(pos, _radius);
                }
                if (_respawnPoints != null)
                {
                    for (int i = 0; i < _respawnPoints.Length; i++)
                    {
                        var p = _respawnPoints[i];
                        Gizmos.color = Color.blue;
                        GizmosExtensions.DrawWireSphere(p.transform.position, 3);
                    }
                }
            }
        }
        #endregion

        [SerializeField]
        CharacterPrefab<T> _characterPrefab;
        [SerializeField]
        uint _maxAmount;
        [SerializeField]
        KeyCode _respawnKey;

        [SerializeField]
        List<T> _characters;
        HealthCallback _healthCallback;
        [SerializeField]
        GenerationPointsCircularManager _generationPointsManager;
        Func<bool> _triggerFuncs;

        public Func<bool> TriggerFuncs { get => _triggerFuncs; set => _triggerFuncs = value; }

        private void Awake()
        {
            _triggerFuncs = () => UnityEngine.Input.GetKeyDown(_respawnKey);
            _generationPointsManager.ownerObj = this.gameObject;
            _healthCallback = new(_characters);
        }
        private void Start()
        {
            _generationPointsManager.GeneratePoints(this.transform);
        }
        private void Update()
        {
            if (_maxAmount >= _characters.Count && (_triggerFuncs == null ? _triggerFuncs() : false))
            {
                Generate();
            }
        }
        GenerationPoint GetPoint()
        {
            return _generationPointsManager.GetPoint();
        }
        public void Generate()
        {
            if (_maxAmount <= _characters.Count)
                return;
            var generatesPoint = GetPoint();
            _characterPrefab.Parent = generatesPoint.transform;
            _characterPrefab.Load();
            var character = _characterPrefab.Resource;
            character.name = _characterPrefab.Resource.name + "_" + _characters.Count;
            character.HP.AddCallback(_healthCallback);
            character.transform.SetParent(null);
            _characters.Add(character);
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            GizmosExtensions.DrawWireCapsule(this.transform.position, 1, 3);
            _generationPointsManager.OnDrawGizmosSelected();
        }
    }
}
