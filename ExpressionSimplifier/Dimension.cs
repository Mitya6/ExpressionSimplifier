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

        public bool Is1x1 { get { return this.N == 1 && this.M == 1; } }

        public Dimension(int n, int m)
        {
            this.N = n;
            this.M = m;
        }
    }
}
