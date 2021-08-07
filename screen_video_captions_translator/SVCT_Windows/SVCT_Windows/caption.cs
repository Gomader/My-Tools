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
    public partial class Caption : Form
    {
        public static int flag;

        public Caption()
        {
            InitializeComponent();
            starts();
        }

        private void starts()
        {
            flag = 1;
            while (flag == 1)
            {

            }
        }

        private void stops()
        {
            flag = 0;
        }
    }
}
