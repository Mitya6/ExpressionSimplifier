using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    public struct Dimension
    {
        public int M;
        public int N;

        public bool Is1x1 { get { return this.M == 1 && this.N == 1; } }

        public Dimension(int m, int n)
        {
            this.M = m;
            this.N = n;
        }

        public override String ToString()
        {
            return String.Format("[{0},{1}]", this.M, this.N);
        }
    }
}
