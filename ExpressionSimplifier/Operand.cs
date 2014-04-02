using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    internal class Operand : TreeNode
    {
        private Dimension dimension;

        public Operand(String name, Dimension dimension)
        {
            this.Name = name;
            this.dimension = dimension;
        }

        public override Dimension GetDimension()
        {
            return this.dimension;
        }
    }
}
