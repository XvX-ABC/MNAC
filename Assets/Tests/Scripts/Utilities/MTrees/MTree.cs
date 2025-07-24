using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace Tests.Utilities.MTrees
{
    internal class MTree : IEnumerable<IMTNode>
    {
        class Enumerator : IEnumerator<IMTNode>
        {
            IMTNode _root;
            IMTNode _current;
            Stack<IMTNode> _stack;
            public Enumerator(IMTNode root)
            {
                _root = root;
                _current = root;
                _stack = new();
                _stack.Push(_root);
            }
            public IMTNode Current => _current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                if (_stack.Count == 0)
                    return false;


                var node = _stack.Pop();
                _current = node;

                var children = node.Children;
                if (children != null)
                    for (int i = children.Count - 1; i >= 0; i--)
                    {
                        var child = children[i];
                        _stack.Push(child);
                    }


                return true;
            }

            public void Reset()
            {
                _current = _root;
            }
        }
        IMTNode _root;
        Enumerator _enumerator;
        public IMTNode Root { get => _root; }
        public MTree(IMTNode root)
        {
            _root = root ?? throw new ArgumentNullException(nameof(root));
            _enumerator = new(_root);
        }
        IMTNode FindNode(Guid id)
        {
            return this.FirstOrDefault(node => node.ID == id);
        }
        IEnumerator FindNodeWithCoroutine(Guid id, Action<IMTNode> successfulAction)
        {
            var enumerator = _enumerator;
            do
            {
                var cnode = enumerator.Current;
                if (cnode.ID == id)
                {
                    successfulAction?.Invoke(cnode);
                    yield break;
                }
                yield return null;
            } while (_enumerator.MoveNext());

        }
        public IEnumerator<IMTNode> GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
