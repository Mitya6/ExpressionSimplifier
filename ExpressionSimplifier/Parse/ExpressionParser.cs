using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExpressionSimplifier.Parse
{
    /// <summary>
    /// Helper class. Contains static methods to convert an expression
    /// string to an expression tree.
    /// </summary>
    public static class ExpressionParser
    {
        private static String matrixPattern = @"[a-zA-Z]+\[\d+,\d+\]";
        private static String vectorPattern = @"[a-zA-Z]+\[\d+\]";
        private static String variableNamePattern = @"[a-zA-Z]+";
        private static String posIntegerPattern = @"[1-9]\d*";
        private static String numberPattern = @"0|(-?[1-9]\d*(\.\d+)?)";

        private static List<char[]> operators = new List<char[]>();

        static ExpressionParser()
        {
            operators.Add(new char[] { '+', '-' });
            operators.Add(new char[] { '*', '/' });
        }


        /// <summary>
        /// Creates the tree representation of the given expression string
        /// and returns its root tree node.
        /// </summary>
        public static ExpressionNode BuildTree(String expression)
        {
            if (CheckParentheses(expression))
            {
                try
                {
                    return Parse(expression);
                }
                catch (ApplicationException)
                {
                    return null;
                }
                catch (NullReferenceException)
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Recursive function that breaks the specified expression into two
        /// subexpressions for further parsing when it finds an operator,
        /// otherwise it creates operands as leaves in the expression tree.
        /// </summary>
        private static ExpressionNode Parse(String expression)
        {
            RemoveOuterParentheses(ref expression);

            ExpressionNode node = Split(expression);
            if (node != null)
            {
                return node;
            }

            // Matrix
            if (Regex.IsMatch(expression, matrixPattern))
            {
                String[] parts = expression.Split(new char[] { ',' });
                node = new Operand(Regex.Match(expression, variableNamePattern).Value,
                    new Dimension(Int32.Parse(Regex.Match(parts[0], posIntegerPattern).Value),
                    Int32.Parse(Regex.Match(parts[1], posIntegerPattern).Value)));
            }
            // Vector
            else if (Regex.IsMatch(expression, vectorPattern))
            {
                node = new Operand(Regex.Match(expression, variableNamePattern).Value,
                    new Dimension(Int32.Parse(Regex.Match(expression, posIntegerPattern).Value),
                    1));
            }
            // Scalar
            else if (!Regex.IsMatch(expression, @"[^a-zA-Z\d-\.]"))
            {
                Match m = Regex.Match(expression, variableNamePattern);
                // Scalar variable
                if (m.Success)
                {
                    node = new Scalar(m.Value);
                }
                // Scalar with value
                else if ((m = Regex.Match(expression, numberPattern)).Success)
                {
                    node = new Scalar(m.Value);
                }
            }

            return node;
        }

        /// <summary>
        /// Splits the specified expression into two subexpressions at the last
        /// valid occurence of an operator with the least precedence, and returns an
        /// ExpressionNode representing this operator. If a split can be done, the
        /// function recursively parses the left and right subexpressions.
        /// </summary>
        private static ExpressionNode Split(String expression)
        {
            ExpressionNode node = null;
            int insideParentheses = 0;

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
                            String left = expression.Substring(0, i);
                            String right = expression.Substring(i + 1, expression.Length - (i + 1));

                            switch (ops[j])
                            {
                                case '+':
                                    {
                                        node = new Addition();
                                        break;
                                    }
                                case '-':
                                    {
                                        if (left != "")
                                            node = new Addition();
                                        else
                                        {
                                            node = new Scalar(expression);
                                            return node;
                                        }
                                        break;
                                    }
                                case '*':
                                    {
                                        node = new Multiplication();
                                        break;
                                    }
                                case '/':
                                    {
                                        RemoveOuterParentheses(ref right);
                                        if (!Regex.IsMatch(right, "^(" + numberPattern + ")$"))
                                        {
                                            throw new ApplicationException("Error: Division by non scalar!");
                                        }
                                        node = new Multiplication();
                                        break;
                                    }
                            }

                            node.AddChild(Parse(left));

                            if (ops[j] == '-')
                            {
                                Multiplication mul = new Multiplication();
                                mul.AddChild(new Scalar("-1"));
                                mul.AddChild(Parse(right));
                                node.AddChild(mul);
                            }
                            else if (ops[j] == '/')
                            {
                                node.AddChild(new Scalar("1/" + right));
                            }
                            else
                            {
                                node.AddChild(Parse(right));
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