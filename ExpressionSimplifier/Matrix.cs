using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    internal class Matrix : Operand
    {
        public Matrix(String name, int n, int m)
        {
            this.Name = name;
            this.dimensions = new Dimension(n, m);
        }
    }
}
