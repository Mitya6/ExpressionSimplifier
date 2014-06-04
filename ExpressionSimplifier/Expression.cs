using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionSimplifier.TreeIteration;

namespace ExpressionSimplifier
{
    public class Expression
    {
        private ExpressionNode root;
        public ExpressionNode Root
        {
            get { return root; }
            set
            {
                root = value;
                root.Parent = null;
            }
        }

        public String DisplayName { get; set; }

        public Expression(String displayName, ExpressionNode root)
        {
            this.DisplayName = displayName;
            if (root == null) return;

            this.Root = root;

            // Set containing expression reference in all children
            if (root != null)
            {
                BFSIterator iter = root.GetBFSIterator();
                while (iter.HasNext())
                {
                    ((ExpressionNode)(iter.Next())).Expr = this;
                }
            }
        }

        public override String ToString()
        {
            return this.DisplayName;
        }


        #region Transformations

        /// <summary>
        /// Deletes the given node if it is zero or empty.
        /// </summary>
        public bool DeleteZeroAddition(ExpressionNode zeroNode)
        {
            // Return if zeroNode is the root node
            if (zeroNode.Parent == null)
            {
                return false;
            }

            if (zeroNode.Parent.Type == NodeType.Addition &&
                (zeroNode.DisplayName == "0" || zeroNode.DisplayName == null))
            {
                zeroNode.Parent.RemoveChild(zeroNode);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Removes the given addition node if it has only one child and
        /// connects its child to its parent node
        /// </summary>
        public bool ContractOneChildAddition(ExpressionNode node)
        {
            if (node.Type == NodeType.Addition &&
                node.ChildrenCount() == 1)
            {
                if (node.IsRoot)
                {
                    node.Expr.Root = (ExpressionNode)(node.GetChild(0));
                }
                else
                {
                    node.Parent.AddChild(node.GetChild(0));
                    node.Parent.RemoveChild(node);
                }

                node.ClearChildren();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Raises an operation node to its parent node by unifying them if they
        /// are of the same type of operation. Preserves the order of child nodes.
        /// </summary>
        public bool Raise(ExpressionNode child)
        {
            ExpressionNode parent = (ExpressionNode)(child.Parent);

            if (parent != null)
            {
                if (child.IsSameOperationAsParent())
                {
                    int idx = parent.IndexOf(child);
                    parent.RemoveChild(child);

                    for (int i = child.ChildrenCount() - 1; i >= 0; i--)
                    {
                        ExpressionNode current = (ExpressionNode)(child.GetChild(i));
                        child.RemoveChild(current);
                        parent.InsertChild(current, idx);
                    }
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Rotates the tree around the given child node and its parent node to the left.
        /// </summary>
        public bool RotateLeft(ExpressionNode child)
        {
            return Rotate(child, true);
        }

        /// <summary>
        /// Rotates the tree around the given child node and its parent node to the right.
        /// </summary>
        public bool RotateRight(ExpressionNode child)
        {
            return Rotate(child, false);
        }

        private bool Rotate(ExpressionNode child, bool left)
        {
            ExpressionNode parent = (ExpressionNode)(child.Parent);

            if (parent != null)
            {
                if (child.IsSameOperationAsParent())
                {
                    parent.RemoveChild(child);

                    // If parent is not the root node
                    if (parent.Parent != null)
                    {
                        int parentIdx = parent.Parent.IndexOf(parent);

                        parent.Parent.InsertChild(child, parentIdx);

                        parent.Parent.RemoveChild(parent);
                    }
                    // parent is the root node
                    else
                    {
                        this.Root = child;
                    }

                    if (left)
                    {
                        child.InsertChild(parent, 0);
                    }
                    else
                    {
                        child.AddChild(parent);
                    }
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Performs the operation stored in the given node if all its children are scalars.
        /// </summary>
        public bool PerformScalarOperation(ExpressionNode node)
        {
            if (node.Type != NodeType.Addition && node.Type != NodeType.Multiplication)
            {
                return false;
            }

            // Check whether all children are of type scalar with values
            bool allChildrenScalar = true;
            for (int i = 0; i < node.ChildrenCount(); i++)
            {
                if (!(node.GetChild(i) is Scalar))
                {
                    allChildrenScalar = false;
                    break;
                }

                if (((Scalar)(node.GetChild(i))).Value == null)
                {
                    allChildrenScalar = false;
                    break;
                }
            }

            if (allChildrenScalar)
            {
                double result = (double)((Scalar)(node.GetChild(0))).Value;
                for (int i = 1; i < node.ChildrenCount(); i++)
                {
                    if (node.Type == NodeType.Addition)
                    {
                        result += (double)((Scalar)(node.GetChild(i))).Value;
                    }
                    else
                    {
                        result *= (double)((Scalar)(node.GetChild(i))).Value;
                    }
                }

                ExpressionNode newNode = new Scalar(result.ToString(), this);
                if (node.Parent != null)
                {
                    node.Parent.ReplaceChild(node, newNode);
                }
                else
                {
                    this.Root = newNode;
                }
                return true;
            }

            return false;
        }

        #endregion
    }
}
