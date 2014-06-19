using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    class Multiplication : ExpressionNode
    {
        public Multiplication()
            : base("*")
        {
            this.type = NodeType.Multiplication;
        }

        public override Dimension GetDimension()
        {
            Dimension resultDim = ((ExpressionNode)(this.children[0])).GetDimension();
            for (int i = 1; i < this.children.Count; i++)
            {
                resultDim = GetDimension(
                    resultDim, ((ExpressionNode)(this.children[i])).GetDimension());
            }
            return resultDim;
        }

        private Dimension GetDimension(Dimension leftDim, Dimension rightDim)
        {
            Dimension dim;

            // At least one operand is scalar
            if (leftDim.Is1x1 || rightDim.Is1x1)
            {
                dim = new Dimension(Math.Max(leftDim.M, rightDim.M),
                    Math.Max(leftDim.N, rightDim.N));
            }

            // No scalar operand
            else
            {
                if (leftDim.N != rightDim.M)
                {
                    throw new ApplicationException(
                        "Error: Invalid " + this.DisplayName + " dimensions!");
                }
                else
                {
                    dim = new Dimension(leftDim.M, rightDim.N);
                }
            }

            return dim;
        }

        public override int ComputationCost()
        {
            int cost = ((ExpressionNode)(this.GetChild(0))).ComputationCost();
            for (int i = 1; i < this.ChildrenCount(); i++)
            {
                cost += ComputationCost(((ExpressionNode)(this.GetChild(i - 1))).GetDimension(),
                    ((ExpressionNode)(this.GetChild(i))).GetDimension());
                cost += ((ExpressionNode)(this.GetChild(i))).ComputationCost();
            }
            return cost;
        }

        private int ComputationCost(Dimension left, Dimension right)
        {
            int cost = 0;

            // At least one operand is scalar
            if (left.Is1x1 || right.Is1x1)
            {
                cost = Math.Max(left.M, right.M) *
                    Math.Max(left.N, right.N);
            }

            // No scalar operand, mxk * kxn matrices
            // cost = 2*k*m*n
            else
            {
                cost = 2 * left.N * left.M * right.N;
            }

            return cost;
        }

        public override int TempStorageCost()
        {
            int cost = 0;
            for (int i = 0; i < this.ChildrenCount(); i++)
            {
                if (i >= 2)
                {
                    cost += TempStorageCost(((ExpressionNode)(this.GetChild(i - 1))).GetDimension(),
                                ((ExpressionNode)(this.GetChild(i))).GetDimension());
                }
                cost += ((ExpressionNode)(this.GetChild(i))).TempStorageCost();
            }

            Dimension dim = this.GetDimension();

            return cost + dim.M * dim.N;
        }

        private int TempStorageCost(Dimension left, Dimension right)
        {
            int cost = 0;

            // At least one operand is scalar
            if (left.Is1x1 || right.Is1x1)
            {
                cost = Math.Max(left.M, right.M) *
                    Math.Max(left.N, right.N);
            }

            // No scalar operand, mxk * kxn matrices
            // cost = m*n
            else
            {
                cost = left.M * right.N;
            }

            return cost;
        }
    }
}
