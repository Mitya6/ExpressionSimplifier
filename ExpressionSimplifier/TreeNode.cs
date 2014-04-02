using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    internal abstract class TreeNode
    {
        public TreeNode LeftChild { get; set; }
        public TreeNode RightChild { get; set; }
        public String Name { get; set; }

        /// <summary>
        /// Calculates the resulting dimension of the node recursively.
        /// </summary>
        public abstract Dimension GetDimension();

        /// <summary>
        /// Returns the depth of the tree starting from this node.
        /// </summary>
        public int Depth()
        {
            int depthLeft = 0;
            int depthRight = 0;

            if (this.LeftChild != null)
            {
                depthLeft = this.LeftChild.Depth();
            }
            if (this.RightChild != null)
            {
                depthRight = this.RightChild.Depth();
            }

            return Math.Max(depthLeft, depthRight) + 1;
        }

        /// <summary>
        /// Returns a 90 degree rotated string representation of the current
        /// expression tree.
        /// </summary>
        public override String ToString()
        {
            return ToString(0);
        }

        private String ToString(int depth)
        {
            String s = "";
            if (this.RightChild != null)
            {
                s += this.RightChild.ToString(depth + 1);
            }

            for (int i = 0; i < depth; i++)
            {
                s += "\t";
            }
            s += this.Name + "\n";

            if (this.LeftChild != null)
            {
                s += this.LeftChild.ToString(depth + 1);
            }

            return s;
        }
    }
}
