
using Luxi;

class Program
{
    static void Main(string[] args)
    {
        var cc = new Cube3Class{
            parityConstraint = x => x == 1,
            cornerConstraint = x => x.TwistCount != 0,
        };
        cc.Init();

        Console.WriteLine($"Probability={cc.probability}"); // Probability=0.221898338667886
    }
}
