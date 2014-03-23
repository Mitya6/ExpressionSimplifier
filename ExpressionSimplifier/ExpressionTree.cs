using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionSimplifier
{
    internal class ExpressionTree
    {
        public TreeNode Root { get; set; }
        public String Expression { get; set; }

        public ExpressionTree(String expression)
        {
            this.Root = ExpressionParser.Parse(expression);
            this.Expression = expression;
        }

        /// <summary>
        /// Returns the string representation of the expression tree
        /// if it is not deeper than the limit.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append(this.Expression);
            sb.Append("\n");

            if (this.Depth() > 6)
            {
                return sb.Append("Expression Tree too big.").ToString();
            }

            List<TreeNode> sameLevelNodes = null;
            for (int i = 1; i <= this.Depth(); i++)
            {
                sameLevelNodes = new List<TreeNode>();
                this.Root.GetLevelNodes(i, sameLevelNodes);
                foreach (TreeNode node in sameLevelNodes)
                {
                    sb.Append(GetSpace(i, 64));
                    sb.Append(node.ToString());
                    sb.Append(GetSpace(i, 64));
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns the depth of the expression tree.
        /// </summary>
        /// <returns></returns>
        public int Depth()
        {
            return this.Root.Depth();
        }

        public Dimension GetDimensions()
        {
            return this.Root.GetDimensions();
        }

        private String GetSpace(int level, int width)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < width/Math.Pow(2, level); i++)
            {
                sb.Append(" ");
            }
            return sb.ToString();
        }
    }
}
