using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    internal class Multiplication : TreeNode
    {
        public Multiplication()
        {
            this.Name = "*";
        }

        public override Dimension GetDimension()
        {
            Dimension leftDim = this.LeftChild.GetDimension();
            Dimension rightDim = this.RightChild.GetDimension();
            Dimension dim;

            // At least one operand is of type Scalar
            if (leftDim.Is1x1 || rightDim.Is1x1)
            {
                dim = new Dimension(Math.Max(leftDim.N, rightDim.N),
                    Math.Max(leftDim.M, rightDim.M));
            }

            // No Scalar operand
            else
            {
                if (leftDim.M != rightDim.N)
                {
                    throw new ApplicationException("Error: Invalid " + this.Name + " dimensions!");
                }
                else
                {
                    dim = new Dimension(leftDim.N, rightDim.M);
                }
            }

            return dim;
        }
    }
}
