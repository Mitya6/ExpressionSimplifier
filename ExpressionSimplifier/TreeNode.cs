using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionSimplifier.TreeIteration;

namespace ExpressionSimplifier
{
    public abstract class TreeNode
    {
        protected List<TreeNode> children;

        public TreeNode Parent { get; set; }
        public Expression Expr { get; set; }
        public String DisplayName { get; set; }
        public bool IsLeaf { get { return this.children.Count == 0; } }
        public bool IsRoot { get { return this.Parent == null; } }

        public TreeNode()
        {
            this.children = new List<TreeNode>();
        }

        public TreeNode(String name, Expression expr = null)
        {
            if (expr != null)
            {
                this.Expr = expr;
            }

            this.DisplayName = name;
            this.children = new List<TreeNode>();
        }

        /// <summary>
        /// Calculates the resulting dimension of the node recursively.
        /// </summary>
        public abstract Dimension GetDimension();

        /// <summary>
        /// Calculates the computation cost of the node recursively.
        /// </summary>
        public abstract int Cost();


        #region Child manipulation

        public TreeNode GetChild(int idx)
        {
            return this.children[idx];
        }

        public int ChildrenCount()
        {
            return this.children.Count;
        }

        public int IndexOf(TreeNode node)
        {
            return this.children.IndexOf(node);
        }

        public void AddChild(TreeNode child)
        {
            this.children.Add(child);
            child.Parent = this;
        }

        public void InsertChild(TreeNode child, int idx)
        {
            this.children.Insert(idx, child);
            child.Parent = this;
        }

        public void RemoveChild(int idx)
        {
            this.children[idx].Parent = null;
            this.children.RemoveAt(idx);
        }

        public void RemoveChild(TreeNode child)
        {
            if (this.children.Contains(child))
            {
                child.Parent = null;
                this.children.Remove(child);
            }
        }

        public void ReplaceChild(int idx, TreeNode newNode)
        {
            InsertChild(newNode, idx);
            RemoveChild(idx + 1);
        }

        public void ReplaceChild(TreeNode oldNode, TreeNode newNode)
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
            foreach (TreeNode child in this.children)
            {
                if (child != null)
                {
                    depth = Math.Max(depth, child.Depth());
                }
            }
            return depth + 1;
        }

        public bool IsSameOperationAsParent()
        {
            return (this.GetType() == typeof(Addition) &&
                this.Parent.GetType() == typeof(Addition)) ||
                (this.GetType() == typeof(Multiplication) &&
                this.Parent.GetType() == typeof(Multiplication));
        }

        public BFSIterator GetBFSIterator()
        {
            return new BFSIterator(this);
        }

        public override String ToString()
        {
            return this.DisplayName;
        }
    }
}
