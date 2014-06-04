using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    class Addition : ExpressionNode
    {
        public Addition(Expression expr = null) : base("+", expr) 
        {
            this.type = NodeType.Addition;
        }

        public override Dimension GetDimension()
        {
            ExpressionNode child0 = (ExpressionNode)(this.children[0]);

            foreach (ExpressionNode child in this.children)
            {
                if (child.GetDimension().M != child0.GetDimension().M ||
                    child.GetDimension().N != child0.GetDimension().N)
                {
                    throw new ApplicationException(
                    "Error: Invalid " + this.DisplayName + " dimensions!");
                }
            }

            return child0.GetDimension();
        }

        public override int Cost()
        {
            Dimension dim = ((ExpressionNode)(this.children[0])).GetDimension();

            int cost = 0;
            for (int i = 0; i < this.children.Count; i++)
            {
                cost += ((ExpressionNode)(this.children[i])).Cost();
                if (i != 0)
                {
                    cost += dim.M * dim.N; 
                }
            }

            return cost;
        }
    }
}
