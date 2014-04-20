using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    internal class Multiplication : TreeNode
    {
        public Multiplication(Expression expr = null) : base("*", expr) { }

        public override Dimension GetDimension()
        {
            Dimension resultDim = this.children[0].GetDimension();
            for (int i = 1; i < this.children.Count; i++)
            {
                resultDim = GetDimension(
                    resultDim, this.children[i].GetDimension());
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

        public override int Cost()
        {
            int cost = this.children[0].Cost();
            for (int i = 1; i < this.children.Count; i++)
            {
                cost += Cost(this.children[i - 1].GetDimension(),
                    this.children[i].GetDimension());
                cost += this.children[i].Cost();
            }
            return cost;
        }

        private int Cost(Dimension leftDim, Dimension rightDim)
        {
            int cost = 0;

            // At least one operand is scalar
            if (leftDim.Is1x1 || rightDim.Is1x1)
            {
                cost = Math.Max(leftDim.M, rightDim.M) *
                    Math.Max(leftDim.N, rightDim.N);
            }

            // No scalar operand, mxk * kxn matrices
            // cost = 2*k*m*n
            else
            {
                cost = 2 * leftDim.N * leftDim.M * rightDim.N;
            }

            return cost;
        }
    }
}
