using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    public abstract class ExpressionNode : Node
    {
        public Expression Expr { get; set; }
        public String DisplayName { get; set; }

        public ExpressionNode() { }

        public ExpressionNode(String name, Expression expr = null)
        {
            if (expr != null)
            {
                this.Expr = expr;
            }

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
            return (this.GetType() == typeof(Addition) &&
                this.Parent.GetType() == typeof(Addition)) ||
                (this.GetType() == typeof(Multiplication) &&
                this.Parent.GetType() == typeof(Multiplication));
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

            s = ((ExpressionNode)this.children[0]).ToString(this.GetType() == typeof(Multiplication));
            for (int i = 1; i < this.children.Count; i++)
            {
                s += this.DisplayName;
                s += ((ExpressionNode)this.children[i]).ToString(this.GetType() == typeof(Multiplication));
            }

            if (hasMultiplicationAbove && this.GetType() == typeof(Addition))
            {
                s = "(" + s + ")";
            }

            return s;
        }
    }
}
