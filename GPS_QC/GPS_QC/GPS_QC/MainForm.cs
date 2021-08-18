using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace GPS_QC
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        #region File Loading

        List<variables> Lvariables = new List<variables>();

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileOpen();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            FileOpen();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Remove();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Remove();
        }

        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveAll();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            RemoveAll();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FileOpen()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Open RINEX observation file";
            openFileDialog1.Filter = "Observation files (*.*o)|*.*o|All files (*.*)|*.*";
            openFileDialog1.RestoreDirectory = true;

            string obs_filename;
            string nav_filename;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(openFileDialog1.FileName.Substring(0, openFileDialog1.FileName.Length - 1) + "n"))
                {
                    obs_filename = openFileDialog1.FileName;
                    nav_filename = openFileDialog1.FileName.Substring(0, openFileDialog1.FileName.Length - 1) + "n";
                    batch_procassing(obs_filename, nav_filename);
                }
                else
                {
                    obs_filename = openFileDialog1.FileName;

                    OpenFileDialog openFileDialog2 = new OpenFileDialog();

                    openFileDialog2.Title = "Open RINEX navigation file";
                    openFileDialog2.Filter = "Navigation files (*.*n)|*.*n|All files (*.*)|*.*";
                    openFileDialog2.RestoreDirectory = true;

                    if (openFileDialog2.ShowDialog() == DialogResult.OK)
                    {
                        nav_filename = openFileDialog2.FileName;
                        batch_procassing(obs_filename, nav_filename);
                    }
                }
            }
        }

        private void Remove()
        {
            if (Lvariables.Count != 0)
            {
                if (selectedParentNodename == "")
                {
                    MessageBox.Show("Select Tree  Node", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    int rNodeNo = -1;

                    for (int i = 0; i < Lvariables.Count; i++)
                    {
                        variables vr = Lvariables[i];
                        if (Path.GetFileNameWithoutExtension(vr.obs_filename) == selectedParentNodename)
                            rNodeNo = i;
                    }

                    treeView1.Nodes[rNodeNo].Remove();
                    Lvariables.RemoveAt(rNodeNo);

                    if (Lvariables.Count == 0)
                        textBox1.Text = "";
                }
            }
        }

        private void RemoveAll()
        {
            if (Lvariables.Count != 0)
            {
                int temp = Lvariables.Count;
                for (int i = 0; i < temp; i++)
                {
                    treeView1.Nodes[0].Remove();
                    Lvariables.RemoveAt(0);
                }
                textBox1.Text = "";
            }
        }

        private void batch_procassing(string obs_filename, string nav_filename)
        {
            bool samefile = false;

            for (int i = 0; i < Lvariables.Count; i++)
            {
                if (Lvariables[i].obs_filename == obs_filename)
                    samefile = true;
            }

            if (samefile == true)
            {
                MessageBox.Show("Same File Loading!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                variables vr = Processing(obs_filename, nav_filename);
                Lvariables.Add(vr);
                tree_ADD(Path.GetFileNameWithoutExtension(obs_filename));
                treeView1.SelectedNode = treeView1.Nodes[treeView1.Nodes.Count - 1];
            }
        }

        private void tree_ADD(string filename)
        {
            TreeNode treenode = treeView1.Nodes.Add(filename);

            treenode.Nodes.Add("Sky Plot");
            treenode.Nodes.Add("Azimuth");
            treenode.Nodes.Add("Elevation");
            treenode.Nodes.Add("DOP");
            treenode.Nodes.Add("ION");
            treenode.Nodes.Add("IOD");
            //treenode.Nodes.Add("ION Map");
            treenode.Nodes.Add("Multi-path L1");
            treenode.Nodes.Add("Multi-path L2");
            treenode.Nodes.Add("Cycle-slip Code");
            treenode.Nodes.Add("Cycle-slip Carrier");

            treenode.ExpandAll();
        }

        private variables Processing(string obs_filename, string nav_filename)
        {
            variables vr = new variables();

            if (nav_filename == null || obs_filename == null)
            {
                MessageBox.Show("Please select RINEX Observation | Navigation File");
                return vr;
            }

            try
            {
                readRinex rR = new readRinex();
                vr.obs = rR.observation(obs_filename);
                vr.eph = rR.nav_eph(nav_filename);

                azieledop aed = new azieledop();
                vr.SkyDops = aed.azieledops(rR.obs_time, vr.eph, rR.approx_position, vr.obs);

                ionmp im = new ionmp();
                vr.ion_mp = im.mp(rR.type_observations, vr.obs, rR.obs_time);

                cycleslip cs = new cycleslip();
                vr.cycleslips = cs.cs(rR.type_observations, vr.obs, rR.obs_time, rR.delta_HEN);


                vr.obs_filename = obs_filename;
                vr.nav_filename = nav_filename;

                vr.time_original = rR.time_original;
                vr.obs_time = rR.obs_time;
                vr.obseph = rR.obseph;
                vr.process_name = rR.process_name;
                vr.obs_name = rR.obs_name;
                vr.mark_name = rR.mark_name;
                vr.mark_number = rR.mark_number;
                vr.interval = rR.interval;
                vr.time_first = rR.time_first;
                vr.time_last = rR.time_last;
                vr.rec_type = rR.rec_type;
                vr.ant_type = rR.ant_type;
                vr.approx_position = rR.approx_position;
                vr.delta_HEN = rR.delta_HEN;
                vr.deltaUTC = rR.deltaUTC;
                vr.ion_alpha = rR.ion_alpha;
                vr.ion_beta = rR.ion_beta;
                vr.XYZ = aed.convertXYZtoGEO(rR.approx_position);
                vr.lPrn = rR.lPrn;
                vr.type_observations = rR.type_observations;
                vr.type_obs = rR.type_obs;

                //총 위성 번호 확인 작업
                int i, j;
                j = 0;
                for (i = 0; i < 32; i++)
                {
                    if (rR.svss[i] == true)
                        j = j + 1;
                }

                vr.svss = new int[j];
                j = 0;
                for (i = 0; i < 32; i++)
                {
                    if (rR.svss[i] == true)
                    {
                        vr.svss[j] = i + 1;
                        j = j + 1;
                    }
                }
                //총 위성 번호 확인 작업, svss 행렬

                return vr;
            }
            catch (Exception exc)
            {
                MessageBox.Show("ERROR : " + exc.ToString());
            }

            return vr;
        }

        #endregion

        #region DataSet Handling

        string selectedParentNodename;
        string selectedNodename;

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode.Parent == null)
            {
                selectedParentNodename = treeView1.SelectedNode.Text;
                selectedNodename = "ROOT";
                int temp = findParentNodeNo(Lvariables, selectedParentNodename);
                dataset_info(Lvariables[temp], textBox1, selectedNodename);
            }
            else
            {
                selectedNodename = treeView1.SelectedNode.Text;
                selectedParentNodename = treeView1.SelectedNode.Parent.Text;
                int temp = findParentNodeNo(Lvariables, selectedParentNodename);
                dataset_info(Lvariables[temp], textBox1, selectedNodename);
            }
        }

        private int findParentNodeNo(List<variables> _vr, string _filename)
        {
            int rNodeNo = -1;

            for (int i=0; i<_vr.Count; i++)
            {
                variables __vr = _vr[i];
                if (Path.GetFileNameWithoutExtension(__vr.obs_filename) == _filename)
                    rNodeNo = i;
            }

            return rNodeNo;
        }

        private void dataset_info(variables vr, TextBox tBox, string Nodename)
        {
            string temp = "";
            double result;
            ParameterForm pForm = new ParameterForm();
            float[,] parameter = pForm.GetParameter;

            switch (Nodename)
            {                  
                case "ROOT":
                    temp = temp + "Name of observer / agency:  " + vr.obs_name + "\r\n";
                    temp = temp + "\r\n";
                    temp = temp + "Obsevation File Name:  " + Path.GetFileName(vr.obs_filename) + "\r\n";
                    temp = temp + "Navigation File Name:  " + Path.GetFileName(vr.nav_filename) + "\r\n";
                    temp = temp + "\r\n";
                    temp = temp + "Name of antenna marker:  " + vr.mark_name + "\r\n";
                    temp = temp + "Number of antenna marker:  " + vr.mark_number + "\r\n";
                    temp = temp + "\r\n";
                    temp = temp + "Name of program creating cuvrent file:  " + vr.process_name.Substring(0, 20) + "\r\n";
                    temp = temp + "Name of agency  creating cuvrent file:  " + vr.process_name.Substring(19, 15) + "\r\n";
                    temp = temp + "Date of file creation:  " + vr.process_name.Substring(34, vr.process_name.Length - 34) + "\r\n";
                    temp = temp + "\r\n";
                    string sv = null;
                    for (int i = 0; i < vr.svss.Length; i++)
                        sv += " " + Convert.ToString(vr.svss[i]);
                    temp = temp + "Number of recorded satellites:  " + sv + "\r\n";
                    temp = temp + "# / TYPES OF OBSERV:  \t" + vr.type_obs.Substring(0,40) + "\r\n";
                    for (int i = 0; i < vr.lPrn.Count; i++)
                    {
                        temp = temp + "PRN / # OF OBS: \t\t" + vr.lPrn[i].Substring(0, 40) + "\r\n";
                    }
                    temp = temp + "\r\n";
                    temp = temp + "Time of first observation record:  " + vr.time_first + "\r\n";
                    temp = temp + "Time of last observation record:  " + vr.time_last + "\r\n";
                    double tot_obs_time = vr.obs_time[vr.obs_time.Length - 1] - vr.obs_time[0];
                    string str_tot_obs_time = " " + Convert.ToString((int)(tot_obs_time / 3600)) + ":" + Convert.ToString((int)(tot_obs_time % 3600) / 60) + ": " + Convert.ToString(Math.Round(tot_obs_time % 60, 5));
                    temp = temp + "Time of total record:  " + str_tot_obs_time + "\r\n";
                    temp = temp + "Number of record:  " + vr.obseph + "\r\n";
                    temp = temp + "Observation interval in seconds:  " + vr.interval + "\r\n";
                    temp = temp + "Delta time due to leap seconds:  " + Convert.ToString(vr.leap_seconds) + "\r\n";
                    temp = temp + "\r\n";
                    temp = temp + "Approximate marker position (WGS84, X/Y/Z):  " + Convert.ToString(vr.approx_position[0]) + "  " + Convert.ToString(vr.approx_position[1]) + "  " + Convert.ToString(vr.approx_position[2]) + "\r\n";
                    temp = temp + "Approximate marker position (WGS84, Lat/Lon/Ell):  " + Convert.ToString(vr.XYZ[0]) + "  " + Convert.ToString(vr.XYZ[1]) + "  " + Convert.ToString(vr.XYZ[2]) + "\r\n";
                    temp = temp + "Antenna height(DELTA H/E/N):  " + Convert.ToString(vr.delta_HEN[0]) + "  " + Convert.ToString(vr.delta_HEN[1]) + "  " + Convert.ToString(vr.delta_HEN[2]) + "\r\n";
                    temp = temp + "\r\n";
                    temp = temp + "Receiver number, type, and version:  " + vr.rec_type + "\r\n";
                    temp = temp + "Antenna number and type:  " + vr.ant_type + "\r\n";
                    temp = temp + "\r\n";
                    temp = temp + "Almanac parameters to compute time in UTC:  " + Convert.ToString(vr.deltaUTC[0]) + "  " + Convert.ToString(vr.deltaUTC[1]) + "  " + Convert.ToString(vr.deltaUTC[2]) + "  " + Convert.ToString(vr.deltaUTC[3]) + "\r\n";
                    temp = temp + "Ionosphere parameters A0-A3 of almanac:  " + Convert.ToString(vr.ion_alpha[0])+"  "+Convert.ToString(vr.ion_alpha[1])+"  "+Convert.ToString(vr.ion_alpha[2])+"  "+Convert.ToString(vr.ion_alpha[3]) + "\r\n";
                    temp = temp + "Ionosphere parameters B0-B3 of almanac:  " + Convert.ToString(vr.ion_beta[0]) + "  " + Convert.ToString(vr.ion_beta[1]) + "  " + Convert.ToString(vr.ion_beta[2]) + "  " + Convert.ToString(vr.ion_beta[3]) + "\r\n";
                    temp = temp + "\r\n";
                    temp = temp + "--------------------------------------------------------------------------" + "\r\n";
                    temp = temp + "ELE QC Result:  \t" + QC_elevation(vr).ToString() + " % " + "\t(Parameter: " + parameter[0, 0].ToString() + "~" + parameter[0, 1].ToString() + ")" + "\r\n";
                    temp = temp + "DOP QC Result:  \t" + QC_DOP(vr).ToString() + " % " + "\t(Parameter: " + parameter[1, 0].ToString() + "~" + parameter[1, 1].ToString() + ")" + "\r\n";
                    temp = temp + "ION QC Result:  \t" + QC_ION(vr).ToString() + " % " + "\t(Parameter: " + parameter[2, 0].ToString() + "~" + parameter[2, 1].ToString() + ")" + "\r\n";
                    temp = temp + "IOD QC Result:  \t" + QC_IOD(vr).ToString() + " % " + "\t(Parameter: " + parameter[3, 0].ToString() + "~" + parameter[3, 1].ToString() + ")" + "\r\n";
                    temp = temp + "MP1 QC Result:  \t" + QC_MP1(vr).ToString() + " % " + "\t(Parameter: " + parameter[4, 0].ToString() + "~" + parameter[4, 1].ToString() + ")" + "\r\n";
                    temp = temp + "MP2 QC Result:  \t" + QC_MP2(vr).ToString() + " % " + "\t(Parameter: " + parameter[5, 0].ToString() + "~" + parameter[5, 1].ToString() + ")" + "\r\n";
                    temp = temp + "CS1 QC Result:  \t" + QC_cycleslip1(vr).ToString() + " % " + "\t(Parameter: " + parameter[6, 0].ToString() + "~" + parameter[6, 1].ToString() + ")" + "\r\n";
                    temp = temp + "CS2 QC Result:  \t" + QC_cycleslip2(vr).ToString() + " % " + "\t(Parameter: " + parameter[7, 0].ToString() + "~" + parameter[7, 1].ToString() + ")" + "\r\n";
                    
                    break;
                case "Sky Plot":
                    temp = temp + "SkyPlot is Blar Blar...." + "\r\n";
                    temp = temp + "This is a numeric formula of SkyPlot...." + "\r\n";
                    temp = temp + "\r\n";
                    break;
                case "Azimuth":
                    temp = temp + "Azimuth is Blar Blar...." + "\r\n";
                    temp = temp + "This is a numeric formula of Azimuth...." + "\r\n";
                    temp = temp + "\r\n";
                    break;
                case "Elevation":
                    temp = temp + "Elevation is Blar Blar...." + "\r\n";
                    temp = temp + "This is a numeric formula of Elevation...." + "\r\n";
                    temp = temp + "\r\n";
                    result = QC_elevation(vr);
                    temp = temp + "--------------------------------------------------------------------" + "\r\n";
                    temp = temp + "Quality Check Result:  " + result.ToString() + " %" + "\r\n";
                    temp = temp + "(Parameter: " + parameter[0, 0].ToString() + "~" + parameter[0, 1].ToString() + ")" + "\r\n";
                    break;
                case "DOP":
                    temp = temp + "DOP is Blar Blar...." + "\r\n";
                    temp = temp + "This is a numeric formula of DOP...." + "\r\n";
                    temp = temp + "\r\n";
                    result = QC_DOP(vr);
                    temp = temp + "--------------------------------------------------------------------" + "\r\n";
                    temp = temp + "Quality Check Result:  " + result.ToString() + " %" + "\r\n";
                    temp = temp + "(Parameter: " + parameter[1, 0].ToString() + "~" + parameter[1, 1].ToString() + ")" + "\r\n";
                    break;
                case "ION":
                    temp = temp + "ION is Blar Blar...." + "\r\n";
                    temp = temp + "This is a numeric formula of ION...." + "\r\n";
                    temp = temp + "\r\n";
                    result = QC_ION(vr);
                    temp = temp + "--------------------------------------------------------------------" + "\r\n";
                    temp = temp + "Quality Check Result:  " + result.ToString() + " %" + "\r\n";
                    temp = temp + "(Parameter: " + parameter[2, 0].ToString() + "~" + parameter[2, 1].ToString() + ")" + "\r\n";
                    break;
                case "IOD":
                    temp = temp + "IOD is Blar Blar...." + "\r\n";
                    temp = temp + "This is a numeric formula of IOD...." + "\r\n";
                    temp = temp + "\r\n";
                    result = QC_IOD(vr);
                    temp = temp + "--------------------------------------------------------------------" + "\r\n";
                    temp = temp + "Quality Check Result:  " + result.ToString() + " %" + "\r\n";
                    temp = temp + "(Parameter: " + parameter[3, 0].ToString() + "~" + parameter[3, 1].ToString() + ")" + "\r\n";
                    break;
                case "ION Map":
                    temp = temp + "ION Map is Blar Blar...." + "\r\n";
                    temp = temp + "This is a numeric formula of ION Map...." + "\r\n";
                    temp = temp + "\r\n";
                    break;
                case "Multi-path L1":
                    temp = temp + "Multi-path L1 is Blar Blar...." + "\r\n";
                    temp = temp + "This is a numeric formula of Multi-path L1...." + "\r\n";
                    temp = temp + "\r\n";
                    result = QC_MP1(vr);
                    temp = temp + "--------------------------------------------------------------------" + "\r\n";
                    temp = temp + "Quality Check Result:  " + result.ToString() + " %" + "\r\n";
                    temp = temp + "(Parameter: " + parameter[4, 0].ToString() + "~" + parameter[4, 1].ToString() + ")" + "\r\n";
                    break;
                case "Multi-path L2":
                    temp = temp + "Multi-path L2 is Blar Blar...." + "\r\n";
                    temp = temp + "This is a numeric formula of Multi-path L2...." + "\r\n";
                    temp = temp + "\r\n";
                    result = QC_MP2(vr);
                    temp = temp + "--------------------------------------------------------------------" + "\r\n";
                    temp = temp + "Quality Check Result:  " + result.ToString() + " %" + "\r\n";
                    temp = temp + "(Parameter: " + parameter[5, 0].ToString() + "~" + parameter[5, 1].ToString() + ")" + "\r\n";
                    break;
                case "Cycle-slip Code":
                    temp = temp + "Cycle-slip Code is Blar Blar...." + "\r\n";
                    temp = temp + "This is a numeric formula of Cycle-slip Code...." + "\r\n";
                    temp = temp + "\r\n";
                    result = QC_cycleslip1(vr);
                    temp = temp + "--------------------------------------------------------------------" + "\r\n";
                    temp = temp + "Quality Check Result:  " + result.ToString() + " %" + "\r\n";
                    temp = temp + "(Parameter: " + parameter[6, 0].ToString() + "~" + parameter[6, 1].ToString() + ")" + "\r\n";
                    break;
                case "Cycle-slip Carrier":
                    temp = temp + "Cycle-slip Carrier is Blar Blar...." + "\r\n";
                    temp = temp + "This is a numeric formula of Cycle-slip Carrier...." + "\r\n";
                    temp = temp + "\r\n";
                    result = QC_cycleslip2(vr);
                    temp = temp + "--------------------------------------------------------------------" + "\r\n";
                    temp = temp + "Quality Check Result:  " + result.ToString() + " %" + "\r\n";
                    temp = temp + "(Parameter: " + parameter[7, 0].ToString() + "~" + parameter[7, 1].ToString() + ")" + "\r\n";
                    break;
            }

            tBox.Text = temp;
        }

        private double QC_elevation(variables vr)
        {
            ParameterForm pForm = new ParameterForm();
            float[,] parameter = pForm.GetParameter;

            float count1 = 0;
            float count2 = 0;
            for (int i = 0; i < vr.svss.Length; i++)
            {
                for (int j = 0; j < vr.obseph; j++)
                {
                    if (vr.SkyDops[j, vr.svss[i] - 1, 1] != 0)
                    {
                        count2++;
                    }

                    if (vr.SkyDops[j, vr.svss[i] - 1, 1] > parameter[0, 0] && vr.SkyDops[j, vr.svss[i] - 1, 1] < parameter[0, 1])
                    {
                        count1++;
                    }

                }
            }
            double result = Math.Round((count1 / count2) * 100, 3);
            return result;
        }

        private double QC_DOP(variables vr)
        {
            ParameterForm pForm = new ParameterForm();
            float[,] parameter = pForm.GetParameter;

            float count1 = 0;
            for (int q = 0; q < 5; q++)
            {
                for (int p = 0; p < vr.obseph; p++)
                {
                    if (vr.SkyDops[p, 33, 0] > parameter[1, 0] && vr.SkyDops[p, 33, 0] < parameter[1, 1])
                    {
                        count1 = count1 + 1;
                    }
                }
            }
            double result = Math.Round((count1 / vr.obseph) * 20, 3);
            return result;
        }

        private double QC_ION(variables vr)
        {
            ParameterForm pForm = new ParameterForm();
            float[,] parameter = pForm.GetParameter;

            float count1 = 0;
            float count2 = 0;
            for (int q = 0; q < vr.svss.Length; q++)
            {
                for (int p = 0; p < vr.obseph; p++)
                {
                    if (vr.ion_mp[p, vr.svss[q] - 1, 0] > parameter[2, 0] && vr.ion_mp[p, vr.svss[q] - 1, 0] < parameter[2, 1])
                    {
                        if (vr.ion_mp[p, vr.svss[q] - 1, 0] != 0)
                        {
                            count2 = count2 + 1;
                        }
                    }
                    else
                    {
                        count1 = count1 + 1;
                    }

                }
            }
            double result = Math.Round(count2 * 100 / (count1 + count2), 3);
            return result;
        }

        private double QC_IOD(variables vr)
        {
            ParameterForm pForm = new ParameterForm();
            float[,] parameter = pForm.GetParameter;

            float count1 = 0;
            float count2 = 0;
            for (int q = 0; q < vr.svss.Length; q++)
            {
                for (int p = 0; p < vr.obseph; p++)
                {
                    if (vr.ion_mp[p, vr.svss[q] - 1, 1] > parameter[3, 0] && vr.ion_mp[p, vr.svss[q] - 1, 1] < parameter[3, 1])
                    {
                        if (vr.ion_mp[p, vr.svss[q] - 1, 1] != 0)
                        {
                            count2 = count2 + 1;
                        }
                    }
                    else
                    {
                        count1 = count1 + 1;
                    }
                }
            }
            double result = Math.Round(count2 / (count1 + count2) * 100, 3);
            return result;
        }

        private double QC_MP1(variables vr)
        {
            ParameterForm pForm = new ParameterForm();
            float[,] parameter = pForm.GetParameter;

            float count1 = 0;
            float count2 = 0;
            for (int q = 0; q < vr.svss.Length; q++)
            {
                for (int p = 0; p < vr.obseph; p++)
                {
                    if (vr.ion_mp[p, vr.svss[q] - 1, 2] > parameter[4, 0] && vr.ion_mp[p, vr.svss[q] - 1, 2] < parameter[4, 1])
                    {
                        if (vr.ion_mp[p, vr.svss[q] - 1, 2] != 0)
                        {
                            count2 = count2 + 1;
                        }
                    }
                    else
                    {
                        count1 = count1 + 1;
                    }

                }
            }
            double result = Math.Round(count2 / (count1 + count2) * 100, 3);
            return result;
        }

        private double QC_MP2(variables vr)
        {
            ParameterForm pForm = new ParameterForm();
            float[,] parameter = pForm.GetParameter;

            float count1 = 0;
            float count2 = 0;
            for (int q = 0; q < vr.svss.Length; q++)
            {
                for (int p = 0; p < vr.obseph; p++)
                {
                    if (vr.ion_mp[p, vr.svss[q] - 1, 3] > parameter[5, 0] && vr.ion_mp[p, vr.svss[q] - 1, 3] < parameter[5, 1])
                    {
                        if (vr.ion_mp[p, vr.svss[q] - 1, 3] != 0)
                        {
                            count2 = count2 + 1;
                        }
                    }
                    else
                    {
                        count1 = count1 + 1;
                    }

                }
            }
            double result = Math.Round(count2 / (count1 + count2) * 100, 3);
            return result;
        }

        private double QC_cycleslip1(variables vr)
        {
            ParameterForm pForm = new ParameterForm();
            float[,] parameter = pForm.GetParameter;

            float count1 = 0;
            float count2 = 0;
            for (int p = 0; p < vr.obseph - 2; p++)
            {
                for (int q = 0; q < vr.svss.Length; q++)
                {

                    if (vr.cycleslips[p, vr.svss[q] - 1, 2] > parameter[6, 0] && vr.cycleslips[p, vr.svss[q] - 1, 2] < parameter[6, 1])
                    {
                        if (vr.cycleslips[p, vr.svss[q] - 1, 2] != 0)
                        {
                            count2 = count2 + 1;
                        }
                    }
                    else
                    {
                        count1 = count1 + 1;
                    }

                }
            }
            double result = Math.Round(count2 / (count1 + count2) * 100, 3);
            return result;
        }

        private double QC_cycleslip2(variables vr)
        {
            ParameterForm pForm = new ParameterForm();
            float[,] parameter = pForm.GetParameter;

            float count1 = 0;
            float count2 = 0;
            for (int p = 0; p < vr.obseph - 2; p++)
            {
                for (int q = 0; q < vr.svss.Length; q++)
                {

                    if (vr.cycleslips[p, vr.svss[q] - 1, 3] > parameter[7, 0] && vr.cycleslips[p, vr.svss[q] - 1, 3] < parameter[7, 1])
                    {
                        if (vr.cycleslips[p, vr.svss[q] - 1, 3] != 0)
                        {
                            count2 = count2 + 1;
                        }
                    }
                    else
                    {
                        count1 = count1 + 1;
                    }

                }
            }
            double result = Math.Round(count2 / (count1 + count2) * 100, 3);
            return result;
        }

        #endregion

        #region Plot Handling

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Plotting();
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            Plotting();
        }

        private void showPlotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Plotting();
        }

        private void showArrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Plotting();
        }

        private void showMaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Plotting();
        }
        
        public PlotForm pForm = null;

        private void Plotting()
        {
            if (Lvariables.Count != 0)
            {
                if (selectedNodename != "ROOT")
                {
                    if (pForm != null)
                    {
                        pForm.Dispose();
                    }

                    pForm = new PlotForm(Lvariables[findParentNodeNo(Lvariables, selectedParentNodename)], selectedNodename);
                    pForm.Text = Path.GetFileName(selectedParentNodename) + " - " + selectedNodename;
                    pForm.Show();
                }
            }
        }

        #endregion

        #region Parameter Handling
        // Parameters
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            ParameterForm pForm = new ParameterForm();
            pForm.Show();
        }
        #endregion

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("GPS Quality Control Software (v1.0) \r\n \r\nSungkyunkwan University Laboratory of Geo-informatics\r\nhttp://geo.skku.ac.kr", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #region Report Handling
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            ReportForm rForm = new ReportForm();
            rForm.Show();
        }
        #endregion
    }
}
