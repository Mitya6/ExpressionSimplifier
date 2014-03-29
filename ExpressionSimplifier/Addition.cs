using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    internal class Addition : TreeNode
    {
        public Addition()
        {
            this.Name = "+";
        }

        public override Dimension GetDimension()
        {
            Dimension leftDim = this.LeftChild.GetDimension();
            Dimension rightDim = this.RightChild.GetDimension();

            if (leftDim.N != rightDim.N || leftDim.M != rightDim.M)
            {
                throw new ApplicationException("Error: Invalid " + this.Name + " dimensions!");
            }
            return leftDim;
        }
    }
}
