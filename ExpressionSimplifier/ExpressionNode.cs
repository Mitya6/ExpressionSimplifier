using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    public abstract class ExpressionNode : Node
    {
        public String DisplayName { get; set; }

        public ExpressionNode() { }

        public ExpressionNode(String name)
        {
            this.DisplayName = name;
        }

        /// <summary>
        /// Calculates the resulting dimension of the node recursively.
        /// </summary>
        public abstract Dimension GetDimension();

        /// <summary>
        /// Calculates the computation cost of the node recursively.
        /// </summary>
        public abstract int Cost();

        public bool IsSameOperationAsParent()
        {
            return (this.Type == NodeType.Addition &&
                this.Parent.Type == NodeType.Addition) ||
                (this.Type == NodeType.Multiplication &&
                this.Parent.Type == NodeType.Multiplication);
        }

        public override String ToString()
        {
            return ToString(false);
        }

        private String ToString(bool hasMultiplicationAbove)
        {
            String s;
            if (this.IsLeaf)
            {
                s = this.DisplayName;
                if (this.DisplayName.StartsWith("-"))
                {
                    s = "(" + s + ")";
                }
                return s;
            }

            s = ((ExpressionNode)this.children[0]).ToString(this.Type == NodeType.Multiplication);
            for (int i = 1; i < this.children.Count; i++)
            {
                s += this.DisplayName;
                s += ((ExpressionNode)this.children[i]).ToString(this.Type == NodeType.Multiplication);
            }

            if (hasMultiplicationAbove && this.Type == NodeType.Addition)
            {
                s = "(" + s + ")";
            }

            return s;
        }


        #region Transformations

        /// <summary>
        /// Deletes the given node if it is zero or empty.
        /// </summary>
        /*public bool DeleteZeroAdditionLeaf()
        {
            // Return if zeroNode is the root node
            if (this.Parent == null)
            {
                return false;
            }

            if (this.Parent.Type == NodeType.Addition && this.Type == NodeType.Scalar
                && ((Scalar)this).Value == 0)
            {
                this.Parent.RemoveChild(this);
                return true;
            }

            return false;
        }*/

        /// <summary>
        /// Removes the given addition node if it has only one child and
        /// connects its child to its parent node
        /// </summary>
        /*public bool ContractOneChildAddition()
        {
            if (this.Type == NodeType.Addition && this.ChildrenCount() == 1)
            {
                if (this.IsRoot)
                {
                    this.Expr.Root = (ExpressionNode)(this.GetChild(0));
                }
                else
                {
                    this.Parent.AddChild(this.GetChild(0));
                    this.Parent.RemoveChild(this);
                }

                this.ClearChildren();
                return true;
            }

            return false;
        }*/

        /// <summary>
        /// Raises an operation node to its parent node by unifying them if they
        /// are of the same type of operation. Preserves the order of child nodes.
        /// </summary>
        public ExpressionNode Raise()
        {
            if (this.Parent != null && this.IsSameOperationAsParent())
            {
                int idx = Parent.IndexOf(this);
                for (int i = this.ChildrenCount() - 1; i >= 0; i--)
                {
                    Node current = this.GetChild(i);
                    this.RemoveChild(current);
                    Parent.InsertChild(current, idx);
                }
                Parent.RemoveChild(this);
            }

            return null;
        }

        /// <summary>
        /// Rotates the tree around the given child node and its parent node to the left.
        /// </summary>
        /*public bool RotateLeft()
        {
            return Rotate(true);
        }*/

        /// <summary>
        /// Rotates the tree around the given child node and its parent node to the right.
        /// </summary>
        /*public bool RotateRight()
        {
            return Rotate(false);
        }*/

        /*private bool Rotate(bool left)
        {
            ExpressionNode parent = (ExpressionNode)(this.Parent);

            if (parent != null)
            {
                if (this.IsSameOperationAsParent())
                {
                    parent.RemoveChild(this);

                    // If parent is not the root node
                    if (parent.Parent != null)
                    {
                        int parentIdx = parent.Parent.IndexOf(parent);

                        parent.Parent.InsertChild(this, parentIdx);

                        parent.Parent.RemoveChild(parent);
                    }
                    // parent is the root node
                    else
                    {
                        this.Expr.Root = this;
                    }

                    if (left)
                    {
                        this.InsertChild(parent, 0);
                    }
                    else
                    {
                        this.AddChild(parent);
                    }
                    return true;
                }
            }

            return false;
        }*/

        /// <summary>
        /// Performs the operation stored in the given node if all its children are scalars.
        /// </summary>
        public ExpressionNode PerformScalarOperation()
        {
            if (this.Type != NodeType.Addition && this.Type != NodeType.Multiplication)
                return null;

            // Check whether all children are of type scalar with values
            bool allChildrenScalar = true;
            for (int i = 0; i < this.ChildrenCount(); i++)
            {
                if (!(this.GetChild(i) is Scalar) || ((Scalar)(this.GetChild(i))).Value == null)
                {
                    allChildrenScalar = false;
                    break;
                }
            }

            if (allChildrenScalar)
            {
                double result = (double)((Scalar)(this.GetChild(0))).Value;
                for (int i = 1; i < this.ChildrenCount(); i++)
                {
                    if (this.Type == NodeType.Addition)
                    {
                        result += (double)((Scalar)(this.GetChild(i))).Value;
                    }
                    else
                    {
                        result *= (double)((Scalar)(this.GetChild(i))).Value;
                    }
                }

                ExpressionNode newNode = new Scalar(result.ToString());
                if (!this.IsRoot)
                {
                    this.Parent.ReplaceChild(this, newNode);
                }
                else
                {
                    return newNode;
                }
            }

            return null;
        }

        public ExpressionNode OrderChildren()
        {
            if (this.Type != NodeType.Multiplication)
            {
                var scalarsWithValue = this.children
                    .Where(child => child.Type == NodeType.Scalar && ((Scalar)child).Value != null)
                    .OrderBy(child => ((Scalar)child).Value).ToList();
                var scalarsWithoutValue = this.children
                    .Where(child => child.Type == NodeType.Scalar && ((Scalar)child).Value == null)
                    .OrderBy(child => ((Scalar)child).Value).ToList();
                var otherOperands = this.children
                    .Where(child => child.Type == NodeType.Operand)
                    .OrderBy(child => ((ExpressionNode)child).DisplayName).ToList();
                var additionNodes = this.children
                    .Where(child => child.Type == NodeType.Addition).ToList();
                var multiplicationNodes = this.children
                    .Where(child => child.Type == NodeType.Multiplication).ToList();

                this.ClearChildren();
                this.children.AddRange(scalarsWithValue);
                this.children.AddRange(scalarsWithoutValue);
                this.children.AddRange(otherOperands);
                this.children.AddRange(additionNodes);
                this.children.AddRange(multiplicationNodes);
            }
            return null;
        }

        #endregion
    }
}
