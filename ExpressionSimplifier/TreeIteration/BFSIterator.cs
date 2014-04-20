using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier.TreeIteration
{
    public class BFSIterator : TreeIterator
    {
        private Queue<TreeNode> queue;

        public BFSIterator(TreeNode root)
        {
            this.queue = new Queue<TreeNode>();
            this.queue.Enqueue(root);
        }

        public TreeNode Next()
        {
            TreeNode currentNode = this.queue.Dequeue();
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
