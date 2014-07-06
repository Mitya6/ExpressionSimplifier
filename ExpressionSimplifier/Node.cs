using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionSimplifier.TreeIteration;

namespace ExpressionSimplifier
{
    public abstract class Node
    {
        protected List<Node> children = new List<Node>();
        protected NodeType type;

        public NodeType Type { get { return this.type; } }
        public WeakReference Parent { get; set; }
        public bool IsLeaf { get { return this.children.Count == 0; } }
        public bool IsRoot { get { return this.Parent == null; } }


        #region Child manipulation

        public Node GetChild(int idx)
        {
            return this.children[idx];
        }

        public Node GetFirstChild(Func<Node, bool> predicate)
        {
            return this.children.FirstOrDefault(predicate);
        }

        public Node GetLastChild()
        {
            return this.children.Last();
        }

        public int ChildrenCount()
        {
            return this.children.Count;
        }

        public int IndexOf(Node node)
        {
            return this.children.IndexOf(node);
        }

        public void AddChild(Node child)
        {
            this.children.Add(child);
            child.Parent = new WeakReference(this);
        }

        public void InsertChild(Node child, int idx)
        {
            this.children.Insert(idx, child);
            child.Parent = new WeakReference(this);
        }

        public void RemoveChild(int idx)
        {
            this.children[idx].Parent = null;
            this.children.RemoveAt(idx);
        }

        public void RemoveChild(Node child)
        {
            if (this.children.Contains(child))
            {
                child.Parent = null;
                this.children.Remove(child);
            }
        }

        public void ReplaceChild(int idx, Node newNode)
        {
            InsertChild(newNode, idx);
            RemoveChild(idx + 1);
        }

        public void ReplaceChild(Node oldNode, Node newNode)
        {
            if (this.children.Contains(oldNode))
            {
                InsertChild(newNode, this.children.IndexOf(oldNode));
                RemoveChild(oldNode);
            }
        }

        public void ClearChildren()
        {
            this.children.Clear();
        }

        #endregion
        
        /// <summary>
        /// Returns the depth of the tree starting from this node.
        /// </summary>
        public int Depth()
        {
            int depth = 0;
            foreach (Node child in this.children)
            {
                if (child != null)
                {
                    depth = Math.Max(depth, child.Depth());
                }
            }
            return depth + 1;
        }

        public BFSIterator GetBFSIterator()
        {
            return new BFSIterator(this);
        }

    }
}
