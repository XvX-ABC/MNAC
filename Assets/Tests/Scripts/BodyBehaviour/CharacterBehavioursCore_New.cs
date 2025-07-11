using BehaviorDesigner.Runtime;
using System.Collections.Generic;
using Tests.Behaviours.Arm;
using Tests.BT;
using Tests.Input;
using UnityEditor.ShaderGraph.Legacy;
using UnityEngine;

namespace Tests.Behaviours
{
    public class CharacterBehavioursCore_New : MonoBehaviour
    {
        BTree _bt;
        Sequencer _sequencer;
        [SerializeField]
        GameObject[] _nodeObjs;



        public class Sequencer : Tests.BT.Sequencer
        {
            public Sequencer(params IActionCore[] nodes)
            {
                children.AddRange(nodes);
            }
        }
        private void Start()
        {
            var input = GetComponent<IInput>();
            var list = new List<IActionCore>();
            foreach (var obj in _nodeObjs)
            {
                var node = obj.GetComponent<IActionCore>();
                if (node == null)
                    return;
                node.Input = input;
                list.Add(node);
            }
            _sequencer = new(list.ToArray());

            _bt = new BTree(_sequencer);
            _bt.RestartWhenComplete = true;
            _bt.StartWhenEnabled = true;
        }
        private void Update()
        {
            _bt.Work();
        }
    }
}
