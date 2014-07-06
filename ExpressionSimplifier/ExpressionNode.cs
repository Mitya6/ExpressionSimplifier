using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    public abstract class ExpressionNode : Node
    {
        public String DisplayName { get; set; }

        protected Cache cache;

        public ExpressionNode() { }

        public ExpressionNode(String name)
        {
            this.DisplayName = name;
        }

        /// <summary>
        /// Calculates the resulting dimension of the node recursively.
        /// </summary>
        public abstract Dimension GetDimension();

        /// <summary>
        /// Calculates the computation cost of the node recursively.
        /// </summary>
        public abstract int ComputationCost();

        /// <summary>
        /// Returns the count of temporarily stored floating point numbers
        /// needed to calculate the node.
        /// </summary>
        public abstract int TempStorageCost();

        public override String ToString()
        {
            if (cache.StringValid)
            {
                return cache.StringValue;
            }

            String value = ToString(false);
            cache.StringValue = value;
            return value;
        }

        private String ToString(bool hasMultiplicationAbove)
        {
            String s;
            if (this.IsLeaf)
            {
                s = this.DisplayName;
                if (this.DisplayName.StartsWith("-"))
                {
                    s = "(" + s + ")";
                }
                return s;
            }

            s = ((ExpressionNode)this.children[0]).ToString(this.Type == NodeType.Multiplication);
            for (int i = 1; i < this.children.Count; i++)
            {
                s += this.DisplayName;
                s += ((ExpressionNode)this.children[i]).ToString(this.Type == NodeType.Multiplication);
            }

            if (hasMultiplicationAbove && this.Type == NodeType.Addition)
            {
                s = "(" + s + ")";
            }

            return s;
        }

        public bool IsSameOperationAsParent()
        {
            return this.Parent != null && this.Parent.IsAlive &&
                (this.Type == NodeType.Addition &&
                ((Node)this.Parent.Target).Type == NodeType.Addition) ||
                (this.Type == NodeType.Multiplication &&
                ((Node)this.Parent.Target).Type == NodeType.Multiplication);
        }

        public void Invalidate()
        {
            cache.Invaildate();
            if (this.Parent != null && this.Parent.Target != null)
            {
                ((ExpressionNode)this.Parent.Target).Invalidate();
            }
        }


        #region Transformations

        /// <summary>
        /// Unifies an operation node with its parent node if they are of the same type.
        /// Preserves the order of child nodes.
        /// </summary>
        public ExpressionNode Raise()
        {
            if (this.IsSameOperationAsParent())
            {
                Node parent = this.Parent.Target as Node;

                if (parent != null)
                {
                    Invalidate();

                    int idx = parent.IndexOf(this);
                    for (int i = this.ChildrenCount() - 1; i >= 0; i--)
                    {
                        Node current = this.GetChild(i);
                        this.RemoveChild(current);
                        parent.InsertChild(current, idx);
                    }
                    parent.RemoveChild(this);
                }
            }

            return null;
        }

        /// <summary>
        /// Helper transformation: Removes the given node if it has only one child and
        /// connects the node's child to the node's parent node.
        /// </summary>
        private ExpressionNode ContractOneChildNode()
        {
            if ((this.Type == NodeType.Addition || this.Type == NodeType.Multiplication) &&
                this.ChildrenCount() == 1)
            {
                if (this.IsRoot)
                {
                    ExpressionNode child = (ExpressionNode)this.GetChild(0);
                    this.ClearChildren();
                    return child;
                }
                else
                {
                    ExpressionNode parent = (ExpressionNode)this.Parent.Target;
                    if (parent != null)
                    {
                        parent.AddChild(this.GetChild(0));
                        parent.RemoveChild(this);
                    }
                    this.ClearChildren();
                }
            }

            return null;
        }

        /// <summary>
        /// Performs all performable scalar operations on the node.
        /// </summary>
        public ExpressionNode PerformScalarOperation()
        {
            if ((this.Type != NodeType.Addition && this.Type != NodeType.Multiplication) ||
                this.children.FirstOrDefault(child => child.Type == NodeType.Scalar) == null)
                return null;

            this.OrderChildren();

            List<Scalar> scalarsWithValue = this.children.TakeWhile(child =>
                child.Type == NodeType.Scalar && ((Scalar)child).Value != null).Cast<Scalar>().ToList();
            if (scalarsWithValue.Count < 2) return null;

            double value = 0;
            if (this.Type == NodeType.Addition)
                value = (double)scalarsWithValue.Sum(scalar => scalar.Value);
            else if (this.Type == NodeType.Multiplication)
                value = scalarsWithValue.Aggregate(1.0, (val, s2) => val * (double)s2.Value);

            foreach (Scalar scalarWithValue in scalarsWithValue)
            {
                this.RemoveChild(scalarWithValue);
            }
            this.InsertChild(new Scalar(value.ToString()), 0);

            return this.ContractOneChildNode();
        }

        /// <summary>
        /// Rearranges the order of the child nodes if they are movable.
        /// </summary>
        public ExpressionNode OrderChildren()
        {
            if (!cache.ChildrenOrdered)
            {
                Invalidate();

                var scalarsWithValue = this.children
                    .Where(child => child.Type == NodeType.Scalar && ((Scalar)child).Value != null)
                    .OrderBy(child => ((Scalar)child).Value).ToList();
                var scalarsWithoutValue = this.children
                    .Where(child => child.Type == NodeType.Scalar && ((Scalar)child).Value == null)
                    .OrderBy(child => ((Scalar)child).Value).ToList();

                if (this.Type == NodeType.Addition)
                {
                    var otherOperands = this.children
                        .Where(child => child.Type == NodeType.Operand)
                        .OrderBy(child => ((ExpressionNode)child).DisplayName).ToList();
                    var additionNodes = this.children
                        .Where(child => child.Type == NodeType.Addition).ToList();
                    var multiplicationNodes = this.children
                        .Where(child => child.Type == NodeType.Multiplication).ToList();

                    this.ClearChildren();
                    this.children.AddRange(scalarsWithValue);
                    this.children.AddRange(scalarsWithoutValue);
                    this.children.AddRange(otherOperands);
                    this.children.AddRange(additionNodes);
                    this.children.AddRange(multiplicationNodes);

                    cache.ChildrenOrdered = true;
                }
                else if (this.Type == NodeType.Multiplication)
                {
                    List<Node> scalars = new List<Node>();
                    scalars.AddRange(scalarsWithValue);
                    scalars.AddRange(scalarsWithoutValue);

                    foreach (Node node in scalars)
                    {
                        this.RemoveChild(node);
                    }
                    for (int i = scalars.Count - 1; i >= 0; i--)
                    {
                        this.InsertChild(scalars[i], 0);
                    }

                    cache.ChildrenOrdered = true;
                }
            }
            return null;
        }

        /// <summary>
        /// Collects the common factors from the child nodes.
        /// </summary>
        public ExpressionNode CollectTerms()
        {
            if (this.Type == NodeType.Addition)
            {
                OrderChildren();

                List<ExpressionNode> mulChildNodes = this.children.Where(
                    child => child.Type == NodeType.Multiplication).Cast<ExpressionNode>().ToList();
                if (mulChildNodes.Count < 2) return null;

                // Can collect scalar grandchildren in any order
                List<List<Scalar>> scalarGrandchildNodes = new List<List<Scalar>>();

                // Can collect nonscalar grandchildren only in original order
                List<List<ExpressionNode>> otherGrandchildNodes = new List<List<ExpressionNode>>();

                foreach (ExpressionNode mulChild in mulChildNodes)
                {
                    mulChild.PerformScalarOperation();

                    List<Scalar> scalars = mulChild.children.TakeWhile(grandchild =>
                        grandchild.Type == NodeType.Scalar).Cast<Scalar>().ToList();
                    if (scalars.Count > 0)
                        scalarGrandchildNodes.Add(scalars);

                    List<ExpressionNode> others = mulChild.children.Where(grandchild =>
                        grandchild.Type == NodeType.Operand || grandchild.Type == NodeType.Addition ||
                        grandchild.Type == NodeType.Multiplication).Cast<ExpressionNode>().ToList();
                    if (others.Count > 0)
                        otherGrandchildNodes.Add(others);
                }

                // Find common scalars
                List<Scalar> likeScalars = FindLikeScalarFactors(scalarGrandchildNodes);

                // Find common nonscalar operands (front)
                List<ExpressionNode> likeFactorsFront = FindLikeFactors(otherGrandchildNodes);

                foreach (List<ExpressionNode> nodeList in otherGrandchildNodes)
                {
                    nodeList.Reverse();
                }

                // Find common nonscalar operands (back)
                List<ExpressionNode> likeFactorsBack = FindLikeFactors(otherGrandchildNodes);

                if (likeScalars.Count + likeFactorsFront.Count + likeFactorsBack.Count == 0) return null;


                // Reorganize expression tree
                Multiplication newMul = new Multiplication();
                Addition newAdd = new Addition();
                this.InsertChild(newMul, 0);
                
                foreach (ExpressionNode mulChild in mulChildNodes)
                {
                    foreach (Scalar scalar in likeScalars)
                    {
                        mulChild.RemoveChild(mulChild.children.First(child => 
                            ((Scalar)child).DisplayName == scalar.DisplayName));
                    }
                    foreach (ExpressionNode operand in likeFactorsFront)
                    {
                        mulChild.RemoveChild(mulChild.children.First(child => 
                            ((ExpressionNode)child).DisplayName == operand.DisplayName));
                    }
                    foreach (ExpressionNode operand in likeFactorsBack)
                    {
                        mulChild.RemoveChild(mulChild.children.Last(child => 
                            ((ExpressionNode)child).DisplayName == operand.DisplayName));
                    }

                    this.RemoveChild(mulChild);
                    newAdd.AddChild(mulChild);

                    if (mulChild.ChildrenCount() == 0) mulChild.AddChild(new Scalar("1"));
                    if (mulChild.ChildrenCount() == 1) mulChild.ContractOneChildNode();
                }

                foreach (Scalar scalar in likeScalars)
                {
                    newMul.AddChild(scalar);
                }
                foreach (ExpressionNode operand in likeFactorsFront)
                {
                    newMul.AddChild(operand);
                }
                newMul.AddChild(newAdd);

                likeFactorsBack.Reverse();
                foreach (ExpressionNode operand in likeFactorsBack)
                {
                    newMul.AddChild(operand);
                }
            }

            return null;
        }

        #endregion


        #region Helper methods

        private List<Scalar> FindLikeScalarFactors(List<List<Scalar>> scalarGrandchildNodes)
        {
            List<Scalar> likeFactors = new List<Scalar>();

            if (scalarGrandchildNodes.Count >= 2)
            {
                foreach (Scalar scalar in scalarGrandchildNodes[0])
                {
                    bool allListsContainFactor = true;

                    for (int i = 1; i < scalarGrandchildNodes.Count; i++)
                    {
                        bool match = false;
                        foreach (Scalar s in scalarGrandchildNodes[i])
                        {
                            if (s.DisplayName == scalar.DisplayName)
                            {
                                match = true;
                                break;
                            }
                        }

                        if (!match)
                        {
                            allListsContainFactor = false;
                            break;
                        }
                    }

                    if (allListsContainFactor)
                    {
                        likeFactors.Add(scalar);
                    }
                }
            }

            return likeFactors;
        }

        private List<ExpressionNode> FindLikeFactors(List<List<ExpressionNode>> otherGrandchildNodes)
        {
            List<ExpressionNode> likeFactors = new List<ExpressionNode>();

            if (otherGrandchildNodes.Count >= 2)
            {
                int minCount = otherGrandchildNodes.Min(nodeList => nodeList.Count);

                for (int i = 0; i < minCount; i++)
                {
                    bool match = true;
                    for (int j = 1; j < otherGrandchildNodes.Count; j++)
                    {
                        if (otherGrandchildNodes[0][i].DisplayName != otherGrandchildNodes[j][i].DisplayName ||
                            otherGrandchildNodes[0][i].Type != NodeType.Operand)
                        {
                            match = false;
                            break;
                        }
                    }

                    if (!match) break;

                    likeFactors.Add(otherGrandchildNodes[0][i]);
                }
            }

            foreach (List<ExpressionNode> nodeList in otherGrandchildNodes)
            {
                nodeList.RemoveRange(0, likeFactors.Count);
            }

            return likeFactors;
        }

        #endregion
    }
}
