using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    class Addition : ExpressionNode
    {
        public Addition()
            : base("+")
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

        public override int ComputationCost()
        {
            Dimension dim = ((ExpressionNode)(this.children[0])).GetDimension();

            int cost = 0;
            for (int i = 0; i < this.ChildrenCount(); i++)
            {
                cost += ((ExpressionNode)(this.GetChild(i))).ComputationCost();
                if (i != 0)
                {
                    cost += dim.M * dim.N;
                }
            }

            return cost;
        }

        public override int TempStorageCost()
        {
            Dimension dim = this.GetDimension();

            int cost = 0;
            for (int i = 0; i < this.ChildrenCount(); i++)
            {
                cost += ((ExpressionNode)(this.GetChild(i))).TempStorageCost();
                if (i >= 2)
                {
                    cost += dim.M * dim.N;
                }
            }

            return cost + dim.M * dim.N;
        }
    }
}
