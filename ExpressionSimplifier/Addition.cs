using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    internal class Addition : TreeNode
    {
        public Addition(Expression expr = null) : base("+", expr) { }

        public override Dimension GetDimension()
        {
            foreach (TreeNode child in this.children)
            {
                if (child.GetDimension().M != this.children[0].GetDimension().M ||
                    child.GetDimension().N != this.children[0].GetDimension().N)
                {
                    throw new ApplicationException(
                    "Error: Invalid " + this.DisplayName + " dimensions!");
                }
            }

            return this.children[0].GetDimension();
        }

        public override int Cost()
        {
            Dimension dim = this.children[0].GetDimension();

            int cost = this.children[0].Cost();
            for (int i = 1; i < this.children.Count; i++)
            {
                cost += this.children[i].Cost();
                cost += dim.M * dim.N;
            }

            return cost;
        }
    }
}
