using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace Tests.Utilities.MTrees
{
    public class MTree : IEnumerable<IMTNode>
    {
        protected class Enumerator : IEnumerator<IMTNode>
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
        protected IMTNode root;
     protected   Enumerator enumerator;
        public IMTNode Root { get => root; }
        public MTree(IMTNode root)
        {
            this.root = root ?? throw new ArgumentNullException(nameof(root));
            enumerator = new(this.root);
        }
        protected MTree() { }
        IMTNode FindNode(Guid id)
        {
            return this.FirstOrDefault(node => node.ID == id);
        }
        IEnumerator FindNodeWithCoroutine(Guid id, Action<IMTNode> successfulAction)
        {
            var enumerator = this.enumerator;
            do
            {
                var cnode = enumerator.Current;
                if (cnode.ID == id)
                {
                    successfulAction?.Invoke(cnode);
                    yield break;
                }
                yield return null;
            } while (this.enumerator.MoveNext());

        }
        public IEnumerator<IMTNode> GetEnumerator()
        {
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
