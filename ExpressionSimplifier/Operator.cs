using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    internal abstract class Operator : TreeNode
    {
        public static Operator CreateOperator(char op)
        {
            Operator o = null;
            if (op == '+')
            {
                o = new Addition();
            }
            if (op == '-')
            {
                o = new Subtraction();
            }
            if (op == '*')
            {
                o = new Multiplication();
            }
            if (op == '/')
            {
                o = new Division();
            }
            return o;
        }
    }
}
