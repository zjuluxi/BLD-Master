using static Luxi.Tools;

namespace Luxi
{
    public class EdgeCC
    {
        public readonly State FirstCycle;
        public readonly State[] OtherCycles;
        private const int _Perm = 12, _Ori = 2;
        public const long Sum = 980995276800;
        public readonly int CodeLength, OtherCycleCount, FlipCount, Parity;
        public readonly long Count;

        public EdgeCC(int FirstCycleLength, State[] OtherCycles)
        {
            this.FirstCycle = (FirstCycleLength, (_Perm * _Ori - OtherCycles.Sum(x => x.ori)) % _Ori);
            this.OtherCycles = OtherCycles;
            CodeLength = OtherCycles.Sum(x => x.perm > 1 ? x.perm + 1 : x.ori * 2) + FirstCycleLength - 1;
            OtherCycleCount = OtherCycles.Count(x => x.perm != 1);
            Parity = CodeLength & 1;
            FlipCount = OtherCycles.Count(x => x.perm == 1 && x.ori > 0);
            Count = FactI64[_Perm - 1];
            foreach (var i in OtherCycles)
                Count /= i.perm;
            Count *= 1L << (_Perm - 1 - OtherCycles.Length);
            foreach (var i in OtherCycles.GroupBy(x => x))
                Count /= FactI64[i.Count()];
        }
        public Edge GetInstance(int Buffer=0)
        {
            State[] instance = new State[_Perm];
            int head, remain = FirstCycle.perm, current, i = 0, color = 0;
            SetNPerm(rd.Next(479001600), 12, out int[] order);
            SetNFlip(rd.Next(2048), 12, out int[] colors);
            current = head = Buffer >> 1;
            order[Array.IndexOf(order, head)] = order[0];
            order[0] = head;
            while ((--remain) > 0)
            {
                i++;
                instance[current].perm = order[i];
                instance[current].ori = colors[i];
                current = order[i];
                color += colors[i];
            }
            instance[current].perm = head;
            instance[current].ori = (FirstCycle.ori + 24 - color) % _Ori;
            foreach (var cycle in OtherCycles)
            {
                color = 0;
                remain = cycle.perm;
                current = head = order[++i];
                while ((--remain) > 0)
                {
                    i++;
                    instance[current].perm = order[i];
                    instance[current].ori = colors[i];
                    current = order[i];
                    color += colors[i];
                }
                instance[current].perm = head;
                instance[current].ori = (cycle.ori + 24 - color) % _Ori;
            }
            return new Edge { state = instance };
        }

#region
        public static readonly List<EdgeCC> OddList, EvenList;
        public static IEnumerable<EdgeCC> AllList => OddList.Concat(EvenList);
        private static void GenerateOri(int index, int[] sizes, int[] colors, List<int> indexes)
        {
            if (index == sizes.Length)
            {
                State[] OtherCycles = new State[sizes.Length];
                for (int i = 0; i < index; i++)
                    OtherCycles[i] = (sizes[i], colors[i]);
                var t = new EdgeCC(12 - sizes.Sum(), OtherCycles);
                (t.Parity == 1 ? OddList : EvenList).Add(t);
            }
            else
            {
                int j = indexes.IndexOf(index);
                for (int i = j >= 0 ? 1 : colors[index - 1]; i >= 0; i--)
                {
                    colors[index] = i;
                    GenerateOri(index + 1, sizes, colors, indexes);
                }
            }
        }
        static EdgeCC()
        {
            OddList = [];
            EvenList = [];
            if (File.Exists("Cache/oe.txt") && File.Exists("Cache/ee.txt"))
            {
                foreach (var s in File.ReadAllLines("Cache/oe.txt"))
                {
                    var t = s.Split(',').Select(int.Parse).ToArray();
                    State[] OtherCycles = new State[(t.Length - 2) / 2];
                    for (int i = 0; i < OtherCycles.Length; i++)
                        OtherCycles[i] = (t[i * 2 + 2], t[i * 2 + 3]);
                    OddList.Add(new EdgeCC(t[0], OtherCycles));
                }
                foreach (var s in File.ReadAllLines("Cache/ee.txt"))
                {
                    var t = s.Split(',').Select(int.Parse).ToArray();
                    State[] OtherCycles = new State[(t.Length - 2) / 2];
                    for (int i = 0; i < OtherCycles.Length; i++)
                        OtherCycles[i] = (t[i * 2 + 2], t[i * 2 + 3]);
                    EvenList.Add(new EdgeCC(t[0], OtherCycles));
                }
            }
            else{
                List<int> temp = [];
                foreach (var s in SizeGenerater(12))
                {
                    int p = 0;
                    for (int i = 0; i < s.Length; i++)
                    {
                        if (s[i] != p)
                        {
                            p = s[i];
                            temp.Add(i);
                        }
                    }
                    GenerateOri(0, s, new int[s.Length], temp);
                    temp.Clear();
                }
                Directory.CreateDirectory("Cache");
                File.WriteAllLines("Cache/oe.txt",
                    OddList.Select(x => string.Join(",", Enumerable.Concat(
                        [x.FirstCycle.perm, x.FirstCycle.ori],
                        x.OtherCycles.SelectMany(y => new int[] { y.perm, y.ori })))
                ));
                File.WriteAllLines("Cache/ee.txt",
                    EvenList.Select(x => string.Join(",", Enumerable.Concat(
                        [x.FirstCycle.perm, x.FirstCycle.ori],
                        x.OtherCycles.SelectMany(y => new int[] { y.perm, y.ori })))
                ));
            }
        }
#endregion

    }
}