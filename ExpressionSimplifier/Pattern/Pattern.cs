using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionSimplifier.TreeIteration;

namespace ExpressionSimplifier.Pattern
{
    public class Pattern
    {
        private PatternNode pattern;

        private Pattern(PatternNode pattern)
        {
            this.pattern = pattern;
        }

        public static void DeleteZeroAdditionFirst(Expression expr)
        {
            Pattern pattern = CreateDeleteZeroAdditionPattern();
            ExpressionNode parent = pattern.FindFirst(expr);

            if (parent == null) return;

            ExpressionNode zeroChild = (ExpressionNode)(parent.GetFirstChild(child =>
            {
                Scalar scalarchild = child as Scalar;
                if (scalarchild != null)
                {
                    if (scalarchild.DisplayName == "0")
                    {
                        return true;
                    }
                }
                return false;
            }));

            expr.DeleteZeroAddition(zeroChild);
        }

        private static Pattern CreateDeleteZeroAdditionPattern()
        {
            PatternNode node = new PatternNode(typeof(Addition));
            node.AddChild(new PatternNode(typeof(Scalar), null, 0));
            return new Pattern(node);
        }

        public static void PerformScalarOperationFirst(Expression expr)
        {
            Pattern pattern = CreatePerformScalarOperationPattern();
            ExpressionNode parent = pattern.FindFirst(expr);

            if (parent == null) return;
            expr.PerformScalarOperation(parent);
        }

        private static Pattern CreatePerformScalarOperationPattern()
        {
            PatternNode node = new PatternNode(
                new Type[] { typeof(Addition), typeof(Multiplication) });
            node.AddChild(new PatternNode(typeof(Scalar)));
            node.AddChild(new PatternNode(typeof(Scalar)));
            return new Pattern(node);
        }


        private ExpressionNode FindFirst(Expression expr)
        {
            BFSIterator iter = expr.Root.GetBFSIterator();
            while (iter.HasNext())
            {
                ExpressionNode current = (ExpressionNode)(iter.Next());
                if (TryMatch(current, this.pattern))
                {
                    return current;
                }
            }
            return null;
        }

        private bool TryMatch(ExpressionNode node, PatternNode pattern)
        {
            if (pattern.Compare(node))
            {
                if (pattern.ChildrenCount() == 0)
                {
                    return true;
                }

                int i = 0;
                for (int j = 0; j < node.ChildrenCount(); j++)
                {
                    if (TryMatch((ExpressionNode)(node.GetChild(j)),
                        (PatternNode)(pattern.GetChild(i))))
                    {
                        i++;
                        if (i >= pattern.ChildrenCount())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
