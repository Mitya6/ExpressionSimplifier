using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionSimplifier
{
    internal class Scalar : Operand
    {
        public double Value { get; set; }

        public const double Epsilon = 0.00001;

        public Scalar(String name, Dimension dimension) : base(name, dimension)
        {
            double number;
            if (this.TryConvert(name, out number))
            {
                this.Value = number;
                this.Name = number.ToString();
            }
            else
            {
                this.Value = Double.NaN; 
            }
        }

        private bool TryConvert(String name, out double number)
        {
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
                    return true;
                }

                number = Double.NaN;
                return false;
            }
            return Double.TryParse(name, out number);
        }
    }
}
