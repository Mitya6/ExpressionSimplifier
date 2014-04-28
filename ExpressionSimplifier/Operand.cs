using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    internal class Operand : ExpressionNode
    {
        protected Dimension dimension;

        protected Operand() { }

        public Operand(String name, Dimension dimension, Expression expr = null) : base(name + dimension.ToString(), expr)
        {
            this.dimension = dimension;
        }

        public override Dimension GetDimension()
        {
            return this.dimension;
        }

        public override int Cost()
        {
            return 0;
        }
    }
}
