using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

using ZedGraph;


namespace GPS_QC
{
    public partial class PlotForm : Form
    {
        public PlotForm()
        {
            InitializeComponent();
        }

        public PlotForm(variables vr, string selectedNodeName)
        {
            InitializeComponent();
            
            switch (selectedNodeName)
            {
                case "Sky Plot":
                    Plot_SkyPlot(vr);
                    tabPage2.Dispose();
                    tabPage3.Dispose();
                    break;
                case "Azimuth":
                    Plot_Azimuth(vr);
                    tabPage3.Dispose();
                    break;
                case "Elevation":
                    Plot_Elevation(vr);
                    tabPage3.Dispose();
                    break;
                case "DOP":
                    Plot_DOP(vr);
                    tabPage3.Dispose();
                    break;
                case "ION":
                    Plot_ION(vr);
                    tabPage3.Dispose();
                    break;
                case "IOD":
                    Plot_IOD(vr);
                    tabPage3.Dispose();
                    break;
                case "ION Map":
                    tabPage1.Dispose();
                    tabPage2.Dispose();
                    break;
                case "Multi-path L1":
                    Plot_MP1(vr);
                    tabPage3.Dispose();
                    break;
                case "Multi-path L2":
                    Plot_MP2(vr);
                    tabPage3.Dispose();
                    break;
                case "Cycle-slip Code":
                    Plot_CS1(vr);
                    tabPage3.Dispose();
                    break;
                case "Cycle-slip Carrier":
                    Plot_CS2(vr);
                    tabPage3.Dispose();
                    break;
            }
        }

        private void dataGridView_Setting(int[] svss)
        {
            dataGridView1.Columns.Add("GPS Time", "GPS Time");
            for (int i = 0; i < svss.Length; i++)
            {
                dataGridView1.Columns.Add(svss[i].ToString(), svss[i].ToString());
            }
        }

        #region Plot

        private void Plot_SkyPlot(variables vr)
        {
            //parameter setup:
            int noOfScaleCircles = 3;
            double scaleDiam = 90;

            // get a reference to the GraphPane
            GraphPane myPane = zedGraphControl1.GraphPane;

            // Set the Titles and axis proprieties

            myPane.Title.Text = "Sky Plot: " + Path.GetFileNameWithoutExtension(vr.obs_filename);
            myPane.XAxis.Title.Text = "";
            myPane.YAxis.Title.Text = "";
            myPane.XAxis.Cross = 0;
            myPane.YAxis.Cross = 0;

            myPane.XAxis.MajorTic.IsAllTics = true;
            myPane.XAxis.MinorTic.IsAllTics = true;
            myPane.YAxis.MajorTic.IsAllTics = true;
            myPane.YAxis.MinorTic.IsAllTics = true;
            myPane.XAxis.Scale.IsVisible = false;
            myPane.YAxis.Scale.IsVisible = false;

            myPane.XAxis.Scale.Min = -95;
            myPane.XAxis.Scale.Max = 95;
            myPane.YAxis.Scale.Max = 95;
            myPane.YAxis.Scale.Min = -95;

            // Render the simulated polar decorations:
            RadarPointList[] scaleCircleList = new RadarPointList[noOfScaleCircles];
            LineItem[] scaleCircle = new LineItem[noOfScaleCircles];
            double delta = (double)scaleDiam / noOfScaleCircles;

            for (int j = 0; j < noOfScaleCircles; j++)
            {
                scaleCircleList[j] = new RadarPointList();
                for (int i = 0; i < 20; i++)
                {
                    scaleCircleList[j].Add(scaleDiam, 1);
                }

                scaleCircle[j] = myPane.AddCurve("", scaleCircleList[j], Color.Black, SymbolType.None);
                scaleCircle[j].Line.IsSmooth = true;
                scaleCircle[j].Line.SmoothTension = 0.6f;
                scaleCircle[j].Line.Style = System.Drawing.Drawing2D.DashStyle.Custom;
                scaleCircle[j].Line.DashOff = 2;
                scaleCircle[j].Line.DashOn = 4;

                scaleDiam = scaleDiam - delta;

            }
            // Render the "rays" from the center
            for (int j = 0; j < 20; j++)
            {
                LineObj line = new ArrowObj(Color.Black, 0, 0, 0, (float)scaleCircleList[0][j].X, (float)scaleCircleList[0][j].Y);
                //line.Style = System.Drawing.Drawing2D.DashStyle.Custom;
                //line.Line.DashOn = 1;
                //line.Line.DashOff = 4;
                myPane.GraphObjList.Add(line);
            }

            // This is plot data
            Random rd = new Random();
            for (int p = 0; p < vr.svss.Length; p++)
            {
                PointPairList dataList = new PointPairList();

                for (int q = 0; q < vr.obseph; q++)
                {
                    double theta = (vr.SkyDops[q, vr.svss[p] - 1, 0]) * (Math.PI / 180);
                    double l = 90 - (vr.SkyDops[q, vr.svss[p] - 1, 1]);
                    double x = l * Math.Sin(theta);
                    double y = l * Math.Cos(theta);

                    if (x != 0)
                        dataList.Add(x, y);
                }

                LineItem data = myPane.AddCurve(vr.svss[p].ToString(), dataList, Color.FromArgb(rd.Next(256), rd.Next(256), rd.Next(256)), SymbolType.Circle);
                data.Symbol.Size = 4;
                data.Symbol.Fill.IsVisible = true;
                data.Line.IsVisible = false;
            }

            zedGraphControl1.AxisChange();
        }

        private void Plot_Azimuth(variables vr)
        {
            // DataGridView
            dataGridView_Setting(vr.svss);
            for (int p = 0; p < vr.obseph; p++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[p].Cells[0].Value = vr.obs_time[p];

                for (int q = 0; q < vr.svss.Length; q++)
                {
                    dataGridView1.Rows[p].Cells[q + 1].Value = vr.SkyDops[p, vr.svss[q] - 1, 0];
                }
            }

            // Get a reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = zedGraphControl1.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "Azimuth: " + Path.GetFileNameWithoutExtension(vr.obs_filename);
            myPane.XAxis.Title.Text = "GPS Time";
            myPane.YAxis.Title.Text = "Degree";

            // Make up some data points based on the Sine function
            Random rd = new Random();
            for (int q = 0; q < vr.svss.Length; q++)
            {
                PointPairList list = new PointPairList();
                int n = dataGridView1.Rows.Add();
                for (int p = 0; p < vr.obseph; p++)
                {
                    double x = vr.obs_time[p];
                    double y = vr.SkyDops[p, vr.svss[q] - 1, 0];

                    if (y != 0)
                        list.Add(x, y);
                }

                LineItem myCurve = myPane.AddCurve(vr.svss[q].ToString(), list, Color.FromArgb(rd.Next(256), rd.Next(256), rd.Next(256)), SymbolType.Circle);
                myCurve.Symbol.Size = 4;
                myCurve.Symbol.Fill.IsVisible = true;
                myCurve.Line.IsVisible = false;
            }

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;
            
            zedGraphControl1.AxisChange();
        }

        private void Plot_Elevation(variables vr)
        {
            // DataGridView
            dataGridView_Setting(vr.svss);
            for (int p = 0; p < vr.obseph; p++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[p].Cells[0].Value = vr.obs_time[p];

                for (int q = 0; q < vr.svss.Length; q++)
                {
                    dataGridView1.Rows[p].Cells[q + 1].Value = vr.SkyDops[p, vr.svss[q] - 1, 1];
                }
            }

            // Get a reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = zedGraphControl1.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "Elevation: " + Path.GetFileNameWithoutExtension(vr.obs_filename);
            myPane.XAxis.Title.Text = "GPS Time";
            myPane.YAxis.Title.Text = "Degree";

            // Make up some data points based on the Sine function
            Random rd = new Random();
            for (int q = 0; q < vr.svss.Length; q++)
            {
                PointPairList list = new PointPairList();

                for (int p = 0; p < vr.obseph; p++)
                {
                    double x = (float)vr.obs_time[p];
                    double y = (float)vr.SkyDops[p, vr.svss[q] - 1, 1];

                    if (y != 0)
                        list.Add(x, y);
                }

                LineItem myCurve = myPane.AddCurve(vr.svss[q].ToString(), list, Color.FromArgb(rd.Next(256), rd.Next(256), rd.Next(256)), SymbolType.Circle);
                myCurve.Symbol.Size = 4;
                myCurve.Symbol.Fill.IsVisible = true;
                myCurve.Line.IsVisible = false;
            }

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;
            
            zedGraphControl1.AxisChange();
        }

        private void Plot_DOP(variables vr)
        {
            // DataGridView
            dataGridView1.Columns.Add("GPS Time", "GPS Time");
            dataGridView1.Columns.Add("GDOP", "GDOP");
            dataGridView1.Columns.Add("PDOP", "PDOP");
            dataGridView1.Columns.Add("HDOP", "HDOP");
            dataGridView1.Columns.Add("VDOP", "VDOP");
            dataGridView1.Columns.Add("TDOP", "TDOP");

            for (int p = 0; p < vr.obseph; p++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[p].Cells[0].Value = vr.obs_time[p];

                for (int q = 0; q < 5; q++)
                {
                    dataGridView1.Rows[p].Cells[q + 1].Value = vr.SkyDops[p, 32 + q, 0];
                }
            }

            // Get a reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = zedGraphControl1.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "DOP: " + Path.GetFileNameWithoutExtension(vr.obs_filename);
            myPane.XAxis.Title.Text = "GPS Time";
            myPane.YAxis.Title.Text = "";

            string[] DOPs = new string[5];
            DOPs[0] = "GDOP";
            DOPs[1] = "PDOP";
            DOPs[2] = "HDOP";
            DOPs[3] = "VDOP";
            DOPs[4] = "TDOP";

            // Make up some data points based on the Sine function
            Random rd = new Random();
            for (int q = 0; q < 5; q++)
            {
                PointPairList list = new PointPairList();

                for (int p = 0; p < vr.obseph; p++)
                {
                    double x = (float)vr.obs_time[p];
                    double y = (float)vr.SkyDops[p, 32 + q, 0];

                    if (y != 0)
                        list.Add(x, y);
                }

                LineItem myCurve = myPane.AddCurve(DOPs[q], list, Color.FromArgb(rd.Next(256), rd.Next(256), rd.Next(256)), SymbolType.Circle);
                myCurve.Symbol.Size = 4;
                myCurve.Symbol.Fill.IsVisible = true;
                myCurve.Line.IsVisible = false;
            }

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;

            zedGraphControl1.AxisChange();
        }

        private void Plot_ION(variables vr)
        {
            // DataGridView
            dataGridView_Setting(vr.svss);
            for (int p = 0; p < vr.obseph; p++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[p].Cells[0].Value = vr.obs_time[p];

                for (int q = 0; q < vr.svss.Length; q++)
                {
                    dataGridView1.Rows[p].Cells[q + 1].Value = vr.ion_mp[p, vr.svss[q] - 1, 0];
                }
            }

            // Get a reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = zedGraphControl1.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "ION: " + Path.GetFileNameWithoutExtension(vr.obs_filename);
            myPane.XAxis.Title.Text = "GPS Time";
            myPane.YAxis.Title.Text = "";
            // Manually set the axis range
            ParameterForm pForm = new ParameterForm();
            float[,] parameter = pForm.GetParameter;
            myPane.YAxis.Scale.Min = parameter[2, 0] * 2;
            myPane.YAxis.Scale.Max = parameter[2, 1] * 2;

            // Make up some data points based on the Sine function
            Random rd = new Random();
            for (int q = 0; q < vr.svss.Length; q++)
            {
                PointPairList list = new PointPairList();

                for (int p = 0; p < vr.obseph; p++)
                {
                    double x = (float)vr.obs_time[p];
                    double y = (float)vr.ion_mp[p, vr.svss[q] - 1, 0];

                    if (y != 0)
                        list.Add(x, y);
                }

                LineItem myCurve = myPane.AddCurve(vr.svss[q].ToString(), list, Color.FromArgb(rd.Next(256), rd.Next(256), rd.Next(256)), SymbolType.Circle);
                myCurve.Symbol.Size = 4;
                myCurve.Symbol.Fill.IsVisible = true;
                myCurve.Line.IsVisible = false;
            }

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;

            zedGraphControl1.AxisChange();
        }

        private void Plot_IOD(variables vr)
        {
            // DataGridView
            dataGridView_Setting(vr.svss);
            for (int p = 0; p < vr.obseph; p++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[p].Cells[0].Value = vr.obs_time[p];

                for (int q = 0; q < vr.svss.Length; q++)
                {
                    dataGridView1.Rows[p].Cells[q + 1].Value = vr.ion_mp[p, vr.svss[q] - 1, 1];
                }
            }

            // Get a reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = zedGraphControl1.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "IOD: " + Path.GetFileNameWithoutExtension(vr.obs_filename);
            myPane.XAxis.Title.Text = "GPS Time";
            myPane.YAxis.Title.Text = "";
            // Manually set the axis range
            ParameterForm pForm = new ParameterForm();
            float[,] parameter = pForm.GetParameter;
            myPane.YAxis.Scale.Min = parameter[3, 0] * 2;
            myPane.YAxis.Scale.Max = parameter[3, 1] * 2;

            // Make up some data points based on the Sine function
            Random rd = new Random();
            for (int q = 0; q < vr.svss.Length; q++)
            {
                PointPairList list = new PointPairList();

                for (int p = 0; p < vr.obseph; p++)
                {
                    double x = (float)vr.obs_time[p];
                    double y = (float)vr.ion_mp[p, vr.svss[q] - 1, 1];

                    if (y != 0)
                        list.Add(x, y);
                }

                LineItem myCurve = myPane.AddCurve(vr.svss[q].ToString(), list, Color.FromArgb(rd.Next(256), rd.Next(256), rd.Next(256)), SymbolType.Circle);
                myCurve.Symbol.Size = 4;
                myCurve.Symbol.Fill.IsVisible = true;
                myCurve.Line.IsVisible = false;
            }

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;

            zedGraphControl1.AxisChange();
        }

        private void Plot_MP1(variables vr)
        {
            // DataGridView
            dataGridView_Setting(vr.svss);
            for (int p = 0; p < vr.obseph; p++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[p].Cells[0].Value = vr.obs_time[p];

                for (int q = 0; q < vr.svss.Length; q++)
                {
                    dataGridView1.Rows[p].Cells[q + 1].Value = vr.ion_mp[p, vr.svss[q] - 1, 2];
                }
            }

            // Get a reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = zedGraphControl1.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "Multi-path L1: " + Path.GetFileNameWithoutExtension(vr.obs_filename);
            myPane.XAxis.Title.Text = "GPS Time";
            myPane.YAxis.Title.Text = "m";
            // Manually set the axis range
            ParameterForm pForm = new ParameterForm();
            float[,] parameter = pForm.GetParameter;
            myPane.YAxis.Scale.Min = parameter[4, 0] * 3;
            myPane.YAxis.Scale.Max = parameter[4, 1] * 3;

            // Make up some data points based on the Sine function
            Random rd = new Random();
            for (int q = 0; q < vr.svss.Length; q++)
            {
                PointPairList list = new PointPairList();

                for (int p = 0; p < vr.obseph; p++)
                {
                    double x = (float)vr.obs_time[p];
                    double y = (float)vr.ion_mp[p, vr.svss[q] - 1, 2];

                    if (y != 0)
                        list.Add(x, y);
                }

                LineItem myCurve = myPane.AddCurve(vr.svss[q].ToString(), list, Color.FromArgb(rd.Next(256), rd.Next(256), rd.Next(256)), SymbolType.Circle);
                myCurve.Symbol.Size = 4;
                myCurve.Symbol.Fill.IsVisible = true;
                myCurve.Line.IsVisible = false;
            }

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;

            zedGraphControl1.AxisChange();
        }

        private void Plot_MP2(variables vr)
        {
            // DataGridView
            dataGridView_Setting(vr.svss);
            for (int p = 0; p < vr.obseph; p++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[p].Cells[0].Value = vr.obs_time[p];

                for (int q = 0; q < vr.svss.Length; q++)
                {
                    dataGridView1.Rows[p].Cells[q + 1].Value = vr.ion_mp[p, vr.svss[q] - 1, 3];
                }
            }

            // Get a reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = zedGraphControl1.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "Multi-path L2: " + Path.GetFileNameWithoutExtension(vr.obs_filename);
            myPane.XAxis.Title.Text = "GPS Time";
            myPane.YAxis.Title.Text = "m";
            // Manually set the axis range
            ParameterForm pForm = new ParameterForm();
            float[,] parameter = pForm.GetParameter;
            myPane.YAxis.Scale.Min = parameter[5, 0] * 2;
            myPane.YAxis.Scale.Max = parameter[5, 1] * 2;

            // Make up some data points based on the Sine function
            Random rd = new Random();
            for (int q = 0; q < vr.svss.Length; q++)
            {
                PointPairList list = new PointPairList();

                for (int p = 0; p < vr.obseph; p++)
                {
                    double x = (float)vr.obs_time[p];
                    double y = (float)vr.ion_mp[p, vr.svss[q] - 1, 3];

                    if (y != 0)
                        list.Add(x, y);
                }

                LineItem myCurve = myPane.AddCurve(vr.svss[q].ToString(), list, Color.FromArgb(rd.Next(256), rd.Next(256), rd.Next(256)), SymbolType.Circle);
                myCurve.Symbol.Size = 4;
                myCurve.Symbol.Fill.IsVisible = true;
                myCurve.Line.IsVisible = false;
            }

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;

            zedGraphControl1.AxisChange();
        }

        private void Plot_CS1(variables vr)
        {
            // DataGridView
            dataGridView_Setting(vr.svss);
            for (int p = 0; p < vr.obseph - 2; p++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[p].Cells[0].Value = vr.obs_time[p];

                for (int q = 0; q < vr.svss.Length; q++)
                {
                    dataGridView1.Rows[p].Cells[q + 1].Value = vr.cycleslips[p, vr.svss[q] - 1, 2];
                }
            }

            // Get a reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = zedGraphControl1.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "Cycle-slip Code: " + Path.GetFileNameWithoutExtension(vr.obs_filename);
            myPane.XAxis.Title.Text = "GPS Time";
            myPane.YAxis.Title.Text = "";
            // Manually set the axis range
            ParameterForm pForm = new ParameterForm();
            float[,] parameter = pForm.GetParameter;
            myPane.YAxis.Scale.Min = parameter[6, 0] * 2;
            myPane.YAxis.Scale.Max = parameter[6, 1] * 2;

            // Make up some data points based on the Sine function
            Random rd = new Random();
            for (int q = 0; q < vr.svss.Length; q++)
            {
                PointPairList list = new PointPairList();

                for (int p = 0; p < vr.obseph - 2; p++)
                {
                    double x = (float)vr.obs_time[p];
                    double y = (float)vr.cycleslips[p, vr.svss[q] - 1, 2];
                    
                    if (y != 0)
                        list.Add(x, y);
                }

                LineItem myCurve = myPane.AddCurve(vr.svss[q].ToString(), list, Color.FromArgb(rd.Next(256), rd.Next(256), rd.Next(256)), SymbolType.Circle);
                myCurve.Symbol.Size = 4;
                myCurve.Symbol.Fill.IsVisible = true;
                myCurve.Line.IsVisible = false;
            }

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;

            zedGraphControl1.AxisChange();
        }

        private void Plot_CS2(variables vr)
        {
            // DataGridView
            dataGridView_Setting(vr.svss);
            for (int p = 0; p < vr.obseph - 2; p++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[p].Cells[0].Value = vr.obs_time[p];

                for (int q = 0; q < vr.svss.Length; q++)
                {
                    dataGridView1.Rows[p].Cells[q + 1].Value = vr.cycleslips[p, vr.svss[q] - 1, 3];
                }
            }

            // Get a reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = zedGraphControl1.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "Cycle-slip Carrier: " + Path.GetFileNameWithoutExtension(vr.obs_filename);
            myPane.XAxis.Title.Text = "GPS Time";
            myPane.YAxis.Title.Text = "";
            // Manually set the axis range
            ParameterForm pForm = new ParameterForm();
            float[,] parameter = pForm.GetParameter;
            myPane.YAxis.Scale.Min = parameter[7, 0] * 2;
            myPane.YAxis.Scale.Max = parameter[7, 1] * 2;

            // Make up some data points based on the Sine function
            Random rd = new Random();
            for (int q = 0; q < vr.svss.Length; q++)
            {
                PointPairList list = new PointPairList();

                for (int p = 0; p < vr.obseph - 2; p++)
                {
                    double x = (float)vr.obs_time[p];
                    double y = (float)vr.cycleslips[p, vr.svss[q] - 1, 3];

                    if (y != 0)
                        list.Add(x, y);
                }

                LineItem myCurve = myPane.AddCurve(vr.svss[q].ToString(), list, Color.FromArgb(rd.Next(256), rd.Next(256), rd.Next(256)), SymbolType.Circle);
                myCurve.Symbol.Size = 4;
                myCurve.Symbol.Fill.IsVisible = true;
                myCurve.Line.IsVisible = false;
            }

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;

            zedGraphControl1.AxisChange();
        }

        #endregion

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
