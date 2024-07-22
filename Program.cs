using System;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using Luxi;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0){
            Console.WriteLine("No argument is given. Please specify the constraint: e/c/53/64/odd/f3/xc/w.");
        }
        else if(args[0] is "xc" or "w"){
            Cube4Class cc = new Cube4Class();
            if (args[0] == "xc"){
                cc.scrambleXCenter = true;
                cc.wingLimit = x => x.CodeLength == 0;
                cc.cornerLimit = x => x.CodeLength == 0;
            }
            else if (args[0] == "w"){
                cc.scrambleXCenter = false;
                cc.cornerLimit = x => x.CodeLength == 0;
            }
            cc.Init();
            // Console.WriteLine($"The probability of your choice is {cc.probability}.");
            while(true){
                Console.Clear();
                Console.Write(cc.GetCube4().GetScramble());
                Console.ReadKey(true);
            }
        }
        else{
            Cube3Class cc = new Cube3Class();
            if(args[0] == "e"){ // Scramble edge only
                cc.cornerLimit = x => x.CodeLength == 0;
            }
            else if(args[0] == "c"){ // Scramble corner only
                cc.edgeLimit = x => x.CodeLength == 0;
            }
            else if(args[0] == "53"){ // Easy scramble for recovery
                cc.edgeLimit = x => x.CodeLength == 10;
                cc.cornerLimit = x => x.CodeLength == 6;
            }
            else if(args[0] == "64"){ // No-fliping scramble for recovery
                cc.edgeLimit = x => x.FlipCount == 0;
                cc.cornerLimit = x => x.TwistCount == 0;
            }
            else if(args[0] == "odd"){ // Odd Parity only
                cc.parityLimit = x => x;
            }
            else if(args[0] == "f3"){ // Floating 3-cycle
                cc.edgeLimit   = x => x.OtherCycles.Contains((3,0));
                cc.cornerLimit = x => x.OtherCycles.Contains((3,0));
            }
            cc.Init();
            // Console.WriteLine($"The probability of your choice is {cc.probability}.");
            while(true){
                Console.Clear();
                Console.Write(cc.GetCube3().GetScramble());
                Console.ReadKey(true);
            }
        }
    }
}
