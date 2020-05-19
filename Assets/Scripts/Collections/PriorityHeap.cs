public class PriorityHeap<T> where T : class
{
    private class Node
    {
        private Node _leftChild, _rightChild;

        public T Data;
        public int Priority;

        public Node Parent;

        public Node LeftChild 
        {
            get => _leftChild;
            set
            {
                _leftChild = value;
                _leftChild.Parent = this;
            }
        }

        public Node RightChild
        {
            get => _rightChild;
            set
            {
                _rightChild = value;
                _rightChild.Parent = this;
            }
        }

        public Node(T data, int priority)
        {
            Data = data;
            Priority = priority;
        }
    }

    private Node _root;
    private Node _last;

    private int _count;

    public void Enqueue(T data, int priority)
    {
        Node node = new Node(data, priority);

        if (_root == null)
        {
            _root = node;
            _last = node;
            _count = 1;
            return;
        }

        _count++;

        if(_last.LeftChild != null)
        {
            _last.LeftChild = node;
        }
        else
        {
            _last.RightChild = node;
        }

        //go until we reach the root, in the worst case
        while(node.Parent != null)
        {
            if (node.Priority <= node.Parent.Priority)
                break;

            Swap(node, node.Parent);

            node = _last;
            _last = node.Parent;
        }
    }

    public T Dequeue()
    {
        return null;
    }

    private void Swap(Node n1, Node n2)
    {
        T tempData = n1.Data;
        int tempPriority = n1.Priority;

        n1.Data = n2.Data;
        n1.Priority = n2.Priority;

        n2.Data = tempData;
        n2.Priority = tempPriority;
    }
}
