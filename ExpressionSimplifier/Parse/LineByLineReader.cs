using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSimplifier.Parse
{
    public class LineByLineReader
    {
        public static List<String> ReadInput(String path)
        {
            List<String> lines = new List<String>();

            using (StreamReader reader = File.OpenText(path))
            {
                String line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line[0] != '#')
                    {
                        lines.Add(line); 
                    }
                }
            }

            return lines;
        }
    }
}
