using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier.TreeIteration
{
    public class BFSIterator : TreeIterator
    {
        private Queue<Node> queue;

        public BFSIterator(Node root)
        {
            this.queue = new Queue<Node>();
            this.queue.Enqueue(root);
        }

        public Node Next()
        {
            Node currentNode = this.queue.Dequeue();
            if (currentNode != null)
            {
                for (int i = 0; i < currentNode.ChildrenCount(); i++)
                {
                    this.queue.Enqueue(currentNode.GetChild(i));
                }
            }
            return currentNode;
        }

        public bool HasNext()
        {
            return this.queue.Count > 0;
        }
    }
}
