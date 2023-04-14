using System;
using System.Diagnostics;
using System.Threading;
using Luxi;

class Program
{
    static void Main(string[] args)
    {
        var cc = new Cube3Class{
            parityLimit = x => x,
            edgeLimit = x => x.CodeLength is >=10 and <= 12 && x.FlipCount == 0,
            cornerLimit = x => x.CodeLength is >=6 and <= 8 && x.TwistCount == 0,
        };
        cc.Init();

        Console.WriteLine($"Probability={cc.probability}");

        Stopwatch sw = new Stopwatch();
        while(true){
            Console.WriteLine(cc.GetCube3().GetScramble());
            Console.WriteLine();
            Console.ReadKey(true);
            sw.Start();
            Console.WriteLine("...");
            Console.ReadKey(true);
            sw.Stop();
            Console.WriteLine($"{sw.Elapsed:c}");
            sw.Reset();
            Console.WriteLine();
        }
    }
}
