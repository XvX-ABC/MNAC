using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace Tests.Utilities.MTrees
{
    internal class MTree<T> : IEnumerable<IMNode<T>>
    {
        class Enumerator : IEnumerator<IMNode<T>>
        {
            IMNode<T> _root;
            IMNode<T> _current;
            Stack<IMNode<T>> _stack;
            public Enumerator(IMNode<T> root)
            {
                _root = root;
                _current = root;
                _stack = new();
                _stack.Push(_root);
            }
            public IMNode<T> Current => _current;

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
        IMNode<T> _root;
        Enumerator _enumerator;
        public IMNode<T> Root { get => _root; }
        public MTree(IMNode<T> root)
        {
            _root = root ?? throw new ArgumentNullException(nameof(root));
            _enumerator = new(_root);
        }
        IMNode<T> FindNode(Guid id)
        {
            return this.FirstOrDefault(node => node.ID == id);
        }
        IEnumerator FindNodeWithCoroutine(Guid id, Action<IMNode<T>> successfulAction)
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
        public IEnumerator<IMNode<T>> GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
