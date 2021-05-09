using System.Windows.Forms;

namespace BLD_Master
{
    public class RadioToolStripItems
    {
        private int checkedIndex = 0;
        private readonly ToolStripItemCollection items;
        public int CheckedIndex
        {
            get => checkedIndex;
            set
            {
                (items[checkedIndex] as ToolStripMenuItem).Checked = false;
                (items[checkedIndex = value] as ToolStripMenuItem).Checked = true;
            }
        }

        public RadioToolStripItems(ToolStripMenuItem parent, string[] ss)
        {
            int c = 0;
            foreach (string s in ss)
                parent.DropDownItems.Add(new ToolStripMenuItem
                {
                    Size = new System.Drawing.Size(224, 26),
                    Text = s,
                    Tag = c++
                });
            parent.DropDownItemClicked += (sender, e) => CheckedIndex = (int)e.ClickedItem.Tag;
            items = parent.DropDownItems;
            (items[0] as ToolStripMenuItem).Checked = true;
        }
    }
}
