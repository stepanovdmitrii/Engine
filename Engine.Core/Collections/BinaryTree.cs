using System;
using System.Collections;
using System.Collections.Generic;

namespace Engine.Core.Collections
{
    public sealed class BinaryTree<TKey, TValue>: IReadOnlyCollection<TValue> where TKey : IComparable<TKey>
    {
        private readonly object _lock = new object();
        private Guid _version = Guid.NewGuid();
        private Node _root;

        public int Count { get; private set; } = 0;

        public bool Find(TKey key, out TValue value)
        {
            Verify.IsNotNull(key, nameof(key));
            return TryFind(node => Find(node, key), out value);
        }

        public bool FindMin(out TValue minimum)
        {
            return TryFind(FindMin, out minimum);
        }

        public bool FindMax(out TValue maximum)
        {
            return TryFind(FindMax, out maximum);
        }

        private bool TryFind(Func<Node, Node> findFunc, out TValue value)
        {
            lock (_lock)
            {
                Node start = _root;
                Node node = findFunc(start);
                if (node != null)
                {
                    value = node.Value;
                    return true;
                }
                value = default(TValue);
                return false;
            }
        }

        private static Node Find(Node current, TKey key)
        {
            while (current != null)
            {
                int compare = key.CompareTo(current.Key);
                if (compare == 0)
                {
                    return current;
                }
                if (compare < 0)
                {
                    current = current.Left;
                }
                else
                {
                    current = current.Right;
                }
            }

            return null;
        }

        private static Node FindMin(Node current)
        {
            while (current?.Left != null)
            {
                current = current.Left;
            }

            return current;
        }

        private static Node FindMax(Node current)
        {
            while (current?.Right != null)
            {
                current = current.Right;
            }

            return current;
        }

        public void Insert(TKey key, TValue value)
        {
            Verify.IsNotNull(key, nameof(key));
            var nodeToInsert = new Node(key, value);
            lock (_lock)
            {
                Insert(nodeToInsert);
                ++Count;
                _version = Guid.NewGuid();
            }
        }

        private void Insert(Node node)
        {
            Node parent = null;
            Node current = _root;
            int compare;
            while (current != null)
            {
                parent = current;
                compare = node.Key.CompareTo(current.Key);
                if (compare < 0)
                {
                    current = current.Left;
                }
                else
                {
                    current = current.Right;
                }
            }

            node.Parent = parent;
            if (node.Parent == null)
            {
                _root = node;
                return;
            }

            compare = node.Key.CompareTo(node.Parent.Key);
            if (compare < 0)
            {
                node.Parent.Left = node;
            }
            else
            {
                node.Parent.Right = node;
            }
        }

        public bool Delete(TKey key)
        {
            Verify.IsNotNull(key, nameof(key));
            lock (_lock)
            {
                Node node = Find(_root, key);

                if (node == null)
                {
                    return false;
                }

                Delete(node);
                --Count;
                _version = Guid.NewGuid();
                return true;
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _root = null;
                Count = 0;
                _version = Guid.NewGuid();
            }
        }

        private void Delete(Node node)
        {
            if (node.Left == null)
            {
                Transplant(node, node.Right);
            }
            else if (node.Right == null)
            {
                Transplant(node, node.Left);
            }
            else
            {
                Node next = FindMin(node.Right);
                if(false == ReferenceEquals(next.Parent, node)){
                    Transplant(next, next.Right);
                    next.Right = node.Right;
                    next.Right.Parent = next;
                }
                Transplant(node, next);
                next.Left = node.Left;
                next.Left.Parent = next;
            }
        }

        private void Transplant(Node oldSubTree, Node newSubTree)
        {
            if (oldSubTree.Parent == null)
            {
                _root = newSubTree;
            }
            else if (ReferenceEquals(oldSubTree, oldSubTree.Parent.Left))
            {
                oldSubTree.Parent.Left = newSubTree;
            }
            else
            {
                oldSubTree.Parent.Right = newSubTree;
            }

            if (newSubTree != null)
            {
                newSubTree.Parent = oldSubTree.Parent;
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class Node
        {
            public Node(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }

            public TValue Value { get; }
            public TKey Key { get; }
            public Node Left { get; set; }
            public Node Right { get; set; }
            public Node Parent { get; set; }
        }

        private struct Enumerator : IEnumerator<TValue>
        {
            private readonly BinaryTree<TKey, TValue> _tree;
            private readonly Node _root;
            private readonly Guid _version;
            private readonly Queue<Node> _queue;
            private Node _current;

            public Enumerator(BinaryTree<TKey, TValue> tree)
            {
                _tree = tree;
                _version = tree._version;
                _root = tree._root;
                _current = null;
                _queue = new Queue<Node>();
                if(_root != null)
                {
                    _queue.Enqueue(_root);
                }
            }

            public TValue Current
            {
                get
                {
                    if (_version != _tree._version)
                    {
                        _current = null;
                        throw new InvalidOperationException(Resources.ErrBinaryTreeChanged);
                    }
                    return _current != null ? _current.Value : default(TValue);
                }
            }

            object IEnumerator.Current => _current;

            public void Dispose()
            {
                return;
            }

            public bool MoveNext()
            {
                if(_version != _tree._version)
                {
                    _current = null;
                    throw new InvalidOperationException(Resources.ErrBinaryTreeChanged);
                }
                if(_queue.Count > 0)
                {
                    _current = _queue.Dequeue();
                    if(_current.Left != null)
                    {
                        _queue.Enqueue(_current.Left);
                    }
                    if(_current.Right != null)
                    {
                        _queue.Enqueue(_current.Right);
                    }
                    return true;
                }
                else
                {
                    _current = null;
                    return false;
                }
            }

            public void Reset()
            {
                _current = null;
                _queue.Clear();
                if(_root != null)
                {
                    _queue.Enqueue(_root);
                }
            }
        }
    }
}