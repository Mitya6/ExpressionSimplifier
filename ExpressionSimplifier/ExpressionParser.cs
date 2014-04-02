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
        public static TreeNode BuildTree(String expression)
        {
            if (CheckParentheses(expression))
            {
                return Parse(expression);
            }
            return null;
        }

        /// <summary>
        /// Recursive function that breaks the specified expression into two
        /// subexpressions for further parsing when it finds an operator,
        /// otherwise it creates operands as leaves in the expression tree.
        /// </summary>
        private static TreeNode Parse(String expression)
        {
            RemoveOuterParentheses(ref expression);

            TreeNode node = Split(expression);
            if (node != null)
            {
                return node;
            }

            // Matrix
            if (expression.Contains(','))
            {
                String[] parts = expression.Split(new char[] { ',' });
                node = new Operand(Regex.Match(expression, @"[a-zA-Z]+").Value,
                    new Dimension(Int32.Parse(Regex.Match(parts[0], @"[0-9]+").Value),
                    Int32.Parse(Regex.Match(parts[1], @"[0-9]+").Value)));
            }
            // Vector
            else if (expression.Contains('['))
            {
                node = new Operand(Regex.Match(expression, @"[a-zA-Z]+").Value,
                    new Dimension(Int32.Parse(Regex.Match(expression, @"[0-9]+").Value),
                    1));
            }
            // Scalar
            else
            {
                node = new Scalar(Regex.Match(expression, @"[a-zA-Z0-9./]+").Value,
                    new Dimension(1, 1));
            }

            if (node == null)
            {
                throw new ApplicationException("Parse error!");
            }

            return node;
        }

        /// <summary>
        /// Splits the specified expression into two subexpressions at the last
        /// valid occurence of an operator with the least precedence, and returns a
        /// TreeNode representing this operator. If a split can be done, the
        /// function recursively parses the left and right subexpressions.
        /// </summary>
        private static TreeNode Split(String expression)
        {
            TreeNode node = null;
            int insideParentheses = 0;
            List<char[]> operators = new List<char[]>();
            operators.Add(new char[] { '+', '-' });
            operators.Add(new char[] { '*', '/' });

            foreach (char[] ops in operators)
            {
                // Reverse processing for correct operator order
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
                            switch (ops[j])
                            {
                                case '+':
                                case '-':
                                    {
                                        node = new Addition();
                                        break;
                                    }
                                case '*':
                                case '/':
                                    {
                                        node = new Multiplication();
                                        break;
                                    }
                            }

                            String left = expression.Substring(0, i);
                            String right = expression.Substring(i + 1, expression.Length - (i + 1));

                            node.LeftChild = Parse(left);

                            if (ops[j] == '-')
                            {
                                node.RightChild = new Multiplication();
                                node.RightChild.LeftChild = new Operand("-1", new Dimension(1, 1));
                                node.RightChild.RightChild = Parse(right);
                            }
                            else if (ops[j] == '/')
                            {
                                node.RightChild = new Scalar(Regex.Match("1/" + right,
                                    @"[a-zA-Z0-9./]+").Value, new Dimension(1, 1));
                            }
                            else
                            {
                                node.RightChild = Parse(right);
                            }

                            return node;
                        }
                    }
                }
            }
            return node;
        }

        /// <summary>
        /// Checks whether every opening parenthesis has its closing parenthesis
        /// pair in the specified expression.
        /// </summary>
        private static bool CheckParentheses(String expression)
        {
            int insideParentheses = 0;

            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == '(')
                {
                    insideParentheses++;
                }
                else if (expression[i] == ')')
                {
                    insideParentheses--;
                }
            }

            return insideParentheses == 0;
        }

        /// <summary>
        /// Checks if there are outer parentheses surrounding the specified
        /// expression and removes them if possible.
        /// </summary>
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
                }
                else if (expression[i] == ')')
                {
                    parentheses--;
                    if (parentheses == 0)
                    {
                        return;
                    }
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