using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    internal class Addition : Operator
    {
        public override String ToString()
        {
            return "+";
        }

        public override Dimension GetDimensions()
        {
            Dimension leftDim = this.LeftChild.GetDimensions();
            Dimension rightDim = this.RightChild.GetDimensions();

            if (leftDim.N != rightDim.N || leftDim.M != rightDim.M)
            {
                throw new ApplicationException("Error: Invalid " + this.ToString() + " dimensions!");
            }
            return leftDim;
        }
    }
}
