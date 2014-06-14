using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionSimplifier.TreeIteration;

namespace ExpressionSimplifier
{
    public class Expression
    {
        private ExpressionNode root;
        public ExpressionNode Root
        {
            get { return root; }
            set
            {
                root = value;
                root.Parent = null;
            }
        }

        public String DisplayName { get; set; }

        public Expression(String displayName, ExpressionNode root)
        {
            this.DisplayName = displayName;
            if (root == null) return;

            this.Root = root;

            // Set containing expression reference in all children
            if (root != null)
            {
                BFSIterator iter = root.GetBFSIterator();
                while (iter.HasNext())
                {
                    ((ExpressionNode)(iter.Next())).Expr = this;
                }
            }
        }

        public override String ToString()
        {
            return this.DisplayName;
        }
    }
}
