using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    /// <summary>
    /// Helper class. Contains static methods to convert an expression
    /// string to an expression tree.
    /// </summary>
    internal static class ExpressionParser
    {
        /// <summary>
        /// Creates the tree representation of the given expression string
        /// and returns its root tree node.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static TreeNode Parse(String expression)
        {
            RemoveOuterParentheses(ref expression);

            TreeNode node = null;

            // Try splitting by '+' or '-' outside ()
            node = SplitByOperation(expression, new char[] { '+', '-' });
            if (node != null)
            {
                return node;
            }

            // Try splitting by '*' or '/' outside ()
            node = SplitByOperation(expression, new char[] { '*', '/' });
            if (node != null)
            {
                return node;
            }

            // Matrix
            if (expression.Contains(','))
            {
                String[] parts = expression.Split(new char[] { ',' });
                node = new Matrix(Regex.Match(expression, @"[a-zA-Z]+").Value,
                    Int32.Parse(Regex.Match(parts[0], @"[0-9]+").Value),
                    Int32.Parse(Regex.Match(parts[1], @"[0-9]+").Value));
            }
            // Vector
            else if (expression.Contains('['))
            {
                node = new Vector(Regex.Match(expression, @"[a-zA-Z]+").Value,
                    Int32.Parse(Regex.Match(expression, @"[0-9]+").Value));
            }
            // Scalar
            else
            {
                node = new Scalar(Regex.Match(expression, @"[a-zA-Z0-9]+").Value);
            }

            if (node == null)
            {
                throw new ApplicationException("Parse error!");
            }
            return node;
        }

        /// <summary>
        /// Splits the given expression into two subexpressions at the last
        /// valid occurence of any operator in the ops array, and returns a 
        /// TreeNode representing this operator. If a split can be done, the
        /// function recursively parses the left and right subexpressions.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="ops"></param>
        /// <returns></returns>
        private static TreeNode SplitByOperation(String expression, char[] ops)
        {
            TreeNode node = null;
            int insideParentheses = 0;

            // Reverse order for correct operator precedence
            for (int i = expression.Length - 1; i >= 0; i--)
            {
                if (expression[i] == ')')
                {
                    insideParentheses++;
                    continue;
                }
                if (expression[i] == '(')
                {
                    insideParentheses--;
                    continue;
                }

                // Do the splitting if appropriate
                for (int j = 0; j < ops.Length; j++)
                {
                    if (insideParentheses == 0 && expression[i] == ops[j])
                    {
                        node = Operator.CreateOperator(ops[j]);
                        node.LeftChild = Parse(expression.Substring(0, i));
                        node.RightChild = Parse(expression.Substring(i + 1,
                            expression.Length - (i + 1)));
                        return node;
                    }
                }
            }
            return node;
        }

        /// <summary>
        /// Checks if there are outer parentheses surrounding the expression
        /// and removes them if possible.
        /// </summary>
        /// <param name="expression"></param>
        private static void RemoveOuterParentheses(ref String expression)
        {
            if (!expression.StartsWith("("))
            {
                return;
            }

            int parentheses = 0;
            for (int i = 0; i < expression.Length - 1; i++)
            {
                if (expression[i] == '(')
                {
                    parentheses++;
                    continue;
                }
                if (expression[i] == ')')
                {
                    parentheses--;
                    if (parentheses == 0)
                    {
                        return;
                    }
                    continue;
                }
            }

            if (expression[expression.Length - 1] != ')')
            {
                return;
            }

            expression = expression.Substring(1, expression.Length - 2);
        }
    }
}