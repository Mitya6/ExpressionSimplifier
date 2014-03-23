using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    internal abstract class Operand : TreeNode
    {
        protected Dimension dimensions;

        public String Name { get; set; }

        public override String ToString()
        {
            return this.Name;
        }

        public override Dimension GetDimensions()
        {
            return this.dimensions;
        }
    }
}
