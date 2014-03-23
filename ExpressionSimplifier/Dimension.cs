using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    internal struct Dimension
    {
        public int N;
        public int M;

        public Dimension(int n, int m)
        {
            this.N = n;
            this.M = m;
        }
    }
}
