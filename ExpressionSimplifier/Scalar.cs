using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    internal class Scalar : Operand
    {
        public Scalar(String name)
        {
            this.Name = name;
            this.dimensions = new Dimension(1, 1);
        }
    }
}
