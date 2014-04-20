using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionSimplifier.Parse;
using ExpressionSimplifier.TreeIteration;

namespace ExpressionSimplifier
{
    class Program
    {
        static void Main(String[] args)
        {
            String path = "Input.txt";
            List<String> lines = LineByLineReader.ReadInput(path);

            List<TreeNode> trees = new List<TreeNode>();
            foreach (String line in lines)
            {
                trees.Add(ExpressionParser.BuildTree(line));
            }


            foreach (var item in trees)
            {
                Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

                if (item == null)
                {
                    Console.WriteLine("Invalid parentheses\n");
                    continue;
                }

                Dimension dim;
                try
                {
                    dim = item.GetDimension();
                    Console.WriteLine("dim: [" + dim.M + "," + dim.N + "]\n");
                    Console.WriteLine("cost: " + item.Cost() + "\n");
                }
                catch (ApplicationException appEx)
                {
                    Console.WriteLine(appEx.Message + "\n");
                }
                finally
                {
                    Console.WriteLine(item.ToString());
                }

            }

            Console.ReadKey();
        }
    }
}
