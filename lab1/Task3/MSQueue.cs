namespace lab1.Task3
{
    public class MsQueue<T> : INonBlockingQueue<T>
    {
        private volatile Node<T> _head;
        private volatile Node<T> _tail;

        private class Node<T> {
            private T _value;
            private AtomicReference<Node<T>> _next;

            public Node(T value) {
                _value = value;
                _next = new AtomicReference<Node<T>>(null);
            }

            public T GetValue() {
                return _value;
            }

            public void SetValue(T value) {
                _value = value;
            }

            public Node<T> GetNext() {
                return _next.Value;
            }

            public bool CompareAndSetNext(Node<T> expected, Node<T> next) {
                return _next.CompareAndExchange(expected, next);
            }
        }

        public MsQueue() {
            var dummyNode = new Node<T>(default);
            _head = dummyNode;
            _tail = dummyNode;
        }
        
        public void Add(T elem)
        {
            var node = new Node<T>(elem);

            while (true) {
                if (_tail.CompareAndSetNext(null, node)) {
                    _tail = node;
                    break;
                } 
                else {
                    var nextNode = _tail.GetNext();

                    if (nextNode != null) {
                        _tail = nextNode;
                    }
                }
            }
        }

        public T Remove()
        {
            Node<T> node;

            while (true) {
                node = _head.GetNext();

                if (node == null) {
                    return default;
                }

                if (_head == _tail) {
                    _tail = node;
                } else if (_head.CompareAndSetNext(node, node.GetNext())) {
                    return node.GetValue();
                }
            }
        }
    }
}