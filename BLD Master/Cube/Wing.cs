using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using static Cube.Move;
using static Cube.Tools;

namespace Cube
{
    public class Wing
    {
        private int[] value = new int[24];
        public const string Code = "ABCDEFGHIJKLMNOPQRSTWXYZ";
        public static readonly char[] code = Code.ToCharArray(); //for custom code
        public static int Buffer = 0;
        public static readonly List<WingCC> list;
        public static readonly BigInteger Sum;
        public Wing() => Solve();
        public Wing(int[] wings) => value = wings;
        public Wing Clone() => new Wing(value.Clone() as int[]);
        public void Solve() => value.Init(i => i);
        public void Copy(Wing other) => value = other.value.Clone() as int[];
        public int this[int index] => value[index];
        public void Cycle(int p0, int p1, int p2, int p3)
        {
            int t = value[p0];
            value[p0] = value[p3];
            value[p3] = value[p2];
            value[p2] = value[p1];
            value[p1] = t;
        }
        public void Swap(int p0, int p1)
        {
            int t = value[p0];
            value[p0] = value[p1];
            value[p1] = t;
        }
        public static int[] Random()
        {
            int[] arr = new int[24];
            for (int i = 22; i >= 0; i--)
            {
                arr[i] = Tools.rd.Next(24 - i);
                for (int j = i + 1; j < 24; j++)
                {
                    if (arr[j] >= arr[i])
                        arr[j]++;
                }
            }
            return arr;
        }
        public bool Cycle(string s)
        {
            bool parity = false;
            int j;
            for (int i = 0; i < s.Length; i++)
            {
                j = Array.IndexOf(code, s[i]);
                if (j < 0) continue;
                parity = !parity;
                Swap(Buffer, j);
            }
            return parity;
        }
        public void Turn(Move m)
        {
            switch (m)
            {
                case U: Cycle(0, 2, 4, 6); Cycle(1, 3, 5, 7); break;
                case U2: Swap(0, 4); Swap(2, 6); Swap(1, 5); Swap(3, 7); break;
                case U_: Cycle(0, 6, 4, 2); Cycle(1, 7, 5, 3); break;
                case u: Cycle(16, 19, 20, 23); break;
                case u2: Swap(16, 20); Swap(19, 23); break;
                case u_: Cycle(16, 23, 20, 19); break;
                case D: Cycle(8, 14, 12, 10); Cycle(9, 15, 13, 11); break;
                case D2: Swap(8, 12); Swap(10, 14); Swap(9, 13); Swap(15, 11); break;
                case D_: Cycle(8, 10, 12, 14); Cycle(9, 11, 13, 15); break;
                case d: Cycle(17, 22, 21, 18); break;
                case d2: Swap(17, 21); Swap(18, 22); break;
                case d_: Cycle(17, 18, 21, 22); break;
                case R: Cycle(6, 22, 14, 16); Cycle(7, 23, 15, 17); break;
                case R2: Swap(6, 14); Swap(22, 16); Swap(7, 15); Swap(23, 17); break;
                case R_: Cycle(6, 16, 14, 22); Cycle(7, 17, 15, 23); break;
                case r: Cycle(0, 5, 12, 9); break;
                case r2: Swap(0, 12); Swap(5, 9); break;
                case r_: Cycle(0, 9, 12, 5); break;
                case L: Cycle(2, 18, 10, 20); Cycle(3, 19, 11, 21); break;
                case L2: Swap(2, 10); Swap(18, 20); Swap(3, 11); Swap(19, 21); break;
                case L_: Cycle(2, 20, 10, 18); Cycle(3, 21, 11, 19); break;
                case l: Cycle(1, 8, 13, 4); break;
                case l2: Swap(1, 13); Swap(8, 4); break;
                case l_: Cycle(1, 4, 13, 8); break;
                case F: Cycle(0, 17, 8, 19); Cycle(1, 16, 9, 18); break;
                case F2: Swap(0, 8); Swap(17, 19); Swap(1, 9); Swap(16, 18); break;
                case F_: Cycle(0, 19, 8, 17); Cycle(1, 18, 9, 16); break;
                case f: Cycle(2, 7, 14, 11); break;
                case f2: Swap(2, 14); Swap(7, 11); break;
                case f_: Cycle(2, 11, 14, 7); break;
                case B: Cycle(4, 21, 12, 23); Cycle(5, 20, 13, 22); break;
                case B2: Swap(4, 12); Swap(21, 23); Swap(5, 13); Swap(20, 22); break;
                case B_: Cycle(4, 23, 12, 21); Cycle(5, 22, 13, 20); break;
                case b: Cycle(3, 10, 15, 6); break;
                case b2: Swap(3, 15); Swap(10, 6); break;
                case b_: Cycle(3, 6, 15, 10); break;
                case x: Turn(R); Turn(r); Turn(l_); Turn(L_); break;
                case x2: Turn(R2); Turn(r2); Turn(l2); Turn(L2); break;
                case x_: Turn(R_); Turn(r_); Turn(l); Turn(L); break;
                case y: Turn(U); Turn(u); Turn(d_); Turn(D_); break;
                case y2: Turn(U2); Turn(u2); Turn(d2); Turn(D2); break;
                case y_: Turn(U_); Turn(u_); Turn(d); Turn(D); break;
                case z: Turn(F); Turn(f); Turn(b_); Turn(B_); break;
                case z2: Turn(F2); Turn(f2); Turn(b2); Turn(B2); break;
                case z_: Turn(F_); Turn(f_); Turn(b); Turn(B); break;
                default: break;
            }
        }
        public string ReadCode()
        {
            StringBuilder code = new StringBuilder();
            int exam = 0xffffff;
            exam ^= 1 << Buffer;
            int next = this[Buffer], borrow;
            while (next != Buffer)
            {
                code.Append(Wing.code[next]);
                exam ^= 1 << next;
                next = this[next];
            }
            while (exam != 0)
            {
                borrow = 0;
                while ((exam & (1 << borrow)) == 0) borrow++;
                exam ^= 1 << borrow;
                next = this[borrow];
                if (next != borrow)
                {
                    code.Append(Wing.code[borrow]);
                    while (next != borrow)
                    {
                        code.Append(Wing.code[next]);
                        exam ^= 1 << next;
                        next = this[next];
                    }
                    code.Append(Wing.code[next]);
                }
            }
            return code.ToString();
        }
        public static string Stat(Func<WingCC, int> func)
        {
            BigInteger[] counts = new BigInteger[36];
            foreach (var w in list)
                counts[func(w)] += w.Count;
            StringBuilder sb = new StringBuilder("数量\t概率\n");
            double sum = 0, squareSum = 0, p;
            for (int i = 0; i < 36; i++)
                if (counts[i] != 0)
                {
                    p = (double)counts[i] / (double)Sum;
                    sum += i * p;
                    squareSum += i * i * p;
                    sb.Append(i).Append('\t').Append(p).Append('\n');
                }
            sb.Append("均值\t").Append(sum).Append('\n');
            sb.Append("标准差\t").Append(Math.Sqrt(squareSum - sum * sum)).Append('\n');
            return sb.ToString();
        }
        public override string ToString()
        {
            char[] cs = new char[24];
            for (int i = 0; i < 24; i++)
                cs[i] = code[this[i]];
            return new string(cs);
        }
        static Wing()
        {
            Sum = BigInteger.One;
            for (int i = 2; i <= 24; i++) Sum *= i;
            list = new List<WingCC>();
            foreach (int[] s in SizeGenerater(24))
                list.Add(new WingCC(24 - s.Sum(), s.Clone() as int[]));
        }
        public class WingCC
        {
            public readonly int MainCycle;
            public readonly int[] OtherCycles;
            public const int Perm = 24;
            public readonly int CodeLength, OtherCycleAmount, Parity;
            public readonly BigInteger Count;
            private static BigInteger Fact(int x) => x > 0 ? x * Fact(x - 1) : BigInteger.One;
            public WingCC(int MainCycle, int[] OtherCycles)
            {
                this.MainCycle = MainCycle;
                this.OtherCycles = OtherCycles;
                CodeLength = OtherCycles.Sum(x => x > 1 ? x + 1 : 0) + MainCycle - 1;
                Parity = CodeLength & 1;
                OtherCycleAmount = OtherCycles.Count(x => x != 1);
                Count = Fact(Perm - 1);
                foreach (var i in OtherCycles)
                    Count /= i;
                foreach (var i in OtherCycles.GroupBy(x => x))
                    Count /= Fact(i.Count());
            }
            public int[] GetInstance()
            {
                int[] instance = new int[Perm], order = Wing.Random();
                int head, remain = MainCycle, current, i = 0;
                current = head = Buffer;
                order[Array.IndexOf(order, head)] = order[0];
                order[0] = head;
                while ((--remain) > 0)
                {
                    i++;
                    instance[current] = order[i];
                    current = order[i];
                }
                instance[current] = head;
                foreach (var cycle in OtherCycles)
                {
                    remain = cycle;
                    current = head = order[++i];
                    while ((--remain) > 0)
                    {
                        i++;
                        instance[current] = order[i];
                        current = order[i];
                    }
                    instance[current] = head;
                }
                return instance;
            }
        }
        public class Limit
        {
            public Range CodeLength = new Range(0, 34), OtherCycleCount = new Range(0, 11),
                MainCycleSize = new Range(1, 24);
            public OtherCycleLimit OtherCycle = new OtherCycleLimit();
            public Option Parity;
            public struct OtherCycleLimit
            {
                public int Amount, Length;
                public bool Test(int[] OtherCycles)
                {
                    int i = Length;
                    return Amount <= OtherCycles.Count(x => x == i);
                }
            }
            public bool Test(WingCC type) => CodeLength.Test(type.CodeLength) &&
                        OtherCycleCount.Test(type.OtherCycleAmount) &&
                        MainCycleSize.Test(type.MainCycle) &&
                        OtherCycle.Test(type.OtherCycles) &&
                Parity.Test(type.Parity == 1);
        }
    }
}
