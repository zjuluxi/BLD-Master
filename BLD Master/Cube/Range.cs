using System;
using System.Text;

namespace Cube
{
    public struct Range
    {
        public int Lower, Upper;
        public Range(int lower, int upper) { Lower = lower; Upper = upper; }
        public bool Test(int x) => x >= Lower && x <= Upper;
        public static Range Parse(string s)
        {
            Range range = new Range(0, 99);
            if (s.Length > 0)
            {
                int i = s.IndexOf('-');
                if (i < 0)
                {
                    if (int.TryParse(s, out int value))
                        range.Lower = range.Upper = value;
                    else
                        throw new FormatException();
                }
                else if ((i > 0 && !int.TryParse(s.Substring(0, i), out range.Lower)) ||
                        (i < s.Length - 1 && !int.TryParse(s.Substring(i + 1), out range.Upper)))
                    throw new FormatException();
            }
            if (range.Lower < 0) range.Lower = 0;
            if (range.Upper > 99) range.Upper = 99;
            return range;
        }
        public override string ToString()
        {
            if (Lower == 0 && Upper == 0) return "";
            else
            {
                StringBuilder sb = new StringBuilder();
                if (Lower > 0) sb.Append(Lower);
                sb.Append('-');
                if (Upper < 99) sb.Append(Upper);
                return sb.ToString();
            }
        }
    }
}
