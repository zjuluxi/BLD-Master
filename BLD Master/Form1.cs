using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using BLD_Master.Properties;
using Cube;
using static Cube.Tools;

namespace BLD_Master
{
    public partial class Form1 : Form
    {
        private ICubeClass cc;
        private readonly Cube3Class cc3 = new Cube3Class();
        private readonly Cube4Class cc4 = new Cube4Class();
        private readonly RadioToolStripItems[] rs;

        private int amount = 1;
        private Thread thread;
        public Form1()
        {
            InitializeComponent();
            rs = new RadioToolStripItems[]{
                new RadioToolStripItems(ebf, new string[] {
                    "UF", "UL", "UB", "UR", "DF", "DL", "DB", "DR", "FR", "FL", "BL", "BR" }),
                new RadioToolStripItems(cbf, new string[] {
                    "UFL", "UBL", "UBR", "UFR", "DFL", "DBL", "DBR", "DFR"}),
                new RadioToolStripItems(wbf, new string[] {
                    "UFr", "FUl", "ULf", "LUb", "UBl", "BUr", "URb", "RUf", "DFl", "FDr", "DLb", "LDf",
                    "DBr", "BDl", "DRf", "RDb", "FRu", "RFd", "FLd", "LFu", "BLu", "LBd", "BRd", "RBu"}),
                new RadioToolStripItems(ori, new string[] {
                    "白顶绿前", "白顶红前", "白顶蓝前", "白顶橙前", "绿顶黄前", "绿顶红前",
                    "绿顶白前", "绿顶橙前", "黄顶蓝前", "黄顶红前", "黄顶绿前", "黄顶橙前",
                    "蓝顶白前", "蓝顶红前", "蓝顶黄前", "蓝顶橙前", "橙顶绿前", "橙顶白前",
                    "橙顶蓝前", "橙顶黄前", "红顶绿前", "红顶黄前", "红顶蓝前", "红顶白前"})
            };
            rs[0].CheckedIndex = Settings.Default.ebf / 2;
            rs[1].CheckedIndex = Settings.Default.cbf / 3;
            rs[2].CheckedIndex = Settings.Default.wbf;
            rs[3].CheckedIndex = Settings.Default.ori;
            dftOri.Checked = Settings.Default.dftOri;
            upsetCoord.Checked = Settings.Default.upsetCoord;
            signNum.Checked = Settings.Default.signNum;

            var h = new KeyPressEventHandler((sender, e) =>
                 e.Handled = !char.IsNumber(e.KeyChar) && e.KeyChar != (char)8);
            var h_ = new KeyPressEventHandler((sender, e) =>
                 e.Handled = !char.IsNumber(e.KeyChar) && e.KeyChar != (char)8 &&
                 !(e.KeyChar == '-' && (sender as TextBox).Text.IndexOf('-') < 0));
            foreach (Control c in MyControl())
            {
                if (c is ComboBox cb)
                    cb.SelectedIndex = 0;
                else if (c is TextBox tb)
                    tb.KeyPress += tb.MaxLength == 5 ? h_ : h;
            }
            cc = cc3;
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (thread != null && thread.IsAlive)
                thread.Abort();
            Settings.Default.ebf = rs[0].CheckedIndex * 2;
            Settings.Default.cbf = rs[1].CheckedIndex * 3;
            Settings.Default.wbf = rs[2].CheckedIndex;
            Settings.Default.ori = rs[3].CheckedIndex;
            Settings.Default.dftOri = dftOri.Checked;
            Settings.Default.upsetCoord = upsetCoord.Checked;
            Settings.Default.signNum = signNum.Checked;
            Settings.Default.Save();
            base.OnFormClosing(e);
        }
        private IEnumerable<Control> MyControl()
        {
            foreach (Control c in edgePanel.Controls)
                yield return c;
            foreach (Control c in cornerPanel.Controls)
                yield return c;
            foreach (Control c in wingPanel.Controls)
                yield return c;
            foreach (Control c in xcenterPanel.Controls)
                yield return c;
            yield return amountBox;
        }
        private bool UpdateData()
        {
            Edge.Buffer = rs[0].CheckedIndex * 2;
            Corner.Buffer = rs[1].CheckedIndex * 3;
            Wing.Buffer = rs[2].CheckedIndex;
            Center.Default = rs[3].CheckedIndex;
            if (amountBox.TextLength == 0)
                amountBox.Text = "1";
            amount = int.Parse(amountBox.Text);
            try
            {
                if (mode3.Checked)
                {
                    cc3.Parity = (Option)cpr.SelectedIndex;
                    ReadEdgeLimit(cc3.edgeLimit);
                    ReadCornerLimit(cc3.cornerLimit);
                }
                else
                {
                    cc4.wingLimit.Parity = (Option)wpr.SelectedIndex;
                    cc4.cornerParity = (Option)cpr.SelectedIndex;
                    ReadCornerLimit(cc4.cornerLimit);
                    ReadWingLimit(cc4.wingLimit);
                    ReadXCenterLimit(ref cc4.xcLimit);
                }
            }
            catch
            {
                MessageBox.Show("格式错误"); return false;
            }
            return true;
        }
        private void ReadEdgeLimit(Edge.Limit limit)
        {
            limit.CodeLength = Range.Parse(e_codeLength.Text);
            limit.FlipOrTwistAmount = Range.Parse(e_flip.Text);
            limit.MainCycleSize = Range.Parse(e_mainCycleLength.Text);
            limit.MainCycleO = (Option)e_mainCycleO.SelectedIndex;
            limit.OtherCycleCount = Range.Parse(e_sideCycleAmount.Text);
            if (!int.TryParse(e_ssAmount.Text, out limit.OtherCycle.Amount)) limit.OtherCycle.Amount = 0;
            if (!int.TryParse(e_ssLength.Text, out limit.OtherCycle.Length)) limit.OtherCycle.Length = 0;
            limit.OtherCycle.ColorOpen = (Option)e_ssO.SelectedIndex;
        }
        private void ReadCornerLimit(Corner.Limit limit)
        {
            limit.CodeLength = Range.Parse(c_codeLength.Text);
            limit.FlipOrTwistAmount = Range.Parse(c_twist.Text);
            limit.MainCycleSize = Range.Parse(c_mainCycleLength.Text);
            limit.MainCycleO = (Option)c_mainCycleO.SelectedIndex;
            limit.OtherCycleCount = Range.Parse(c_sideCycleAmount.Text);
            if (!int.TryParse(c_ssAmount.Text, out limit.OtherCycle.Amount)) limit.OtherCycle.Amount = 0;
            if (!int.TryParse(c_ssLength.Text, out limit.OtherCycle.Length)) limit.OtherCycle.Length = 0;
            limit.OtherCycle.ColorOpen = (Option)c_ssO.SelectedIndex;
        }
        private void ReadWingLimit(Wing.Limit limit)
        {
            limit.CodeLength = Range.Parse(w_codeLength.Text);
            limit.MainCycleSize = Range.Parse(w_mainCycleLength.Text);
            limit.OtherCycleCount = Range.Parse(w_sideCycleAmount.Text);
            if (!int.TryParse(w_ssAmount.Text, out limit.OtherCycle.Amount)) limit.OtherCycle.Amount = 0;
            if (!int.TryParse(w_ssLength.Text, out limit.OtherCycle.Length)) limit.OtherCycle.Length = 0;
        }
        private void ReadXCenterLimit(ref Range r) => r = Range.Parse(xc_codeLength.Text);

        private void Done_Click(object sender, EventArgs e)
        {
            if (!UpdateData())
                return;
            else if (done.Text[0] == '停')
            {
                done.Text = "开始";
                thread.Abort();
            }
            else if (amount == 0)
                MessageBox.Show("数量非正值");
            else if (!cc.Exist())
                MessageBox.Show("类型不存在");
            else
            {
                done.Text = "停止";
                result.Clear();
                bool signnumber = signNum.Checked, mode444 = mode4.Checked,
                    upsetcoordinate = upsetCoord.Checked;
                int center = dftOri.Checked ? Center.Default : 0;
                cc.Reset();
                (thread = new Thread(() =>
                {
                    Action<string> a = s => result.AppendText(s);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < amount; i++)
                    {
                        sb.Clear();
                        if (signnumber)
                            sb.Append($"{i+1}. ");
                        sb.Append(mode444? cc4.GetCube4().GetScramble(): upsetcoordinate ? 
                            cc3.GetCube3(center).GetScramble(rd.Next(24)):
                            cc3.GetCube3(center).GetScramble()).Append("\r\n");
                        Invoke(a, sb.ToString());
                    }
                    Invoke(new Action(() => done.Text = "开始"));
                })).Start();
            }
        }
        private void Result_MouseWheel(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                int newSize = (int)result.Font.Size + e.Delta / 120;
                if (newSize > 8 && newSize < 30)
                    result.Font = new System.Drawing.Font(result.Font.FontFamily, newSize);
            }
        }
        private void Parity_Changed(object sender, EventArgs e)
        {
            epr.SelectedIndex = cpr.SelectedIndex = (sender as ComboBox).SelectedIndex;
        }
        private void Mode_Changed(object sender, EventArgs e)
        {
            if (sender == mode3)
            {
                edgeBox.Enabled = mode3.Checked = true;
                wingBox.Enabled = xcenterBox.Enabled = mode4.Checked = false;
                amountBox.MaxLength = 3;
                cc = cc3;
            }
            else
            {
                edgeBox.Enabled = mode3.Checked = false;
                wingBox.Enabled = xcenterBox.Enabled = mode4.Checked = true;
                amountBox.MaxLength = 2;
                cc = cc4;
            }
            result.Clear();
        }

        private void 棱编码数ToolStripMenuItem_Click(object sender, EventArgs e) => 
            MessageBox.Show(Edge.Stat(x => x.CodeLength));

        private void 棱翻色数ToolStripMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show(Edge.Stat(x => x.FlipAmount));

        private void 棱大循环长度ToolStripMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show(Edge.Stat(x => x.MainCycle.Item1));

        private void 棱小循环数ToolStripMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show(Edge.Stat(x => x.OtherCycleAmount));

        private void 角编码数ToolStripMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show(Corner.Stat(x => x.CodeLength));

        private void 角翻色数ToolStripMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show(Corner.Stat(x => x.TwistAmount));

        private void 角大循环长度ToolStripMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show(Corner.Stat(x => x.MainCycle.Item1));

        private void 角小循环数ToolStripMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show(Corner.Stat(x => x.OtherCycleAmount));

        private void 翼棱编码数ToolStripMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show(Wing.Stat(x => x.CodeLength));

        private void 翼棱大循环长度ToolStripMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show(Wing.Stat(x => x.MainCycle));

        private void 翼棱小循环数ToolStripMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show(Wing.Stat(x => x.OtherCycleAmount));

        private void 角心未归位数ToolStripMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show(XCenter.Stat());

        private void 三盲棱公式数ToolStripMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show(Resources.EdgeStatistics);

        private void 三盲角公式数ToolStripMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show(Resources.CornerStatistics);

        private void 三盲总公式数ToolStripMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show(Resources.CubeStatistics);
        private void AuthorInfo(object sender, EventArgs e)
            => MessageBox.Show(Resources.String1);

        private void 概率ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (UpdateData())
                MessageBox.Show(cc.Stat());
        }
    }
}
