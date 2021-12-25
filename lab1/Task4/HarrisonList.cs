namespace lab1.Task4
{
    public class HarrisonList<T>
    {
        private volatile AtomicReference<Node<T>> _head;
        private volatile int _size;

        public HarrisonList()
        {
            _head = new AtomicReference<Node<T>>(null);
            _size = 0;
        }
        
        public void Add(T elem) {
            Add(-1, elem);
        }
        
        public void Add(int index, T elem) {
            var node = new Node<T>(elem);
            Node<T> prevNode;
            Node<T> nextNode;

            int nodeIndex = index == -1 ? _size : 1;

            while (true) {
                if (_head.Value == null) {
                    if (_head.CompareAndExchange(null, node)) {
                        _size++;
                        break;
                    }
                }
                else if (nodeIndex == 0) {
                    prevNode = _head.Value;
                    node.SetNext(prevNode);

                    if (_head.CompareAndExchange(prevNode, node)) {
                        _size++;
                        break;
                    }
                }
                else if (_head.Value != null) {
                    prevNode = GetNode(nodeIndex - 1);

                    if (prevNode == null) {
                        continue;
                    }

                    nextNode = prevNode.GetNext();
                    node.SetNext(nextNode);

                    if (prevNode.CompareAndSetNext(nextNode, node)) {
                        _size++;
                        break;
                    }
                }
            }
        }
        
        public bool Remove(int index) {
            Node<T> node;
            Node<T> prevNode;

            while (true) {
                node = GetNode(index);
                node.SetToBeDeleted(true);
                if (index == 0) {
                    if (_head.CompareAndExchange(node, node.GetNext())) {
                        _size--;
                        return true;
                    }
                }
                else {
                    prevNode = GetNode(index - 1);

                    if (prevNode.CompareAndSetNext(node, node.GetNext())) {
                        _size--;
                        return true;
                    }
                }

                node.SetToBeDeleted(false);
                return false;
            }
        }
        
        public T Get(int index) {
            return GetNode(index).GetValue();
        }
        
        private Node<T> GetNode(int index) {
            var currNode = _head.Value;
            int currIndex = 0;

            while (
                currIndex < index &&
                currNode != null
            ) {
                currNode = currNode.GetNext();
                currIndex++;
            }

            return currIndex == index ? currNode : null;
        }
        
        private class Node<T> {
            private T _value;
            private AtomicReference<Node<T>> _next;
            private bool _toBeDeleted;

            public Node(T value) {
                _value = value;
                _next = new AtomicReference<Node<T>>(null);
                _toBeDeleted = false;
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

            public void SetNext(Node<T> next) {
                _next.Set(next);
            }

            public bool IsToBeDeleted() {
                return _toBeDeleted;
            }

            public void SetToBeDeleted(bool toBeDeleted) {
                _toBeDeleted = toBeDeleted;
            }

            public bool CompareAndSetNext(Node<T> expected, Node<T> next) {
                return !_toBeDeleted && _next.CompareAndExchange(expected, next);
            }
        }
    }
}