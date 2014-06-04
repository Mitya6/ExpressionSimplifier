using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier.Pattern
{
    class PatternNode : Node
    {
        private NodeType[] matchingTypes;
        private String displayName;
        private double? value;

        public PatternNode(NodeType[] matchingTypes, String displayName = null, double? value = null)
        {
            this.matchingTypes = matchingTypes;
            this.displayName = displayName;
            this.value = value;
        }

        public PatternNode(NodeType matchingType, String displayName = null, double? value = null)
        {
            this.matchingTypes = new NodeType[] { matchingType };
            this.displayName = displayName;
            this.value = value;
        }

        public bool Compare(ExpressionNode node)
        {
            bool result = this.matchingTypes.Contains(node.Type);
            if (this.displayName != null)
            {
                result = result && this.displayName == node.DisplayName;
            }
            if (this.value != null && node is Scalar && ((Scalar)node).Value != null)
            {
                result = result && Math.Abs((double)(this.value - ((Scalar)node).Value))
                    < Scalar.Epsilon;
            }
            return result;
        }
    }
}
