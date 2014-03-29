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
            return Parse(expression, false, false);
        }
        
        private static TreeNode Parse(String expression, bool isParentSubtraction, bool isParentDivision)
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
                /*node = new Matrix(Regex.Match(expression, @"[a-zA-Z]+").Value,
                    Int32.Parse(Regex.Match(parts[0], @"[0-9]+").Value),
                    Int32.Parse(Regex.Match(parts[1], @"[0-9]+").Value));*/
                node = new Operand(Regex.Match(expression, @"[a-zA-Z]+").Value,
                    new Dimension(Int32.Parse(Regex.Match(parts[0], @"[0-9]+").Value),
                    Int32.Parse(Regex.Match(parts[1], @"[0-9]+").Value)));
            }
            // Vector
            else if (expression.Contains('['))
            {
                /*node = new Vector(Regex.Match(expression, @"[a-zA-Z]+").Value,
                    Int32.Parse(Regex.Match(expression, @"[0-9]+").Value));*/
                node = new Operand(Regex.Match(expression, @"[a-zA-Z]+").Value,
                    new Dimension(Int32.Parse(Regex.Match(expression, @"[0-9]+").Value),
                    1));
            }
            // Scalar
            else
            {
                //node = new Scalar(Regex.Match(expression, @"[a-zA-Z0-9]+").Value);
                node = new Operand(Regex.Match(expression, @"[a-zA-Z0-9]+").Value,
                    new Dimension(1, 1));
            }

            if (node == null)
            {
                throw new ApplicationException("Parse error!");
            }

            // kivonás összeadássá alakítása itt 
            if (isParentSubtraction)
            {
                TreeNode helperNode = new Multiplication();
                helperNode.LeftChild = new Operand("-1", new Dimension(1, 1));
                helperNode.RightChild = node;
                node = helperNode;
            }


            // osztás szorzássá alakítása itt
            if (isParentDivision)
            {
                // elvileg itt már csak skalár lehet a jobb oldalon
                ((Operand)node).Name = "1/" + ((Operand)node).Name;
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
                        node = CreateOperatorNode(ops[j]);
                        node.LeftChild = Parse(expression.Substring(0, i), false, false);
                        if (ops[j] == '-')
                        {
                            node.RightChild = Parse(expression.Substring(i + 1,
                                                expression.Length - (i + 1)), true, false); 
                        }
                        else if (ops[j] == '/')
                        {
                            node.RightChild = Parse(expression.Substring(i + 1,
                                                expression.Length - (i + 1)), false, true); 
                        }
                        else
                        {
                            node.RightChild = Parse(expression.Substring(i + 1,
                                                expression.Length - (i + 1)), false, false);
                        }
                        return node;
                    }
                }
            }
            return node;
        }

        private static TreeNode CreateOperatorNode(char p)
        {
            TreeNode node = null;
            switch (p)
            {
                case '+':
                    {
                        node = new Addition();
                        break;
                    }
                case '-':
                    {
                        node = new Addition();
                        break;
                    }
                case '*':
                    {
                        node = new Multiplication();
                        break;
                    }
                case '/':
                    {
                        node = new Multiplication();
                        break;
                    }
                default:
                    throw new ApplicationException("Unknown operator: " + p);
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