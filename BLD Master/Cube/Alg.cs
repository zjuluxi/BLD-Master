using System.Collections;
using System.Collections.Generic;

namespace Cube
{
    public enum Move
    {
        U, U2, U_, u, u2, u_, Uw, Uw2, Uw_, _3Uw, _3Uw2, _3Uw_, _4Uw, _4Uw2, _4Uw_, E, E2, E_,
        D, D2, D_, d, d2, d_, Dw, Dw2, Dw_, _3Dw, _3Dw2, _3Dw_, _4Dw, _4Dw2, _4Dw_, y, y2, y_,
        R, R2, R_, r, r2, r_, Rw, Rw2, Rw_, _3Rw, _3Rw2, _3Rw_, _4Rw, _4Rw2, _4Rw_, M, M2, M_,
        L, L2, L_, l, l2, l_, Lw, Lw2, Lw_, _3Lw, _3Lw2, _3Lw_, _4Lw, _4Lw2, _4Lw_, x, x2, x_,
        F, F2, F_, f, f2, f_, Fw, Fw2, Fw_, _3Fw, _3Fw2, _3Fw_, _4Fw, _4Fw2, _4Fw_, S, S2, S_,
        B, B2, B_, b, b2, b_, Bw, Bw2, Bw_, _3Bw, _3Bw2, _3Bw_, _4Bw, _4Bw2, _4Bw_, z, z2, z_,
    }
    public class Alg : List<Move>
    {
        public Alg() { }
        public Alg(IEnumerable<Move> moves) : base(moves) { }
        public Alg(string s)
        {
            Queue<int> q3 = new Queue<int>(), q4 = new Queue<int>();
            foreach (char c in s)
                switch (c)
                {
                    case 'U': Add(Move.U); break;
                    case 'u': Add(Move.u); break;
                    case 'D': Add(Move.D); break;
                    case 'd': Add(Move.d); break;
                    case 'R': Add(Move.R); break;
                    case 'r': Add(Move.r); break;
                    case 'L': Add(Move.L); break;
                    case 'l': Add(Move.l); break;
                    case 'F': Add(Move.F); break;
                    case 'f': Add(Move.f); break;
                    case 'B': Add(Move.B); break;
                    case 'b': Add(Move.b); break;
                    case 'E': Add(Move.E); break;
                    case 'y': Add(Move.y); break;
                    case 'M': Add(Move.M); break;
                    case 'x': Add(Move.x); break;
                    case 'S': Add(Move.S); break;
                    case 'z': Add(Move.z); break;
                    case '3': q3.Enqueue(Count); break;
                    case '4': q4.Enqueue(Count); break;
                    default:
                        if (Count == 0) break;
                        int i = (int)this[Count - 1];
                        if (c == '2') this[Count - 1] = (Move)(i / 3 * 3 + 1);
                        else if (c == '\'' && i % 3 == 0) this[Count - 1] = (Move)(i + 2);
                        else if (c == 'w' && i % 18 == 0) this[Count - 1] = (Move)(i + 6);
                        break;
                }
            foreach (var i in q3)
                if (i < Count && ((int)this[i]) % 18 / 3 == 2)
                    this[i] = (Move)((int)this[i] + 3);
            foreach (var i in q4)
                if (i < Count && ((int)this[i]) % 18 / 3 == 2)
                    this[i] = (Move)((int)this[i] + 6);
        }
        public new void Reverse()
        {
            base.Reverse();
            for (int i = Count - 1; i >= 0; i--)
            {
                int j = (int)this[i];
                this[i] = (Move)(j / 3 * 6 + 2 - j);
            }
        }
        public IEnumerable<Move> Inverse()
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                int j = (int)this[i];
                yield return (Move)(j / 3 * 6 + 2 - j);
            }
        }
        public override string ToString()
        {
            string[] strings = new string[Count];
            for (int i = 0; i < Count; i++)
                strings[i] = this[i].ToString().TrimStart('_').Replace('_', '\'');
            return string.Join(" ", strings);
        }
    }
    public class Commutator : IEnumerable<Move>
    {
        public Alg A, B, setup;
        public Commutator(Alg A, Alg B, Alg setup = null)
        {
            this.A = A;
            this.B = B;
            this.setup = setup;
        }
        public static implicit operator Alg(Commutator c)
        {
            Alg a = new Alg();
            foreach (var m in c)
                a.Add(m);
            return a;
        }
        IEnumerator<Move> IEnumerable<Move>.GetEnumerator()
        {
            if (setup != null)
                foreach (var m in setup) 
                    yield return m;
            foreach (var m in A) yield return m;
            foreach (var m in B) yield return m;
            foreach (var m in A.Inverse()) yield return m;
            foreach (var m in B.Inverse()) yield return m;
            if (setup != null)
                foreach (var m in setup) 
                    yield return m;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
        public void Reverse()
        {
            Alg t = A; A = B; B = t;
        }
        public IEnumerable<Move> Inverse()
        {
            if (setup != null)
                foreach (var m in setup)
                    yield return m;
            foreach (var m in B) yield return m;
            foreach (var m in A) yield return m;
            foreach (var m in B.Inverse()) yield return m;
            foreach (var m in A.Inverse()) yield return m;
            if (setup != null)
                foreach (var m in setup)
                    yield return m;
        }
        public override string ToString()
        {
            if (setup == null)
                return $"[{A}, {B}]";
            else
                return $"{setup} :[{A}, {B}]";
        }
    }
}
