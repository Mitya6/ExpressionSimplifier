using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionSimplifier
{
    class Scalar : Operand
    {
        public double? Value { get; set; }

        public const double Epsilon = 0.00001;

        public Scalar(String name)
        {
            this.type = NodeType.Scalar;
            this.dimension = new Dimension(1, 1);

            this.Value = Convert(name);

            if (Value != null)
            {
                this.DisplayName = String.Format("{0:0.####}", this.Value);
            }
            else
            {
                this.DisplayName = name;
            }
        }

        private double? Convert(String name)
        {
            double? number = null;
            if (name.Contains('/'))
            {
                List<String> stringList = name.Split('/').ToList();
                if (stringList.Count != 2)
                {
                    throw new ApplicationException("Error: Invalid division arguments!");
                }

                double n1, n2;
                bool result1 = Double.TryParse(stringList[0], out n1);
                bool result2 = Double.TryParse(stringList[1], out n2);

                if (result1 && result2)
                {
                    number = n1 / n2;
                }
            }
            else
            {
                double n;
                if (Double.TryParse(name, out n))
                {
                    number = n;
                }
            }
            return number;
        }
    }
}
