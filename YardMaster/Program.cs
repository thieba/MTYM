using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardMaster
{
    class Program
    {
        static void Main(string[] args)
        {
            string path;
            if (args.Length > 0)
                path = args[0];
            else
                path = GetAFile();
            var yardMaster = new YardMaster(path);
            yardMaster.Resolve();
        }

        static string GetAFile()
        {
            Console.WriteLine("Enter a input file:");
            return Console.ReadLine();
        }
    }
}