using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    internal class Division : Operator
    {
        public override String ToString()
        {
            return "/";
        }

        public override Dimension GetDimensions()
        {
            Dimension leftDim = this.LeftChild.GetDimensions();
            Dimension rightDim = this.RightChild.GetDimensions();

            // The right operand has to be of type Scalar
            if (this.RightChild.GetType() != typeof(Scalar) && 
                (rightDim.N != 1 || rightDim.M != 1))
            {
                throw new ApplicationException("Error: Invalid " + this.ToString() + " right operand!");
            }

            return leftDim;
        }
    }
}
