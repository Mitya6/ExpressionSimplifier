using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    internal class Multiplication : Operator
    {
        public override String ToString()
        {
            return "*";
        }

        public override Dimension GetDimensions()
        {
            Dimension leftDim = this.LeftChild.GetDimensions();
            Dimension rightDim = this.RightChild.GetDimensions();
            Dimension dim;

            // At least one operand is of type Scalar
            if (this.LeftChild.GetType() == typeof(Scalar) ||
                this.RightChild.GetType() == typeof(Scalar))
            {
                dim = new Dimension(Math.Max(leftDim.N, rightDim.N),
                    Math.Max(leftDim.M, rightDim.M));
            }

            // No Scalar operand
            else
            {
                if (leftDim.M != rightDim.N)
                {
                    throw new ApplicationException("Error: Invalid " + this.ToString() + " dimensions!");
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
