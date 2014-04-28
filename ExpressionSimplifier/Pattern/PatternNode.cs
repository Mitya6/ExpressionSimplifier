using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier.Pattern
{
    internal class PatternNode : Node
    {
        private Type[] matchingTypes;
        private String displayName;
        private double? value;

        public PatternNode(Type[] matchingTypes, String displayName = null, double? value = null)
        {
            this.matchingTypes = matchingTypes;
            this.displayName = displayName;
            this.value = value;
        }

        public PatternNode(Type matchingType, String displayName = null, double? value = null)
        {
            this.matchingTypes = new Type[] { matchingType };
            this.displayName = displayName;
            this.value = value;
        }

        public bool Compare(ExpressionNode node)
        {
            bool result = this.matchingTypes.Contains(node.GetType());
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
