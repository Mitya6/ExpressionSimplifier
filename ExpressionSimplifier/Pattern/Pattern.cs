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

        public static ExpressionNode PerformScalarOperationFirst(ExpressionNode root)
        {
            Pattern pattern = CreatePerformScalarOperationPattern();
            ExpressionNode scalarOperationNode = pattern.FindFirst(root);

            if (scalarOperationNode == null) return null;

            return scalarOperationNode.PerformScalarOperation();
        }

        private static Pattern CreatePerformScalarOperationPattern()
        {
            PatternNode node = new PatternNode(new NodeType[] { NodeType.Addition, NodeType.Multiplication });
            node.AddChild(new PatternNode(NodeType.Scalar));
            node.AddChild(new PatternNode(NodeType.Scalar));
            return new Pattern(node);
        }

        private ExpressionNode FindFirst(ExpressionNode root)
        {
            BFSIterator iter = root.GetBFSIterator();
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
