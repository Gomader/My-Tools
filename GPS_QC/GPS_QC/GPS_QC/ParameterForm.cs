using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GPS_QC
{
    public partial class ParameterForm : Form
    {
        public ParameterForm()
        {
            InitializeComponent();

            string path = Path.GetDirectoryName(Application.ExecutablePath) + "\\parameter.txt";

            using (StreamReader sr = new StreamReader(path))
            {
                string Temp;

                while ((Temp = sr.ReadLine()) != null)
                {
                    string[] Temp2 = Temp.Split(':');
                    string[] temp3;

                    switch (Temp2[0])
                    {
                        case "ELE":
                            temp3 = Temp2[1].Split('~');
                            txtBox_Ell_1.Text = temp3[0];
                            txtBox_Ell_2.Text = temp3[1];
                            break;
                        case "DOP":
                            temp3 = Temp2[1].Split('~');
                            txtBox_DOP_1.Text = temp3[0];
                            txtBox_DOP_2.Text = temp3[1];
                            break;
                        case "ION":
                            temp3 = Temp2[1].Split('~');
                            txtBox_ION_1.Text = temp3[0];
                            txtBox_ION_2.Text = temp3[1];
                            break;
                        case "IOD":
                            temp3 = Temp2[1].Split('~');
                            txtBox_IOD_1.Text = temp3[0];
                            txtBox_IOD_2.Text = temp3[1];
                            break;
                        case "MP1":
                            temp3 = Temp2[1].Split('~');
                            txtBox_MP1_1.Text = temp3[0];
                            txtBox_MP1_2.Text = temp3[1];
                            break;
                        case "MP2":
                            temp3 = Temp2[1].Split('~');
                            txtBox_MP2_1.Text = temp3[0];
                            txtBox_MP2_2.Text = temp3[1];
                            break;
                        case "CS1":
                            temp3 = Temp2[1].Split('~');
                            txtBox_CS1_1.Text = temp3[0];
                            txtBox_CS1_2.Text = temp3[1];
                            break;
                        case "CS2":
                            temp3 = Temp2[1].Split('~');
                            txtBox_CS2_1.Text = temp3[0];
                            txtBox_CS2_2.Text = temp3[1];
                            break;
                    }
                }
            }
        }

        public string path { get; set; }

        public float[,] GetParameter
        {
            get
            {
                float[,] temp = new float[8, 2];
                temp[0, 0] = Convert.ToSingle(txtBox_Ell_1.Text);
                temp[0, 1] = Convert.ToSingle(txtBox_Ell_2.Text);
                temp[1, 0] = Convert.ToSingle(txtBox_DOP_1.Text);
                temp[1, 1] = Convert.ToSingle(txtBox_DOP_2.Text);
                temp[2, 0] = Convert.ToSingle(txtBox_ION_1.Text);
                temp[2, 1] = Convert.ToSingle(txtBox_ION_2.Text);
                temp[3, 0] = Convert.ToSingle(txtBox_IOD_1.Text);
                temp[3, 1] = Convert.ToSingle(txtBox_IOD_2.Text);
                temp[4, 0] = Convert.ToSingle(txtBox_MP1_1.Text);
                temp[4, 1] = Convert.ToSingle(txtBox_MP1_2.Text);
                temp[5, 0] = Convert.ToSingle(txtBox_MP2_1.Text);
                temp[5, 1] = Convert.ToSingle(txtBox_MP2_2.Text);
                temp[6, 0] = Convert.ToSingle(txtBox_CS1_1.Text);
                temp[6, 1] = Convert.ToSingle(txtBox_CS1_2.Text);
                temp[7, 0] = Convert.ToSingle(txtBox_CS2_1.Text);
                temp[7, 1] = Convert.ToSingle(txtBox_CS2_2.Text);

                return temp;
            }
        }

        // Save
        private void button1_Click(object sender, EventArgs e)
        {
            if (txtBox_CS1_1.Text == "")
                return;
            if (txtBox_CS1_2.Text == "")
                return;
            if (txtBox_CS2_1.Text == "")
                return;
            if (txtBox_CS2_2.Text == "")
                return;
            if (txtBox_DOP_1.Text == "")
                return;
            if (txtBox_DOP_2.Text == "")
                return;
            if (txtBox_Ell_1.Text == "")
                return;
            if (txtBox_Ell_2.Text == "")
                return;
            if (txtBox_IOD_1.Text == "")
                return;
            if (txtBox_IOD_2.Text == "")
                return;
            if (txtBox_ION_1.Text == "")
                return;
            if (txtBox_ION_2.Text == "")
                return;
            if (txtBox_MP1_1.Text == "")
                return;
            if (txtBox_MP1_2.Text == "")
                return;
            if (txtBox_MP2_1.Text == "")
                return;
            if (txtBox_MP2_2.Text == "")
                return;
            
            string path = Path.GetDirectoryName(Application.ExecutablePath) + "\\parameter.txt";

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("ELE:" + txtBox_Ell_1.Text + "~" + txtBox_Ell_2.Text);
                sw.WriteLine("DOP:" + txtBox_DOP_1.Text + "~" + txtBox_DOP_2.Text);
                sw.WriteLine("ION:" + txtBox_ION_1.Text + "~" + txtBox_ION_2.Text);
                sw.WriteLine("IOD:" + txtBox_IOD_1.Text + "~" + txtBox_IOD_2.Text);
                sw.WriteLine("MP1:" + txtBox_MP1_1.Text + "~" + txtBox_MP1_2.Text);
                sw.WriteLine("MP2:" + txtBox_MP2_1.Text + "~" + txtBox_MP2_2.Text);
                sw.WriteLine("CS1:" + txtBox_CS1_1.Text + "~" + txtBox_CS1_2.Text);
                sw.WriteLine("CS2:" + txtBox_CS2_1.Text + "~" + txtBox_CS2_2.Text);
            }

            this.Close();
        }
    }
}
