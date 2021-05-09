using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Cube.Move;
using static Cube.Tools;

namespace Cube
{
    public class Corner
    {
        private int[] value;
        public const string Code = "abcdefghijklwmnopqrstxyz";
        public static readonly char[] code = Code.ToCharArray(); //for custom code
        public static readonly Corner[] std = new Corner[24];
        public static int Buffer = 9;
        public static readonly List<CornerCC> odd, even;
        public const long Sum = 88179840;

        public Corner() => value = new int[8].Init(i => i * 3);
        public Corner(int center) => Copy(std[center]);
        public Corner(int[] value) => this.value = value.Clone() as int[];
        public Corner(int[] value, int center)
        {
            this.value = new int[8];
            for (int i = 0; i < 8; i++)
                this.value[std[center][i * 3] / 3] = value[i] / 3 * 3
                    + (value[i] - std[center][i * 3] + 24) % 3;
        }
        public static int[] Random()
        {
            int cpVal = Tools.rd.Next(40320), coVal = Tools.rd.Next(2187);
            int[] arr = new int[8];
            Tools.SetNPerm(cpVal, 8, out int[] ep);
            Tools.SetNTwist(coVal, 8, out int[] eo);
            for (int i = 0; i < 8; i++)
                arr[i] = ep[i] * 3 + eo[i];
            return arr;
        }
        public void Copy(Corner other) => value = other.value.Clone() as int[];
        public void Solve(int center) => Copy(std[center]);
        public bool IsSolved()
        {
            for (int i = 0; i < 8; i++)
                if (value[i] != i * 3) return false;
            return true;
        }
        public void MapBy(Corner map)
        {
            if (map == this) return;
            for (int i = 0; i < 8; i++)
                value[i] = map[value[i]];
        }
        public int this[int index] => value[index / 3] / 3 * 3 + (value[index / 3] + index) % 3;
        public void Cycle(int p0, int p1, int p2, int p3, int o)
        {
            int t = value[p0];
            value[p0] = value[p3] / 3 * 3 + (value[p3] + o) % 3;
            value[p3] = value[p2] / 3 * 3 + (value[p2] + 3 - o) % 3;
            value[p2] = value[p1] / 3 * 3 + (value[p1] + o) % 3;
            value[p1] = t / 3 * 3 + (t + 3 - o) % 3;
        }
        public void Swap(int p0, int p1, int o=0)
        {
            int t = value[p0];
            value[p0] = value[p1] / 3 * 3 + (value[p1] + o) % 3;
            value[p1] = t / 3 * 3 + (t + 3 - o) % 3;
        }
        public bool Cycle(string s)
        {
            bool parity = false;
            int j, k = -1;
            foreach (char c in s)
            {
                j = Array.IndexOf(code, c);
                if (j < 0 || j / 3 == Buffer / 3)
                {
                    if (c == '+' && k >= 0)
                        j = k / 3 * 3 + (k + 1) % 3;
                    else if (c == '-' && k >= 0)
                        j = k / 3 * 3 + (k + 2) % 3;
                    else
                        continue;
                }
                parity = !parity;
                k = j;
                Swap(Buffer / 3, j / 3, (j - Buffer + 24) % 3);
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
                case R: Cycle(3, 2, 6, 7, 1); break;
                case R2: Swap(3, 6); Swap(2, 7); break;
                case R_: Cycle(3, 7, 6, 2, 1); break;
                case L: Cycle(0, 4, 5, 1, 2); break;
                case L2: Swap(0, 5); Swap(4, 1); break;
                case L_: Cycle(0, 1, 5, 4, 2); break;
                case F: Cycle(0, 3, 7, 4, 1); break;
                case F2: Swap(0, 7); Swap(3, 4); break;
                case F_: Cycle(0, 4, 7, 3, 1); break;
                case B: Cycle(1, 5, 6, 2, 2); break;
                case B2: Swap(1, 6); Swap(5, 2); break;
                case B_: Cycle(1, 2, 6, 5, 2); break;
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
        private int GetRelativeCorner(int index, int center) => std[Center.Inverse(Center.Default)][
            this[std[Center.Inverse(center)][std[Center.Default][index]]]];
        public string ReadCode(int center = 0)
        {
            StringBuilder code = new StringBuilder(), twist = new StringBuilder();
            int exam = 0xff;
            exam ^= 1 << (Buffer / 3);
            int next = GetRelativeCorner(Buffer, center), borrow;
            while (next / 3 != Buffer / 3)
            {
                code.Append(Corner.code[next]);
                exam ^= 1 << (next / 3);
                next = GetRelativeCorner(next, center);
            }
            while (exam != 0)
            {
                borrow = 0;
                while ((exam & (1 << borrow)) == 0) borrow++;
                exam ^= 1 << borrow;
                borrow *= 3;
                next = GetRelativeCorner(borrow, center);
                if (next != borrow)
                    if (next / 3 == borrow / 3)
                    {
                        twist.Append(Corner.code[borrow]);
                        twist.Append(next % 3 == 2 ? '-' : '+');
                    }
                    else
                    {
                        code.Append(Corner.code[borrow]);
                        while (next / 3 != borrow / 3)
                        {
                            code.Append(Corner.code[next]);
                            exam ^= 1 << (next / 3);
                            next = GetRelativeCorner(next, center);
                        }
                        code.Append(Corner.code[next]);
                    }
            }
            code.Append(twist);
            return code.ToString();
        }
        private static void Recursion(int index, int[] sizes, int[] colors, List<int> indexes)
        {
            if (index == sizes.Length)
            {
                (int, int)[] OtherCycles = new (int, int)[sizes.Length];
                for (int i = 0; i < index; i++)
                    OtherCycles[i] = (sizes[i], colors[i]);
                var t = new CornerCC(8 - sizes.Sum(), OtherCycles);
                (t.Parity == 1 ? odd : even).Add(t);
            }
            else
            {
                int j = indexes.IndexOf(index);
                for (int i = j >= 0 ? 2 : colors[index - 1]; i >= 0; i--)
                {
                    colors[index] = i;
                    Recursion(index + 1, sizes, colors, indexes);
                }
            }
        }
        static Corner()
        {
            Move[] s1 = { x, x2, x_, z, z_ }, s2 = { y, y2, y_ };
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 4; j++)
                {
                    std[i * 4 + j] = new Corner();
                    if (i > 0) std[i * 4 + j].Turn(s1[i - 1]);
                    if (j > 0) std[i * 4 + j].Turn(s2[j - 1]);
                }
            odd = new List<CornerCC>();
            even = new List<CornerCC>();
            List<int> temp = new List<int>();
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
                Recursion(0, s, new int[s.Length], temp);
                temp.Clear();
            }
        }
        public static string Stat(Func<CornerCC, int> func)
        {
            int[] counts = new int[24];
            foreach (var c in even)
                counts[func(c)] += c.Count;
            foreach (var c in odd)
                counts[func(c)] += c.Count;
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
        public class CornerCC
        {
            public readonly (int, int) MainCycle;
            public readonly (int, int)[] OtherCycles;
            public const int Perm = 8, Ori = 3;
            public readonly int CodeLength, OtherCycleAmount, TwistAmount, Parity;
            public readonly int Count;
            public CornerCC(int MainCycle, (int, int)[] OtherCycles)
            {
                this.MainCycle = (MainCycle, (Perm * Ori - OtherCycles.Sum(x => x.Item2)) % Ori);
                this.OtherCycles = OtherCycles;
                CodeLength = OtherCycles.Sum(x => x.Item1 > 1 ? x.Item1 + 1 : x.Item2 > 0 ? 2 : 0) + MainCycle - 1;
                OtherCycleAmount = OtherCycles.Count(x => x.Item1 != 1);
                Parity = CodeLength & 1;
                TwistAmount = OtherCycles.Count(x => x.Item1 == 1 && x.Item2 > 0);
                Count = Fact(Perm - 1);
                foreach (var i in OtherCycles)
                    Count /= i.Item1;
                Count *= Pow(Ori, Perm - 1 - OtherCycles.Length);
                foreach (var i in OtherCycles.GroupBy(x => x))
                    Count /= Fact(i.Count());
            }
            public int[] GetInstance()
            {
                int[] instance = new int[Perm];
                int head, remain = MainCycle.Item1, current, i = 0, color = 0;
                SetNPerm(rd.Next(40320), 8, out int[] order);
                SetNTwist(rd.Next(2187), 8, out int[] colors);
                current = head = Buffer / 3;
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
            public Range CodeLength = new Range(0, 10), OtherCycleCount = new Range(0, 3),
                FlipOrTwistAmount = new Range(0, 7), MainCycleSize = new Range(1, 8);
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
            public bool Test(CornerCC type) => CodeLength.Test(type.CodeLength) &&
                        OtherCycleCount.Test(type.OtherCycleAmount) &&
                        FlipOrTwistAmount.Test(type.TwistAmount) &&
                        MainCycleSize.Test(type.MainCycle.Item1) &&
                        MainCycleO.Test(type.MainCycle.Item2 != 0) &&
                        OtherCycle.Test(type.OtherCycles);
        }
    }
}

