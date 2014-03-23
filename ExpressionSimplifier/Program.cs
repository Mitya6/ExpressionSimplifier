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

            List<ExpressionTree> trees = new List<ExpressionTree>();
            foreach (String line in lines)
            {
                trees.Add(new ExpressionTree(line));
            }


            foreach (var item in trees)
            {
                Dimension dim;
                try
                {
                    dim = item.GetDimensions();
                }
                catch (ApplicationException appEx)
                {
                    Console.WriteLine(appEx.Message);
                    continue;
                }
                Console.WriteLine("\n\n [" + dim.N + "," + dim.M + "]");
                Console.WriteLine(item.ToString());
            }
        }
    }
}
