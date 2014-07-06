using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier
{
    public struct Cache
    {
        private bool dimensionValid;
        private bool computationCostValid;
        private bool storageCostValid;
        private bool stringValid;

        private Dimension dim;
        private int computationCost;
        private int storageCost;
        private String stringValue;

        public bool DimensionValid { get { return dimensionValid; } }
        public bool ComputationCostValid { get { return computationCostValid; } }
        public bool StorageCostValid { get { return storageCostValid; } }
        public bool StringValid { get { return stringValid; } }

        public Dimension Dim 
        {
            get { return dim; } 
            set
            {
                dim = value;
                dimensionValid = true;
            }
        }

        public int ComputationCost
        {
            get { return computationCost; }
            set
            {
                computationCost = value;
                computationCostValid = true;
            }
        }

        public int StorageCost
        {
            get { return storageCost; }
            set
            {
                storageCost = value;
                storageCostValid = true;
            }
        }

        public String StringValue
        {
            get { return stringValue; }
            set
            {
                stringValue = value;
                stringValid = true;
            }
        }

        public bool ChildrenOrdered { get; set; }

        public void Invaildate()
        {
            dimensionValid = false;
            computationCostValid = false;
            storageCostValid = false;
            stringValid = false;
            ChildrenOrdered = false;
        }
    }
}
