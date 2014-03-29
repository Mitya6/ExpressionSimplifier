using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                trees.Add(ExpressionParser.Parse(line));
            }


            foreach (var item in trees)
            {
                Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

                Dimension dim;
                try
                {
                    dim = item.GetDimension();
                    Console.WriteLine("dim: [" + dim.N + "," + dim.M + "]\n");
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
