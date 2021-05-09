using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Cube.Move;
using static Cube.Tools;

namespace Cube
{
    public class Edge
    {
        private int[] value;
        public const string Code = "ABCDEFGHIJKLMNOPQRSTWXYZ";
        public static readonly char[] code = Code.ToCharArray(); //for custom code
        public static readonly Edge[] std = new Edge[24];
        public static int Buffer = 0;
        public static readonly List<EdgeCC> odd, even;
        public const long Sum = 980995276800;

        public Edge() => value = new int[12].Init(i => i << 1);
        public Edge(int center) => Copy(std[center]);
        public Edge(int[] value) => this.value = value.Clone() as int[];
        public Edge(int[] value, int center)
        {
            this.value = new int[12];
            for (int i = 0; i < 12; i++)
                this.value[std[center][i * 2] >> 1] = value[i] ^ (std[center][i * 2] & 1);
        }
        public static int[] Random()
        {
            int epVal = Tools.rd.Next(479001600), eoVal = Tools.rd.Next(2048);
            int[] arr = new int[12];
            Tools.SetNPerm(epVal, 12, out int[] ep);
            Tools.SetNFlip(eoVal, 12, out int[] eo);
            for (int i = 0; i < 12; i++)
                arr[i] = (ep[i] << 1) | eo[i];
            return arr;
        }
        public void Copy(Edge other) => value = other.value.Clone() as int[];
        public void Solve(int center) => Copy(std[center]);
        public bool IsSolved()
        {
            for (int i = 0; i < 12; i++)
                if (value[i] != i * 2) return false;
            return true;
        }
        public void MapBy(Edge map)
        {
            if (map == this) return;
            for (int i = 0; i < 12; i++)
                value[i] = map[value[i]];
        }
        public int this[int index] => value[index >> 1] ^ (index & 1);
        public void Cycle(int p0, int p1, int p2, int p3, int o)
        {
            int t = value[p0];
            value[p0] = value[p3] ^ o;
            value[p3] = value[p2] ^ o;
            value[p2] = value[p1] ^ o;
            value[p1] = t ^ o;
        }
        public void Swap(int p0, int p1, int o = 0)
        {
            int t = value[p0];
            value[p0] = value[p1] ^ o;
            value[p1] = t ^ o;
        }
        public bool Cycle(string s)
        {
            bool parity = false;
            int j, k = -1;
            for (int i = 0; i < s.Length; i++)
            {
                j = Array.IndexOf(code, s[i]);
                if (j < 0 || j >> 1 == Buffer >> 1)
                {
                    if (s[i] == '+' && k >= 0)
                        j = k ^ 1;
                    else
                        continue;
                }
                parity = !parity;
                k = j;
                Swap(Buffer / 2, j / 2, (Buffer ^ j) & 1);
            }
            return parity;
        }
        public void Turn(Move m)
        {
            switch (m)
            {
                case U: Cycle(0, 1, 2, 3, 0); break;
                case U2: Swap(0, 2); Swap(1, 3); break;
                case U_: Cycle(0, 3, 2, 1, 0); break;
                case D: Cycle(4, 7, 6, 5, 0); break;
                case D2: Swap(4, 6); Swap(7, 5); break;
                case D_: Cycle(4, 5, 6, 7, 0); break;
                case R: Cycle(3, 11, 7, 8, 0); break;
                case R2: Swap(3, 7); Swap(11, 8); break;
                case R_: Cycle(3, 8, 7, 11, 0); break;
                case L: Cycle(1, 9, 5, 10, 0); break;
                case L2: Swap(1, 5); Swap(9, 10); break;
                case L_: Cycle(1, 10, 5, 9, 0); break;
                case F: Cycle(0, 8, 4, 9, 1); break;
                case F2: Swap(0, 4); Swap(8, 9); break;
                case F_: Cycle(0, 9, 4, 8, 1); break;
                case B: Cycle(2, 10, 6, 11, 1); break;
                case B2: Swap(2, 6); Swap(10, 11); break;
                case B_: Cycle(2, 11, 6, 10, 1); break;
                case E: Cycle(8, 11, 10, 9, 1); break;
                case E2: Swap(8, 10); Swap(11, 9); break;
                case E_: Cycle(8, 9, 10, 11, 1); break;
                case M: Cycle(0, 4, 6, 2, 1); break;
                case M2: Swap(0, 6); Swap(4, 2); break;
                case M_: Cycle(0, 2, 6, 4, 1); break;
                case S: Cycle(1, 3, 7, 5, 1); break;
                case S2: Swap(1, 7); Swap(5, 3); break;
                case S_: Cycle(1, 5, 7, 3, 1); break;
                case x: Turn(R); Turn(L_); Turn(M_); break;
                case x2: Turn(R2); Turn(L2); Turn(M2); break;
                case x_: Turn(R_); Turn(L); Turn(M); break;
                case y: Turn(U); Turn(D_); Turn(E_); break;
                case y2: Turn(U2); Turn(D2); Turn(E2); break;
                case y_: Turn(U_); Turn(D); Turn(E); break;
                case z: Turn(F); Turn(B_); Turn(S); break;
                case z2: Turn(F2); Turn(B2); Turn(S2); break;
                case z_: Turn(F_); Turn(B); Turn(S_); break;
                default: break;
            }
        }
        private int GetRelativeEdge(int index, int center) => std[Center.Inverse(Center.Default)][
            this[std[Center.Inverse(center)][std[Center.Default][index]]]];
        public string ReadCode(int center = 0)
        {
            StringBuilder code = new StringBuilder(), flip = new StringBuilder();
            int color, exam = 0xfff;
            exam ^= 1 << (Buffer >> 1);
            int next = GetRelativeEdge(Buffer, center), borrow;
            while (next / 2 != Buffer / 2)
            {
                code.Append(Edge.code[next]);
                exam ^= 1 << (next >> 1);
                next = GetRelativeEdge(next, center);
            }
            color = (next ^ Buffer) & 1;
            while (exam != 0)
            {
                borrow = 0;
                while ((exam & (1 << borrow)) == 0) borrow++;
                exam ^= 1 << borrow;
                borrow <<= 1;
                borrow |= color;
                next = GetRelativeEdge(borrow, center);
                if (next / 2 == borrow / 2 && next != borrow)
                {
                    flip.Append(Edge.code[borrow & ~1]);
                    flip.Append('+');
                }
                else if (next != borrow)
                {
                    code.Append(Edge.code[borrow]);
                    while (next / 2 != borrow / 2)
                    {
                        code.Append(Edge.code[next]);
                        exam ^= 1 << (next >> 1);
                        color = next % 2;
                        next = GetRelativeEdge(next, center);
                    }
                    code.Append(Edge.code[next]);
                }
            }
            code.Append(flip);
            return code.ToString();
        }
        private static void Recursion(int index, int[] sizes, int[] colors, List<int> indexes)
        {
            if (index == sizes.Length)
            {
                (int, int)[] OtherCycles = new (int, int)[sizes.Length];
                for (int i = 0; i < index; i++)
                    OtherCycles[i] = (sizes[i], colors[i]);
                var t = new EdgeCC(12 - sizes.Sum(), OtherCycles);
                (t.Parity == 1 ? odd : even).Add(t);
            }
            else
            {
                int j = indexes.IndexOf(index);
                for (int i = j >= 0 ? 1 : colors[index - 1]; i >= 0; i--)
                {
                    colors[index] = i;
                    Recursion(index + 1, sizes, colors, indexes);
                }
            }
        }
        static Edge()
        {
            Move[] s1 = { x, x2, x_, z, z_ }, s2 = { y, y2, y_ };
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 4; j++)
                {
                    std[i * 4 + j] = new Edge();
                    if (i > 0) std[i * 4 + j].Turn(s1[i - 1]);
                    if (j > 0) std[i * 4 + j].Turn(s2[j - 1]);
                }
            odd = new List<EdgeCC>();
            even = new List<EdgeCC>();
            List<int> temp = new List<int>();
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
                Recursion(0, s, new int[s.Length], temp);
                temp.Clear();
            }
        }
        public static string Stat(Func<EdgeCC, int> func)
        {
            long[] counts = new long[24];
            foreach (var e in even)
                counts[func(e)] += e.Count;
            foreach (var e in odd)
                counts[func(e)] += e.Count;
            StringBuilder sb = new StringBuilder("数量\t概率\n");
            double sum = 0, squareSum = 0, p;
            for (int i = 0; i < 24; i++)
                if (counts[i] != 0)
                {
                    p = counts[i] * 1.0 / Sum;
                    sum += i * p;
                    squareSum += i * i * p;
                    sb.Append(i).Append('\t').Append(p).Append('\n');
                }
            sb.Append("均值\t").Append(sum).Append('\n');
            sb.Append("标准差\t").Append(Math.Sqrt(squareSum - sum * sum)).Append('\n');
            return sb.ToString();
        }
        public class EdgeCC
        {
            public readonly (int, int) MainCycle;
            public readonly (int, int)[] OtherCycles;
            public const int Perm = 12, Ori = 2;
            public readonly int CodeLength, OtherCycleAmount, FlipAmount, Parity;
            public readonly long Count;
            public EdgeCC(int MainCycle, (int, int)[] OtherCycles)
            {
                this.MainCycle = (MainCycle, (Perm * Ori - OtherCycles.Sum(x => x.Item2)) % Ori);
                this.OtherCycles = OtherCycles;
                CodeLength = OtherCycles.Sum(x => x.Item1 > 1 ? x.Item1 + 1 : x.Item2 * 2) + MainCycle - 1;
                OtherCycleAmount = OtherCycles.Count(x => x.Item1 != 1);
                Parity = CodeLength & 1;
                FlipAmount = OtherCycles.Count(x => x.Item1 == 1 && x.Item2 > 0);
                Count = FactL(Perm - 1);
                foreach (var i in OtherCycles)
                    Count /= i.Item1;
                Count *= PowL(Ori, Perm - 1 - OtherCycles.Length);
                foreach (var i in OtherCycles.GroupBy(x => x))
                    Count /= FactL(i.Count());
            }
            public int[] GetInstance()
            {
                int[] instance = new int[Perm];
                int head, remain = MainCycle.Item1, current, i = 0, color = 0;
                SetNPerm(rd.Next(479001600), 12, out int[] order);
                SetNFlip(rd.Next(2048), 12, out int[] colors);
                current = head = Buffer >> 1;
                order[Array.IndexOf(order, head)] = order[0];
                order[0] = head;
                while ((--remain) > 0)
                {
                    i++;
                    instance[current] = order[i] * Ori + colors[i];
                    current = order[i];
                    color += colors[i];
                }
                instance[current] = head * Ori + (MainCycle.Item2 + 24 - color) % Ori;
                foreach (var cycle in OtherCycles)
                {
                    color = 0;
                    remain = cycle.Item1;
                    current = head = order[++i];
                    while ((--remain) > 0)
                    {
                        i++;
                        instance[current] = order[i] * Ori + colors[i];
                        current = order[i];
                        color += colors[i];
                    }
                    instance[current] = head * Ori + (cycle.Item2 + 24 - color) % Ori;
                }
                return instance;
            }
        }
        public class Limit
        {
            public Range CodeLength = new Range(0, 16), OtherCycleCount = new Range(0, 5),
                FlipOrTwistAmount = new Range(0, 11), MainCycleSize = new Range(1, 12);
            public Option MainCycleO = Option.Any;
            public OtherCycleLimit OtherCycle = new OtherCycleLimit();
            public struct OtherCycleLimit
            {
                public int Amount, Length;
                public Option ColorOpen;
                public bool Test((int, int)[] OtherCycles)
                {
                    int i = Length; Option o = ColorOpen;
                    return Amount <= OtherCycles.Count(x => x.Item1 == i && o.Test(x.Item2 == 0));
                }
            }
            public bool Test(EdgeCC type) => CodeLength.Test(type.CodeLength) &&
                        OtherCycleCount.Test(type.OtherCycleAmount) &&
                        FlipOrTwistAmount.Test(type.FlipAmount) &&
                        MainCycleSize.Test(type.MainCycle.Item1) &&
                        MainCycleO.Test(type.MainCycle.Item2 != 0) &&
                        OtherCycle.Test(type.OtherCycles);
        }
    }
}
