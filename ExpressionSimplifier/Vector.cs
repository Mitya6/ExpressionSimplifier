using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    internal class Vector : Operand
    {
        public Vector(String name, int n)
        {
            this.Name = name;
            this.dimensions = new Dimension(n, 1);
        }
    }
}
