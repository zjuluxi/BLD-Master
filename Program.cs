
using Luxi;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0){
            Console.WriteLine("No argument is given. Please specify the constraint: dotnet run e/c/53/flip/parity/float3/cyclist/xc/w.");
        }
        else if(args[0] is "xc" or "w"){
            Cube4Class cc = new();
            if (args[0] == "xc"){
                cc.scrambleXCenter = true;
                cc.wingConstraint = x => x.CodeLength == 0;
                cc.cornerConstranit = x => x.CodeLength == 0;
            }
            else if (args[0] == "w"){
                cc.scrambleXCenter = false;
                cc.cornerConstranit = x => x.CodeLength == 0;
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
            Cube3Class cc = new();
            if(args[0] == "e"){ // Scramble edge only
                cc.cornerConstraint = x => x.CodeLength == 0;
            }
            else if(args[0] == "c"){ // Scramble corner only
                cc.edgeConstraint = x => x.CodeLength == 0;
            }
            else if(args[0] == "53"){ // Easy scramble for recovery
                cc.edgeConstraint = x => x.CodeLength == 10;
                cc.cornerConstraint = x => x.CodeLength == 6;
            }
            else if(args[0] == "flip"){ // Happy flipping and twisting!
                cc.edgeConstraint = x => x.FlipCount >= 2;
                cc.cornerConstraint = x => x.TwistCount >= 2;
            }
            else if(args[0] == "parity"){ // Focus on full parity?
                cc.parityConstraint = x => x == 1;
                cc.edgeConstraint = x => x.OtherCycleCount == 0 && x.FlipCount == 0;
                cc.cornerConstraint = x => x.OtherCycleCount == 0 && x.TwistCount == 0;
            }
            else if(args[0] == "float3"){ // Floating 3-cycle
                cc.edgeConstraint   = x => x.OtherCycles.Contains((3,0));
                cc.cornerConstraint = x => x.OtherCycles.Contains((3,0));
            }
            else if(args[0] == "cyclist"){ // King of cycling
                cc.edgeConstraint = x => x.OtherCycleCount >= 2;
                cc.cornerConstraint = x => x.OtherCycleCount >= 2;
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
