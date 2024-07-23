using static Luxi.Tools;

namespace Luxi
{
    public class CornerCC {
        public readonly State FirstCycle;
        public readonly State[] OtherCycles;
        private const int _Perm = 8, _Ori = 3;
        public const long Sum = 88179840;
        public readonly int CodeLength, OtherCycleCount, TwistCount, Parity;
        public readonly int Count;
        public CornerCC(int FirstCycle, State[] OtherCycles)
        {
            this.FirstCycle = (FirstCycle, (_Perm * _Ori - OtherCycles.Sum(x => x.ori)) % _Ori);
            this.OtherCycles = OtherCycles;
            CodeLength = OtherCycles.Sum(x => x.perm > 1 ? x.perm + 1 : x.ori > 0 ? 2 : 0) + FirstCycle - 1;
            OtherCycleCount = OtherCycles.Count(x => x.perm != 1);
            Parity = CodeLength & 1;
            TwistCount = OtherCycles.Count(x => x.perm == 1 && x.ori > 0);
            Count = FactI[_Perm - 1];
            foreach (var i in OtherCycles)
                Count /= i.perm;
            Count *= Pow3[_Perm - 1 - OtherCycles.Length];
            foreach (var i in OtherCycles.GroupBy(x => x))
                Count /= FactI[i.Count()];
        }
        public Corner GetInstance(int Buffer=9)
        {
            State[] instance = new State[_Perm];
            int head, remain = FirstCycle.perm, current, i = 0, color = 0;
            SetNPerm(rd.Next(40320), 8, out int[] order);
            SetNTwist(rd.Next(2187), 8, out int[] colors);
            current = head = Buffer / 3;
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
            return new Corner { state = instance };
        }
        

#region 
        public static readonly List<CornerCC> OddList, EvenList, AllList;
        private static void GenerateOri(int index, int[] sizes, int[] colors, List<int> indexes)
        {
            if (index == sizes.Length)
            {
                State[] OtherCycles = new State[sizes.Length];
                for (int i = 0; i < index; i++)
                    OtherCycles[i] = (sizes[i], colors[i]);
                var t = new CornerCC(8 - sizes.Sum(), OtherCycles);
                (t.Parity == 1 ? OddList : EvenList).Add(t);
            }
            else
            {
                int j = indexes.IndexOf(index);
                for (int i = j >= 0 ? 2 : colors[index - 1]; i >= 0; i--)
                {
                    colors[index] = i;
                    GenerateOri(index + 1, sizes, colors, indexes);
                }
            }
        }
        static CornerCC()
        {
            OddList = [];
            EvenList = [];
            if (File.Exists("Cache/oc.txt") && File.Exists("Cache/ec.txt")){
                foreach (var s in File.ReadAllLines("Cache/oc.txt"))
                {
                    var t = s.Split(',').Select(int.Parse).ToArray();
                    State[] OtherCycles = new State[(t.Length - 2) / 2];
                    for (int i = 0; i < OtherCycles.Length; i++)
                        OtherCycles[i] = (t[i * 2 + 2], t[i * 2 + 3]);
                    OddList.Add(new CornerCC(t[0], OtherCycles));
                }
                foreach (var s in File.ReadAllLines("Cache/ec.txt"))
                {
                    var t = s.Split(',').Select(int.Parse).ToArray();
                    State[] OtherCycles = new State[(t.Length - 2) / 2];
                    for (int i = 0; i < OtherCycles.Length; i++)
                        OtherCycles[i] = (t[i * 2 + 2], t[i * 2 + 3]);
                    EvenList.Add(new CornerCC(t[0], OtherCycles));
                }
            }
            else{
                List<int> temp = [];
                foreach (var s in SizeGenerater(8))
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
                File.WriteAllLines("Cache/oc.txt",
                    OddList.Select(x => string.Join(",", Enumerable.Concat(
                        [x.FirstCycle.perm, x.FirstCycle.ori],
                        x.OtherCycles.SelectMany(y => new int[] { y.perm, y.ori })))
                ));
                File.WriteAllLines("Cache/ec.txt",
                    EvenList.Select(x => string.Join(",", Enumerable.Concat(
                        [x.FirstCycle.perm, x.FirstCycle.ori],
                        x.OtherCycles.SelectMany(y => new int[] { y.perm, y.ori })))
                ));
            }
            AllList = OddList.Concat(EvenList).ToList();
        }
#endregion

    }
}