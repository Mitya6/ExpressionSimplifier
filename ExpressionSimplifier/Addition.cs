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
            if (cache.DimensionValid)
            {
                return cache.Dim;
            }

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

            Dimension dim = child0.GetDimension();
            cache.Dim = dim;
            return dim;
        }

        public override int ComputationCost()
        {
            if (cache.ComputationCostValid)
            {
                return cache.ComputationCost;
            }

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

            cache.ComputationCost = cost;
            return cost;
        }

        public override int TempStorageCost()
        {
            if (cache.StorageCostValid)
            {
                return cache.StorageCost;
            }

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

            int value = cost + dim.M * dim.N;
            cache.StorageCost = value;
            return value;
        }
    }
}
