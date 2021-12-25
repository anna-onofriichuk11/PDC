using System;

namespace lab1.Task2
{
    public class SkipList<T> : INonBlockingList<T>
    {
        private int _maxHeight;
        private Node<T> _headTop;

        private static int _defaultMaxHeight = 32;
        
        public SkipList() {
            _maxHeight = _defaultMaxHeight;
            Node<T> currNode = new Node<T>(default, _defaultMaxHeight);
            _headTop = currNode;

            for (int i = 1; i < _maxHeight; i++) {
                Node<T> nextNode = new Node<T>(default, _maxHeight);
                currNode.CompareAndSetLowerNode(null, nextNode);

                currNode = nextNode;
            }
        }

        public void Add(T elem)
        {
            var height = GetNodeHeight();

            var currNode = GetHeadNodeByIndex(height - 1);
            Node<T> prevAddedNode = null;

            while (currNode != null)
            {
                var rightNode = currNode.GetRightNode();

                if (
                    rightNode != null &&
                    rightNode.GetRightNode().ToString().CompareTo(elem) == -1
                )
                {
                    currNode = rightNode;
                }
                else
                {
                    var node = new Node<T>(elem, height);

                    node.SetRightNode(rightNode);

                    if (currNode.CompareAndSetRightNode(rightNode, node))
                    {
                        currNode = currNode.GetLowerNode();

                        if (prevAddedNode != null)
                        {
                            prevAddedNode.SetLowerNode(node);
                        }

                        prevAddedNode = node;
                    }
                }
            }
        }

        public bool Remove(T elem)
        {
            var currNode = GetHeadBottomNode();

            while (currNode != null)
            {
                var rightNode = currNode.GetRightNode();
                if (
                    rightNode != null &&
                    rightNode.GetValue().ToString().CompareTo(elem) == -1
                )
                {
                    currNode = rightNode;
                }
                else if (
                    rightNode != null &&
                    rightNode.GetValue().ToString().CompareTo(elem) == 0
                )
                {
                    rightNode.SetToBeDeleted(true);

                    if (currNode.CompareAndSetRightNode(rightNode, rightNode.GetRightNode()))
                    {
                        currNode = currNode.GetLowerNode();
                    }
                    else
                    {
                        rightNode.SetToBeDeleted(false);
                       
                    }
                    return true;
                }
                else
                {
                    currNode = currNode.GetLowerNode();
                }
            }
            return false;
        }

        public bool Contains(T elem) {
            var currNode = GetHeadBottomNode().GetRightNode();

        while (currNode != null && !currNode.GetValue().Equals(elem)) {
            var rightNode = currNode.GetRightNode();
            if (
                rightNode != null &&
                rightNode.GetValue().ToString().CompareTo(elem) != 1
            ) {
                currNode = rightNode;
            } else {
                currNode = currNode.GetLowerNode();
            }
        }

        return currNode != null;
    }
    private Node<T> GetHeadBottomNode()
    {
            return GetHeadNodeByIndex(0);
    }

    private Node<T> GetHeadNodeByIndex(int index)
    { 
        var currNode = _headTop;
        
        for (int i = _maxHeight - 1; i > index; i--) { 
            currNode = currNode.GetLowerNode();
        }
        
        return currNode; 
    }

    private int GetNodeHeight() { 
        Random random = new Random(); 
        int level = 1;
        
        while (level < _maxHeight && Math.Abs(random.Next()) % 2 == 0) {
                level++; 
        }
        
        return level;
    }
    }
    
    public class Node<T> {
        private T _node;
        private AtomicReference<Node<T>> _rightNode;
        private AtomicReference<Node<T>> _lowerNode;
        private int _height;
        private bool _toBeDeleted;

        public Node(T node, int height) {
            _node = node;
            _rightNode = new AtomicReference<Node<T>>(null);
            _lowerNode = new AtomicReference<Node<T>>(null);
            _height = height;
            _toBeDeleted = false;
        }

        public T GetValue() {
            return _node;
        }

        public void SetValue(T value) {
            _node = value;
        }

        public Node<T> GetRightNode() {
            return _rightNode.Value;
        }

        public void SetRightNode(Node<T> rightNode) {
            _rightNode.Set(rightNode);
        }

        public Node<T> GetLowerNode() {
            return _lowerNode.Value;
        }

        public void SetLowerNode(Node<T> lowerNode) {
            _lowerNode.Set(lowerNode);
        }

        public int GetHeight() {
            return _height;
        }

        public void SetHeight(int height) {
            _height = height;
        }

        public bool IsToBeDeleted() {
            return _toBeDeleted;
        }

        public void SetToBeDeleted(bool toBeDeleted) {
            _toBeDeleted = toBeDeleted;
        }

        public bool CompareAndSetRightNode(Node<T> expected, Node<T> next) {
            return !_toBeDeleted && _rightNode.CompareAndExchange(expected, next);
        }

        public bool CompareAndSetLowerNode(Node<T> expected, Node<T> next) {
            return _lowerNode.CompareAndExchange(expected, next);
        }
    }
}