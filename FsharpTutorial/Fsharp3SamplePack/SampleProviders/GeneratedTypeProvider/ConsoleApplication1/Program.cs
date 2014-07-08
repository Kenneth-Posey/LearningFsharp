using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = new FSharpLib.T();
            var c = t.F1(13);

            Console.WriteLine("value is {0}", c);
        }
    }
}
