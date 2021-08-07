using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SVCT_Windows
{

    public partial class Form_Main : Form
    {

        public Form_Main()
        {
            InitializeComponent();
            source.Text = IniFunc.ReadInit("Language","source","");
            target.Text = IniFunc.ReadInit("Language","target","");
        }

        private void source_SelectedIndexChanged(object sender, EventArgs e)
        {
            IniFunc.WriteInit("Language","source",source.Text);
        }

        private void target_SelectedIndexChanged(object sender, EventArgs e)
        {
            IniFunc.WriteInit("Language", "target", target.Text);
        }

        private void start_Click_1(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            Caption caption = new Caption();
            caption.Show();
        }

        private void exit_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
