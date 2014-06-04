using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    class Operand : ExpressionNode
    {
        protected Dimension dimension;

        protected Operand() 
        {
            this.type = NodeType.Operand;
        }

        public Operand(String name, Dimension dimension, Expression expr = null) : base(name + dimension.ToString(), expr)
        {
            this.type = NodeType.Operand;
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
