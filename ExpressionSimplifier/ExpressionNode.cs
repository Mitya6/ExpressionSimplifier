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

        protected Cache cache;

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
        public abstract int ComputationCost();

        /// <summary>
        /// Returns the count of temporarily stored floating point numbers
        /// needed to calculate the node.
        /// </summary>
        public abstract int TempStorageCost();

        public override String ToString()
        {
            if (cache.StringValid)
            {
                return cache.StringValue;
            }

            String value = ToString(false);
            cache.StringValue = value;
            return value;
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

        public bool IsSameOperationAsParent()
        {
            return (this.Type == NodeType.Addition &&
                this.Parent.Type == NodeType.Addition) ||
                (this.Type == NodeType.Multiplication &&
                this.Parent.Type == NodeType.Multiplication);
        }

        public void Invalidate()
        {
            cache.Invaildate();
            if (this.Parent != null)
            {
                ((ExpressionNode)this.Parent).Invalidate();
            }
        }


        #region Transformations       

        /// <summary>
        /// Raises an operation node to its parent node by unifying them if they
        /// are of the same type of operation. Preserves the order of child nodes.
        /// </summary>
        public ExpressionNode Raise()
        {
            if (this.Parent != null && this.IsSameOperationAsParent())
            {
                Invalidate();

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
                Invalidate();

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
                Invalidate();

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
