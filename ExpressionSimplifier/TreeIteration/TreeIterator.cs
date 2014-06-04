using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier.TreeIteration
{
    interface TreeIterator
    {
        Node Next();

        bool HasNext();
    }
}
