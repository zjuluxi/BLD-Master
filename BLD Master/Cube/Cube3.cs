using System.Collections.Generic;
using System.Text;
using CS.Min2Phase;
using static Cube.Move;

namespace Cube
{
    public class Cube3
    {
        public Edge edge;
        public Corner corner;
        public int center;
        private readonly static string std = "dEgC0GaAjkHiR1ZzPsbBlS2QnJywIxK3OoMreDcX4TqLmhFfY5WtNp";
        public readonly static string faces = "urfdlb";
        private readonly static Search search = new Search();
        private static readonly Alg[] choice1 = { new Alg(), new Alg{Rw},
            new Alg{Rw2}, new Alg{Rw_}, new Alg{Fw}, new Alg{Fw_} },
            choice2 = { new Alg(), new Alg { Uw }, new Alg { Uw2 }, new Alg { Uw_ } },
            suffix = { new Alg(), new Alg{D_}, new Alg{D2}, new Alg{D},
                    new Alg{L_}, new Alg{B_, L_}, new Alg{B2, L_}, new Alg{B, L_},
                    new Alg{L2}, new Alg{U_, L2}, new Alg{U2, L2}, new Alg{U, L2},
                    new Alg{L}, new Alg{F_, L}, new Alg{F2, L}, new Alg{F, L},
                    new Alg{B_}, new Alg{R_, B_}, new Alg{R2, B_}, new Alg{R, B_},
                    new Alg{B}, new Alg{L_, B}, new Alg{L2, B}, new Alg{L, B}
            };
        public Cube3()
        {
            center = 0;
            edge = new Edge();
            corner = new Corner();
        }
        public Cube3(int center)
        {
            this.center = center;
            edge = new Edge(center);
            corner = new Corner(center);
        }
        public Cube3(int[] edge, int[] corner, int center)
        {
            this.center = center;
            this.edge = new Edge(edge, center);
            this.corner = new Corner(corner, center);
        }
        public static Cube3 RandomCube3(bool edge=true, bool corner=true, int center=0)
        {
            int epVal = edge ? Tools.rd.Next(479001600) : 0, eoVal = edge ? Tools.rd.Next(2048) : 0,
                    cpVal = corner ? Tools.rd.Next(40320) : 0, coVal = corner ? Tools.rd.Next(2187) : 0;
            Tools.SetNPerm(epVal, 12, out int[] ep);
            Tools.SetNFlip(eoVal, 12, out int[] eo);
            Tools.SetNPerm(cpVal, 8, out int[] cp);
            Tools.SetNTwist(coVal, 8, out int[] co);
            if (Tools.GetNParity(cpVal, 8) ^ Tools.GetNParity(epVal, 12))
            {
                if (!edge) { int t = cp[0]; cp[0] = cp[1]; cp[1] = t; }
                else { int t = ep[0]; ep[0] = ep[1]; ep[1] = t; }
            }
            int[] newEdge = new int[12], newCorner = new int[8];
            for (int i = 0; i < 12; i++)
                newEdge[i] = ep[i] * 2 + eo[i];
            for (int i = 0; i < 8; i++)
                newCorner[i] = cp[i] * 3 + co[i];
            return new Cube3(newEdge, newCorner, center);
        }
        private char CharAt(char index)
        {
            if (index >= 'A' && index <= 'Z')
                return Edge.Code[edge[Edge.Code.IndexOf(index)]];
            else if (index >= 'a' && index <= 'z')
                return Corner.Code[corner[Corner.Code.IndexOf(index)]];
            else if (index >= '0' && index < '6')
                return (char)('0' + Center.FaceAt(center, index - '0'));
            else
                return index;
        }
        public void Turn(IEnumerable<Move> s)
        {
            foreach (Move m in s) Turn(m);
        }
        public void Turn(Move m)
        {
            switch (m)
            {
                case U: edge.Turn(U); corner.Turn(U); break;
                case U2: edge.Turn(U2); corner.Turn(U2); break;
                case U_: edge.Turn(U_); corner.Turn(U_); break;
                case D: edge.Turn(D); corner.Turn(D); break;
                case D2: edge.Turn(D2); corner.Turn(D2); break;
                case D_: edge.Turn(D_); corner.Turn(D_); break;
                case R: edge.Turn(R); corner.Turn(R); break;
                case R2: edge.Turn(R2); corner.Turn(R2); break;
                case R_: edge.Turn(R_); corner.Turn(R_); break;
                case L: edge.Turn(L); corner.Turn(L); break;
                case L2: edge.Turn(L2); corner.Turn(L2); break;
                case L_: edge.Turn(L_); corner.Turn(L_); break;
                case F: edge.Turn(F); corner.Turn(F); break;
                case F2: edge.Turn(F2); corner.Turn(F2); break;
                case F_: edge.Turn(F_); corner.Turn(F_); break;
                case B: edge.Turn(B); corner.Turn(B); break;
                case B2: edge.Turn(B2); corner.Turn(B2); break;
                case B_: edge.Turn(B_); corner.Turn(B_); break;
                case E: edge.Turn(E); center = Center.Turn(center, 0, 1); break;
                case E2: edge.Turn(E2); center = Center.Turn(center, 0, 2); break;
                case E_: edge.Turn(E_); center = Center.Turn(center, 0, 3); break;
                case M: edge.Turn(M); center = Center.Turn(center, 1, 1); break;
                case M2: edge.Turn(M2); center = Center.Turn(center, 1, 2); break;
                case M_: edge.Turn(M_); center = Center.Turn(center, 1, 3); break;
                case S: edge.Turn(S); center = Center.Turn(center, 2, 1); break;
                case S2: edge.Turn(S2); center = Center.Turn(center, 2, 2); break;
                case S_: edge.Turn(S_); center = Center.Turn(center, 2, 3); break;
                case Uw: case u: Turn(U); Turn(E_); break;
                case Uw2: case u2: Turn(U2); Turn(E2); break;
                case Uw_: case u_: Turn(U_); Turn(E); break;
                case Dw: case d: Turn(D); Turn(E); break;
                case Dw2: case d2: Turn(D2); Turn(E2); break;
                case Dw_: case d_: Turn(D_); Turn(E_); break;
                case Fw: case f: Turn(F); Turn(S); break;
                case Fw2: case f2: Turn(F2); Turn(S2); break;
                case Fw_: case f_: Turn(F_); Turn(S_); break;
                case Bw: case b: Turn(B); Turn(S_); break;
                case Bw2: case b2: Turn(B2); Turn(S2); break;
                case Bw_: case b_: Turn(B_); Turn(S); break;
                case Rw: case r: Turn(R); Turn(M_); break;
                case Rw2: case r2: Turn(R2); Turn(M2); break;
                case Rw_: case r_: Turn(R_); Turn(M); break;
                case Lw: case l: Turn(L); Turn(M); break;
                case Lw2: case l2: Turn(L2); Turn(M2); break;
                case Lw_: case l_: Turn(L_); Turn(M_); break;
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
        public bool Cycle(string code)
        {
            return edge.Cycle(code) ^ corner.Cycle(code);
        }
        public void Copy(Cube3 cube)
        {
            edge.Copy(cube.edge);
            corner.Copy(cube.corner);
            center = cube.center;
        }
        public void Solve()
        {
            center = 0;
            edge.Solve(0);
            corner.Solve(0);
        }
        public void SetCoordinate(int index)
        {
            if (center != index)
            {
                edge.MapBy(Edge.std[Center.Inverse(center)]);
                edge.MapBy(Edge.std[index]);
                corner.MapBy(Corner.std[Center.Inverse(center)]);
                corner.MapBy(Corner.std[index]);
                center = index;
            }
        }
        public string ReadCode() => edge.ReadCode() + corner.ReadCode();
        public bool IsSolved() => edge.IsSolved() && corner.IsSolved();
        public static char GetColor(char c) => faces[std.IndexOf(c) / 9];
        public string GetScramble() => search.Solution(ToString(), 21, 100000000, 0, 2);
        public string GetScramble(int index)
        {
            Cube3 t = new Cube3();
            t.Copy(this);
            t.Turn(suffix[index]);
            Alg s = new Alg(t.GetScramble());
            s.AddRange(choice1[index / 4]);
            s.AddRange(choice2[index % 4]);
            return s.ToString();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (char i in std)
                sb.Append(GetColor(CharAt(i)));
            return sb.ToString();
        }
    }
}