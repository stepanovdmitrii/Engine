using System.Threading;

namespace Engine.Core.Concurrent
{
    public sealed class ConcurrentStack<T> where T: class
    {
        private volatile Node _head;

        public void Push(T value)
        {
            Verify.IsNotNull(value, nameof(value));

            var node = new Node(value);
            Node head;
            do
            {
                head = _head;
                node._next = head;
            } while (Interlocked.CompareExchange(ref _head, node, head) != head);
        }

        public T Pop()
        {
            Node next;
            Node head;
            do
            {
                head = _head;
                if(_head == null)
                {
                    return null;
                }
                next = head._next;

            } while (Interlocked.CompareExchange(ref _head, next, head) != head);
            return head._value;
        }

        private sealed class Node
        {
            internal readonly T _value;
            internal Node _next;

            public Node(T value)
            {
                _value = value;
            }
        }
    }
}
