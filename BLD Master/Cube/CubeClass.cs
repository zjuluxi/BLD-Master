using System;
using System.Numerics;
using static Cube.Tools;

namespace Cube
{
    interface ICubeClass
    {
        void Reset();
        string Stat();
        bool Exist();
    }
    class Cube3Class : ICubeClass
    {
        public Option Parity = Option.Any;
        public Edge.Limit edgeLimit = new Edge.Limit();
        public Corner.Limit cornerLimit = new Corner.Limit();
        private readonly double[] EEPs = new double[Edge.even.Count], OEPs = new double[Edge.odd.Count],
            ECPs = new double[Corner.even.Count], OCPs = new double[Corner.odd.Count];
        private double EEP, OEP, ECP, OCP, probability, flag;
        private long EEC, OEC, ECC, OCC;
        public void Reset()
        {
            int index = 0;
            EEC = 0;
            foreach (var x in Edge.even)
                EEPs[index++] = (EEC += Parity.Test(false) && edgeLimit.Test(x) ? x.Count : 0) * 2.0 / Edge.Sum;
            EEP = EEPs[EEPs.Length - 1];

            OEC = index = 0;
            foreach (var x in Edge.odd)
                OEPs[index++] = (OEC += Parity.Test(true) && edgeLimit.Test(x) ? x.Count : 0) * 2.0 / Edge.Sum;
            OEP = OEPs[OEPs.Length - 1];

            ECC = index = 0;
            foreach (var x in Corner.even)
                ECPs[index++] = (ECC += Parity.Test(false) && cornerLimit.Test(x) ? x.Count : 0) * 2.0 / Corner.Sum;
            ECP = ECPs[ECPs.Length - 1];

            OCC = index = 0;
            foreach (var x in Corner.odd)
                OCPs[index++] = (OCC += Parity.Test(true) && cornerLimit.Test(x) ? x.Count : 0) * 2.0 / Corner.Sum;
            OCP = OCPs[OCPs.Length - 1];

            double even = 2.0 * EEC / Edge.Sum * ECC / Corner.Sum,
                odd = 2.0 * OEC / Edge.Sum * OCC / Corner.Sum;
            probability = even + odd;
            flag = odd / (even + odd);
        }
        public bool Exist()
        {
            Reset();
            return EEC != 0 && ECC != 0 || OEC != 0 && OCC != 0;
        }
        public string Stat()
        {
            Reset();
            return $"概率\t{ probability }\r\n奇偶期望\t{ flag }\n";
        }
        // Must call Reset() before this in advance
        public Cube3 GetCube3(int center = 0)
        {
            if (!Exist()) return null;
            else if (rd.NextDouble() < flag)
            {
                double a = rd.NextDouble() * OEP, b = rd.NextDouble() * OCP;
                return new Cube3(Edge.odd[Array.FindIndex(OEPs, x => x > a)].GetInstance(),
                Corner.odd[Array.FindIndex(OCPs, x => x > b)].GetInstance(), center);
            }
            else
            {
                double a = rd.NextDouble() * EEP, b = rd.NextDouble() * ECP;
                return new Cube3(Edge.even[Array.FindIndex(EEPs, x => x > a)].GetInstance(),
                Corner.even[Array.FindIndex(ECPs, x => x > b)].GetInstance(), center);
            }
        }
    }
    public class Cube4Class : ICubeClass
    {
        public Range xcLimit = new Range(0, 24);
        public Corner.Limit cornerLimit = new Corner.Limit();
        public Wing.Limit wingLimit = new Wing.Limit();
        public Option cornerParity = Option.Any;
        private readonly double[] ECPs = new double[Corner.even.Count],
            OCPs = new double[Corner.odd.Count], WPs = new double[Wing.list.Count];
        private long CC, XCC;
        private BigInteger WC;
        private double CF, WF, ECP, OCP, CP, WP, XCP;

        public void Reset()
        {
            int index = 0;
            long ECC = 0, OCC = 0;
            foreach (var x in Corner.odd)
                OCPs[index++] = (OCC += cornerParity.Test(true) && cornerLimit.Test(x) ? x.Count : 0) * 2.0 / Corner.Sum;
            OCP = OCPs[OCPs.Length - 1];
            CC = index = 0;
            foreach (var x in Corner.even)
                ECPs[index++] = (ECC += cornerParity.Test(false) && cornerLimit.Test(x) ? x.Count : 0) * 2.0 / Corner.Sum;
            ECP = ECPs[ECPs.Length - 1];
            CC = OCC + ECC;
            CP = ECP + OCP;
            CF = (double)OCC / CC;

            WC = 0;
            double OWC = 0, wingSum = (double)Wing.Sum;
            for (index = 0; index < WPs.Length; ++index)
            {
                if (wingLimit.Test(Wing.list[index]))
                {
                    WPs[index] = (double)(WC += Wing.list[index].Count) / wingSum;
                    if (Wing.list[index].Parity == 1) OWC += (double)Wing.list[index].Count / wingSum;
                }
            }
            WP = WPs[WPs.Length - 1];
            WF = OWC / (double)WC;

            XCC = XCenter.GetCount(xcLimit);
            XCP = (double)XCC / XCenter.Sum;
        }
        public bool Exist()
        {
            Reset();
            return CC != 0 && !WC.Equals(BigInteger.Zero) && XCC != 0;
        }
        public string Stat()
        {
            Reset();
            return $"概率    \t{ CP * WP * XCP }\n角奇偶期望\t{ CF }\n翼棱奇偶期望\t{ WF }\n" +
                $"角概率\t{ CP }\n翼棱概率\t{ WP }\n角心概率\t{ XCP }";
        }
        // Must call Reset() before this in advance
        public Cube4 GetCube4()
        {
            if (!Exist()) return null;
            double a = rd.NextDouble(), b = rd.NextDouble();
            var c = rd.NextDouble() < CF ? Corner.odd[Array.FindIndex(OCPs, x => x > a * OCP)] :
                Corner.even[Array.FindIndex(ECPs, x => x > a * ECP)];
            var w = Wing.list[Array.FindIndex(WPs, x => x > b)];
            var xc = XCenter.GetInstance(xcLimit);
            return new Cube4(c.GetInstance(), w.GetInstance(), xc);
        }
    }
}