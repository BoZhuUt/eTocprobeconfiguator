using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using Modbus.Device;
using Nancy.Session;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Data.SqlTypes;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using eTocprobeconfiguator.Properties;
using Newtonsoft.Json;
using eTocprobeconfiguator.Para;
using System.Collections;
using ST58X_71XConfigurator;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;

namespace eTocprobeconfiguator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Color[] colors = new Color[] { Color.Red, Color.Orange, Color.DarkGreen, Color.Blue, Color.DarkBlue, Color.Purple, Color.DarkGreen, Color.DarkOrange, Color.AliceBlue, Color.Aqua, Color.DeepPink, Color.LightYellow };
        SerialPort port = new SerialPort();
        IModbusSerialMaster master;
        bool isPortExist = false;
        bool isProbe1Enable = false;
        bool isProbe2Enable = false;
        bool isProbe3Enable = false;
        bool ismA1Enable = false;
        bool isConnected=false;
        bool isRefreshing = false;
        byte proeb1Addr = 1;
        byte proeb2Addr = 1;
        byte proeb3Addr = 1;
        byte mA1Addr = 1;
        string probe1SN;
        string probe2SN;
        string probe3SN;
        string mA1SN;
        string measureValuePath_1;
        string measureValuePath_2;
        string measureValuePath_3;
        string measureValuePath_mA1;
        UInt16 readTimeoutProbe1 = 0;
        UInt16 readTimeoutProbe2 = 0;
        UInt16 readTimeoutMA1 = 0;
        void InitPort(bool prompt = true)
        {
            SearchPort(prompt);
            if (isPortExist)
            {
                Port_comboBox.SelectedIndex = 0;
            }
            Baud_comboBox.SelectedIndex = 5;
            Parity_comboBox.SelectedIndex = 2;
        }
        void SearchPort(bool prompt = true)
        {
            Port_comboBox.Items.Clear();
            string[] portnames = SerialPort.GetPortNames();
            if (portnames.Length != 0)
            {
                Port_comboBox.Items.AddRange(portnames);
                isPortExist = true;
            }
            else
            {
                isPortExist = false;
                if (prompt) { MessageBox.Show("Not Found Port!"); }
            }
        }
        void AddChartSeries(System.Windows.Forms.DataVisualization.Charting.Chart chart, string name, Color color, bool isShow = false)
        {
            chart.Series.Add(name);
            chart.Series[name].Enabled = isShow;
            chart.Series[name].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart.Series[name].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            chart.Series[name].IsValueShownAsLabel = false;
            chart.Series[name].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            chart.Series[name].Color = color;
            chart.Series[name].BorderWidth = 1;
            chart.Legends.Add(name);
            chart.Legends[name].BackColor = Color.FromArgb(0, Color.Transparent);
            chart.Legends[name].Font = new Font("Arial", 15);
            chart.Legends[name].DockedToChartArea = "mainChartArea";
            chart.Legends[name].IsDockedInsideChartArea = true;
        }
        private void LoadChart()
        {        
            string[] chartST72XTypes = new string[] { "conduction", "conductivity", "Res", "temperature" };
            string[] chartmATypes = new string[] { "mA1" };
            Measurement_Chart_1.Series.Clear();
            Measurement_Chart_1.Legends.Clear();
            ChartType_ComboBox_1.Items.Clear();
            string[] TempChart = new string[] { };
            Color[] TempColor = colors;
            Measurement_Chart_1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            Measurement_Chart_1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            Measurement_Chart_1.ChartAreas[0].Area3DStyle.Enable3D = false;
            TempChart = chartST72XTypes;  
            ChartType_ComboBox_1.Items.AddRange(TempChart);
            ChartType_ComboBox_1.SelectedIndex = 0;
            if (TempChart.Length == 0)
                return;
            for (int i = 0; i < TempChart.Length; i++)
            {
                if (i == 0)
                    AddChartSeries(Measurement_Chart_1, TempChart[i], TempColor[i], true);
                else
                    AddChartSeries(Measurement_Chart_1, TempChart[i], TempColor[i], false);
            }
            Measurement_Chart_2.Series.Clear();
            Measurement_Chart_2.Legends.Clear();
            ChartType_ComboBox_2.Items.Clear();
            TempColor = colors;
            Measurement_Chart_2.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            Measurement_Chart_2.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            Measurement_Chart_2.ChartAreas[0].Area3DStyle.Enable3D = false;
            TempChart = chartST72XTypes;
            ChartType_ComboBox_2.Items.AddRange(TempChart);
            ChartType_ComboBox_2.SelectedIndex = 0;
            if (TempChart.Length == 0)
                return;
            for (int i = 0; i < TempChart.Length; i++)
            {
                if (i == 0)
                    AddChartSeries(Measurement_Chart_2, TempChart[i], TempColor[i], true);
                else
                    AddChartSeries(Measurement_Chart_2, TempChart[i], TempColor[i], false);
            }
            Measurement_Chart_3.Series.Clear();
            Measurement_Chart_3.Legends.Clear();
            ChartType_ComboBox_3.Items.Clear();
            TempColor = colors;
            Measurement_Chart_3.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            Measurement_Chart_3.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            Measurement_Chart_3.ChartAreas[0].Area3DStyle.Enable3D = false;
            TempChart = chartmATypes;
            ChartType_ComboBox_3.Items.AddRange(TempChart);
            ChartType_ComboBox_3.SelectedIndex = 0;
            if (TempChart.Length == 0)
                return;
            for (int i = 0; i < TempChart.Length; i++)
            {
                if (i == 0)
                    AddChartSeries(Measurement_Chart_3, TempChart[i], TempColor[i], true);
                else
                    AddChartSeries(Measurement_Chart_3, TempChart[i], TempColor[i], false);
            }
            /*
            //测试图表显示用代码块
            for (int i = 0; i < 1000; i++)
            {
                DateTime CrtTime = DateTime.Now;
                Measurement_Chart_1.Series["conduction"].Points.AddXY(CrtTime, i);
            }
            */
        }
        string getCharType(ushort[] value)
        {
            char[] CharList = new char[value.Length * 2];
            for (int i = 0, j = 0; i < value.Length; i++)
            {
                CharList[j] = (char)(value[i] & 0x00FF); j++;
                CharList[j] = (char)(value[i] >> 8); j++;
            }
            return new string(CharList);
        }
        private Single ReturnFloatType(ushort data)
        {
            UInt32 tData = (UInt32)data;
            byte[] temp_bytes = BitConverter.GetBytes(tData);
            return BitConverter.ToSingle(temp_bytes, 0);
        }
        private Single ReturnFloatType(ushort data2, ushort data1)
        {
            UInt32 tData = ((UInt32)data2 << 16) + data1;
            byte[] temp_bytes = BitConverter.GetBytes(tData);
            return BitConverter.ToSingle(temp_bytes, 0);
        }
        private string ReturnUInt32Value(ushort data2, ushort data1)
        {
            UInt32 tData = ((UInt32)data2 << 16) + data1;
            return tData.ToString();
        }
        private string ReturnUInt16(ushort data)
        {
            byte[] temp_bytes = BitConverter.GetBytes(data);
            int value = BitConverter.ToInt16(temp_bytes, 0);
            return value.ToString();
        }

        List<PTSA_SystemItem> RegParaList = new List<PTSA_SystemItem>();
        List<PTSA_SystemItem> RegParaList2 = new List<PTSA_SystemItem>();
        getJsonParameter getJson = new getJsonParameter();
        OperateRegister rwReg = new OperateRegister();
        private async Task getProbeRegAndValue()
        {
            try
            {
                ReadParameterButton.Enabled = false;
                isRefreshing = true;
                if (isProbe1Enable)
                {
                    await Task.Delay(1000);
                    dataGridView1.DataSource = null;
                    dataGridView1.Invalidate();
                    RegParaList.Clear();
                    RegParaList = getJson.GetRegPara(Resources.parameTOC);
                    for (int i = 0; i < RegParaList.Count; i++)
                    {
                        string[] res = rwReg.parameterGettingContinuous(proeb1Addr, (ushort)(RegParaList[i].addr - 1), RegParaList[i].size, RegParaList[i].type, port);
                        if (res[0] == "Success")
                        {
                            RegParaList[i].value = res[1];
                        }
                        else
                        {
                            RegParaList[i].value = res[0]; continue;
                        }
                    }
                    dataGridView1.DataSource = RegParaList;
                    await Task.Delay(1000);
                    for (int i = 0; i < 4; i++)
                    {
                        dataGridView1.Columns[i].ReadOnly = true;
                        dataGridView1.Columns[i].DefaultCellStyle.ForeColor = Color.Blue;
                    }
                }
                //探头二参数表
                if (isProbe2Enable)
                {
                    await Task.Delay(1000);
                    dataGridView2.DataSource = null;
                    dataGridView2.Invalidate();
                    RegParaList2.Clear();
                    RegParaList2 = getJson.GetRegPara(Resources.parameTOC);
                    for (int i = 0; i < RegParaList2.Count; i++)
                    {
                        string[] res2 = rwReg.parameterGettingContinuous(proeb2Addr, (ushort)(RegParaList2[i].addr - 1), RegParaList2[i].size, RegParaList2[i].type, port);
                        if (res2[0] == "Success")
                        {
                            RegParaList2[i].value = res2[1];
                        }
                        else
                        {
                            RegParaList2[i].value = res2[0]; continue;
                        }
                    }
                    dataGridView2.DataSource = RegParaList2;
                    await Task.Delay(1000);
                    for (int i = 0; i < 4; i++)
                    {
                        dataGridView2.Columns[i].ReadOnly = true;
                        dataGridView2.Columns[i].DefaultCellStyle.ForeColor = Color.Blue;
                    }
                }
                isRefreshing = false;
                ReadParameterButton.Enabled = true;
                if (port.IsOpen)
                    port.Close();
            }
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message);
            }
        }
        Hashtable regChangeHash = new Hashtable();
        Hashtable regChangeHash2 = new Hashtable();
        private void probeRegMap_DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.Red;
            var value = dataGridView1[e.ColumnIndex, e.RowIndex].Value;
            if (regChangeHash.Contains(e.RowIndex))
            {
                if (value != null)
                    regChangeHash[e.RowIndex] = value.ToString();
                else
                    MessageBox.Show($"The value of the {dataGridView1[0, e.RowIndex].Value} cannot be null");
            }
            else
            {
                if (value != null)
                    regChangeHash.Add(e.RowIndex, value.ToString());
                else
                    MessageBox.Show($"The value of the {dataGridView1[0, e.RowIndex].Value} cannot be null");
            }
        }
        private void probeRegMap_DataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView2[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.Red;
            var value = dataGridView2[e.ColumnIndex, e.RowIndex].Value;
            if (regChangeHash2.Contains(e.RowIndex))
            {
                if (value != null)
                    regChangeHash2[e.RowIndex] = value.ToString();
                else
                    MessageBox.Show($"The value of the {dataGridView2[0, e.RowIndex].Value} cannot be null");
            }
            else
            {
                if (value != null)
                    regChangeHash2.Add(e.RowIndex, value.ToString());
                else
                    MessageBox.Show($"The value of the {dataGridView2[0, e.RowIndex].Value} cannot be null");
            }
        }
        private async Task setProbeRegAndValue()
        {
            string result = "Success";
            List<PTSA_SystemItem> regChange = new List<PTSA_SystemItem>();
            ArrayList position = new ArrayList();
            PTSA_SystemItem temp = new PTSA_SystemItem();
            WriteParameterButton.Enabled = false;
            if (isProbe1Enable)
            {
                foreach (DictionaryEntry item in regChangeHash)
                {
                    regChange.Add(RegParaList[(int)item.Key]);
                    position.Add((int)item.Key);
                }
                for (int i = 0; i < regChange.Count && result == "Success"; i++)
                {
                    temp = regChange[i];
                    try
                    {
                        result = rwReg.parameterSetting(proeb1Addr, (ushort)(temp.addr - 1), (byte)temp.size, temp.value.Replace(" ", ""), temp.type, port);
                        if (result == "Success")
                        {
                            dataGridView1[4, (int)position[i]].Style.ForeColor = Color.Black;
                        }
                        else
                        {
                            MessageBox.Show(result);
                            break;
                        }

                    }
                    catch (FormatException)
                    {
                        MessageBox.Show($"{dataGridView1[0, (int)position[i]].Value} parameter erro");
                        break;
                    }
                }
            }
            if (isProbe2Enable)
            {
                await Task.Delay(50);
                foreach (DictionaryEntry item in regChangeHash2)
                {
                    regChange.Add(RegParaList2[(int)item.Key]);
                    position.Add((int)item.Key);
                }
                for (int i = 0; i < regChange.Count && result == "Success"; i++)
                {
                    temp = regChange[i];
                    try
                    {
                        result = rwReg.parameterSetting(proeb2Addr, (ushort)(temp.addr - 1), (byte)temp.size, temp.value.Replace(" ", ""), temp.type, port);
                        if (result == "Success")
                        {
                            dataGridView2[4, (int)position[i]].Style.ForeColor = Color.Black;
                        }
                        else
                        {
                            MessageBox.Show(result);
                            break;
                        }

                    }
                    catch (FormatException)
                    {
                        MessageBox.Show($"{dataGridView2[0, (int)position[i]].Value} parameter erro");
                        break;
                    }
                }
            }
            WriteParameterButton.Enabled = true;
            //Thread.Sleep(50);
            //StatusLabel.Text = "...";
            //Thread.CurrentThread.Abort();
        }
        private void dataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        { 
            e.Cancel = true;
        }
        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        EEEECondData cddata;
        async void GetData(Chart chart1, Chart chart2)
        {
            if (!port.IsOpen)
                port.Open();
            byte[] buffer = new byte[4096];
            try
            {
                port.ReadExisting();
                int timeout = 500;
                while (timeout > 0)
                {
                    if (port.BytesToRead >= 2048)
                    {
                        break;
                    }
                    await Task.Delay(10);
                    timeout--;
                }
                if (port.BytesToRead >= 2048)
                {
                    int len = port.Read(buffer, 0, 2048);
                    cddata = new EEEECondData(buffer);
                    chart1.Series[0].Points.Clear();
                    chart1.Series[1].Points.Clear();
                    //chart2.Series[0].Points.Clear();
                    //chart2.Series[1].Points.Clear();
                    for (int i = 0; i < 1024; i++)
                    {
                        chart1.Series[0].Points.AddXY(i, cddata.CondIWave[i]);
                        // chart1.Series[1].Points.AddXY(i, cddata.CondVWave[i]);

                        //chart2.Series[0].Points.AddXY(i, cddata.CondIWave[i]);
                        //chart2.Series[1].Points.AddXY(i, cddata.CondVWave[i]);
                    }
                    chart2.Series[0].Points.Clear();
                    chart2.Series[1].Points.Clear();
                    for (int i = 0; i < 192; i++)
                    {
                        chart2.Series[0].Points.AddXY(i, cddata.CondIWave[i]);
                    }
                    MessageBox.Show("Get OK");
                }
                else
                {
                    MessageBox.Show("error pkg len = " + port.BytesToRead.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Search_Btn_Click(object sender, EventArgs e)
        {
            InitPort();
        }

        private void button_probeaddr1Enable_Click(object sender, EventArgs e)
        {
            try
            {
                if (button_probeaddr1Enable.Text == "Enable")
                {
                    isProbe1Enable = true;
                    proeb1Addr = Convert.ToByte(textBox_probeaddr1.Text);
                    button_probeaddr1Enable.Text = "Disable";
                }
                else
                {
                    isProbe1Enable = false;
                    button_probeaddr1Enable.Text = "Enable";
                }
            }
            catch 
            {
                MessageBox.Show("请输入正确的地址");
            }

        }

        private void button_probeaddr2Enable_Click(object sender, EventArgs e)
        {
            try
            {
                if (button_probeaddr2Enable.Text == "Enable")
                {
                    isProbe2Enable = true;
                    proeb2Addr = Convert.ToByte(textBox_probeaddr2.Text);
                    button_probeaddr2Enable.Text = "Disable";
                }
                else
                {
                    isProbe2Enable = false;
                    button_probeaddr2Enable.Text = "Enable";
                }
            }
            catch
            {
                MessageBox.Show("请输入正确的地址");
            }
        }

        private void button_probeaddr3Enable_Click(object sender, EventArgs e)
        {

        }
        private void buttonmAaddr1Enable_Click(object sender, EventArgs e)
        {
            try
            {
                if (buttonmAaddr1Enable.Text == "Enable")
                {
                    ismA1Enable = true;
                    mA1Addr = Convert.ToByte(textBox_mAaddr1.Text);
                    buttonmAaddr1Enable.Text = "Disable";
                }
                else
                {
                    ismA1Enable = false;
                    buttonmAaddr1Enable.Text = "Enable";
                }
            }
            catch
            {
                MessageBox.Show("请输入正确的地址");
            }
        }

        private async void Connect_Btn_ClickAsync(object sender, EventArgs e)
        {
            uint count = 0;
            if (Connect_Btn.Text == "Connect")
            {
                while (Connect_Btn.Text == "Connect" && ++count < 3)
                {
                    if (port.IsOpen) port.Close();
                    if (Port_comboBox.Text != "")
                    {
                        port.PortName = Port_comboBox.Text;
                        port.BaudRate = int.Parse(Baud_comboBox.Text);
                        if (Parity_comboBox.Text == "ODD")
                            port.Parity = Parity.Odd;
                        else if (Parity_comboBox.Text == "EVEN")
                            port.Parity = Parity.Even;
                        else
                            port.Parity = Parity.None;
                        try
                        {
                            if (!port.IsOpen)
                                port.Open();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return;
                        }
                        master = ModbusSerialMaster.CreateRtu(port);
                        master.Transport.ReadTimeout = 1000;
                        master.Transport.WriteTimeout = 1000;
                        master.Transport.Retries = 0;
                        try
                        {
                            if (isProbe1Enable == true)
                            {                         
                                ushort[] serialnum = master.ReadHoldingRegisters(proeb1Addr, (ushort)(41015 - 1), 8);
                                string snString = getCharType(serialnum);
                                label_probe1SN.Text= "probe1 SN : "+snString;
                                probe1SN = snString;
                                measureValuePath_1 = probe1SN.Replace("\0", "") + "_measureValue.csv";
                            }
                            if (isProbe2Enable == true)
                            {
                                ushort[] serialnum = master.ReadHoldingRegisters(proeb2Addr, (ushort)(41015 - 1), 8);
                                string snString = getCharType(serialnum);
                                label_probe2SN.Text = "probe2 SN : " + snString;
                                probe2SN = snString;
                                measureValuePath_2 = probe2SN.Replace("\0", "") + "_measureValue.csv";
                            }
                            if (isProbe3Enable == true)
                            {
                                ushort[] serialnum = master.ReadHoldingRegisters(proeb3Addr, (ushort)(41015 - 1), 8);
                                string snString = getCharType(serialnum);
                                label_probe3SN.Text = "probe3 SN : " + snString;
                                probe3SN = snString;
                            }
                            if (ismA1Enable == true)
                            {
                                //ushort[] serialnum = master.ReadHoldingRegisters(mA1Addr, (ushort)(41015 - 1), 2);
                                //string snString = getCharType(serialnum);
                                label_mA1SN.Text = "mA1 SN : " + "0001";
                                mA1SN ="001";
                                measureValuePath_mA1 = " MA1__measureValue.csv";
                            }

                            isConnected= true;
                            Connect_Btn.Text = "Disconnect";

                        }
                        catch
                        {
                            MessageBox.Show("Failed!!!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("No probe available!", "Error");
                    }
                }
                //if (count >= 3)
                //{
                //    count = 0;
                //    MessageBox.Show("No probe available!");
                //}
            }
            else
            {
                isConnected= false;
                Connect_Btn.Text = "Connect";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadChart();
        }

        private void Read_Btn_Click(object sender, EventArgs e)
        {
            MeasurementTimer.Interval = 2 * 1000;
           // measureValuePath_3 = label_probe3SN + "_measureValue.csv";
            try
            {
                if ((!File.Exists(measureValuePath_1)) && (isProbe1Enable == true))
                {
                    File.WriteAllText(measureValuePath_1, "date,Res,conduction\r\n");
                }
                if ((!File.Exists(measureValuePath_2)) && (isProbe2Enable == true))
                {
                    File.WriteAllText(measureValuePath_2, "date,Res,conduction\r\n");
                }
                if ((!File.Exists(measureValuePath_mA1)) && (ismA1Enable == true))
                {
                    File.WriteAllText(measureValuePath_mA1, "date,Res,conduction\r\n");
                }

                //if (!File.Exists(measureValuePath_3))
                //{
                //    File.WriteAllText(measureValuePath_3, "date,measureValue,temperature,t365,s365,t420,s420\r\n");
                //}
            }
            catch
            {
                MessageBox.Show("Read Error !");
            }
            if (Read_Btn.Text == "Read")
            {
                MeasurementTimer.Enabled = true;
                MeasurementTimer.Start();
                Read_Btn.Text = "Stop";
            }
            else
            {
                Read_Btn.Text = "Read";
                MeasurementTimer.Enabled = false;
                MeasurementTimer.Stop();
            }

        }

        private void MeasurementTimer_Tick(object sender, EventArgs e)
        {
            Single conduction1 = 0.0F, conduction2 = 0.0F;
            Single conductivity = 0.0F;
            Single Res1 = 0.0F, Res2 = 0.0F, Res3 = 0.0F;
            Single mA1 = 0.0F;
            if (!isPortExist)
            {
                InitPort(false);
                MeasurementTimer.Enabled = false;
                Connection_GroupBox.Enabled = true;
            }
            else
            {
                 DateTime CrtTime = DateTime.Now;
                try
                {
                    if (!port.IsOpen)
                    {
                        port.Open();
                    }
                    if (isProbe1Enable)
                    {
                        try
                        {                            
                            ushort[] data1 = master.ReadHoldingRegisters(proeb1Addr, 46001 - 1, 50);
                            Res1 = ReturnFloatType(data1[40], data1[39]);
                            conduction1 = 1 / Res1 * 1000000;
                            probe1Res.Text = "Res1:  " +  Res1.ToString("F1");
                            probe1Condction.Text = "conduction1: " + conduction1.ToString("F2");
                            Measurement_Chart_1.Series["Res"].Points.AddXY(CrtTime, Res1);
                            Measurement_Chart_1.Series["conduction"].Points.AddXY(CrtTime, conduction1);
                            readTimeoutProbe1 = 0; 
                        }
                        catch (Exception ex) 
                        {
                            readTimeoutProbe1++;
                            if (readTimeoutProbe1 >= 4) 
                            {
                                isProbe1Enable = false;
                                MessageBox.Show("Probe1 Error , please confirm !");
                            }
                        }
                            
                        }
                    if (isProbe2Enable)
                    {
                        try
                        {
                            ushort[] data2 = master.ReadHoldingRegisters(proeb2Addr, 46001 - 1, 50);
                            Res2 = ReturnFloatType(data2[40], data2[39]);
                            conduction2 = 1 / Res2 * 1000000;
                            probe2Res.Text = "Res2:  " + Res2.ToString("F1");
                            probe2Conduction.Text = "conduction2: " + conduction2.ToString("F2");
                            Measurement_Chart_2.Series["Res"].Points.AddXY(CrtTime, Res2);
                            Measurement_Chart_2.Series["conduction"].Points.AddXY(CrtTime, conduction2);
                            readTimeoutProbe2 = 0;
                        }
                        catch (Exception ex) 
                        {
                            readTimeoutProbe2++;
                            if (readTimeoutProbe2 >= 4)
                            {
                                isProbe2Enable = false;
                                MessageBox.Show("Probe2 Error , please confirm !");
                            }
                        }                         
                     }

                    if (isProbe3Enable)
                    {
                        ushort[] data3 = master.ReadHoldingRegisters(proeb3Addr, 46001 - 1, 28);
                        //readTimeout = 0;

                    }
                    if (ismA1Enable)
                    {
                        try
                        {
                            ushort[] data4 = master.ReadInputRegisters(mA1Addr, 1 - 1, 2);
                            mA1 = (float)Math.Pow(10 , - (int)(data4[0] / 10000));
                            mA1 = (data4[0] % 10000 ) * mA1 / 249 * 1000 ;
                            label_MA1.Text ="mA: " + mA1.ToString("F3");
                            Measurement_Chart_3.Series["mA1"].Points.AddXY(CrtTime, mA1);
                            readTimeoutMA1 = 0;
                        }
                        catch (Exception ex)
                        {
                            readTimeoutMA1++;
                            if (readTimeoutMA1 >= 40000)
                            {
                                ismA1Enable = false;
                                MessageBox.Show("mA Error , please confirm !");
                            }

                        }
                        if ((readTimeoutProbe1 == 0) && (readTimeoutProbe2 == 0) )
                        {
                            if (isProbe1Enable)
                            {
                                File.AppendAllText(measureValuePath_1, CrtTime.ToString() + "," + Res1.ToString("F2") + "," + conduction1.ToString("F2") + "\r\n");
                            }
                            if (isProbe2Enable)
                            {
                                File.AppendAllText(measureValuePath_2, CrtTime.ToString() + "," + Res2.ToString("F2") + "," + conduction2.ToString("F2") + "\r\n");
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MeasurementTimer.Enabled = false;
                    MessageBox.Show("Read Error !");
                }
            }
        }

        private void ChartType_ComboBox_1_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var item in Measurement_Chart_1.Series)
            {
                if (ChartType_ComboBox_1.Text.ToLower() == item.Name.ToLower())
                    item.Enabled = true;
                else
                    item.Enabled = false;
            }
        }

        private void ChartType_ComboBox_2_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var item in Measurement_Chart_2.Series)
            {
                if (ChartType_ComboBox_2.Text.ToLower() == item.Name.ToLower())
                    item.Enabled = true;
                else
                    item.Enabled = false;
            }
        }

        private async void ReadParameterButton_Click(object sender, EventArgs e)
        {
            await Task.Delay(500);
            await getProbeRegAndValue();
        }

        private async void WriteParameterButton_Click(object sender, EventArgs e)
        {
            await Task.Delay(500);
            await setProbeRegAndValue();
        }

        private async void readWavrButton_Click(object sender, EventArgs e)
        {
            string status;
            status = rwReg.parameterSetting(proeb1Addr, 44004 - 1, 1, 1001, "uint16", port);
            readWavrButton.Enabled = false;
            GetData(rawDatachart1, rawDatachart1);
            await Task.Delay(500);
            readWavrButton.Enabled = true;
        }

        private async void readWavrButton2_Click(object sender, EventArgs e)
        {
            string status;
            readWavrButton2.Enabled = false;
            status = rwReg.parameterSetting(proeb2Addr, 44004 - 1, 1, 1001, "uint16", port);
            await Task.Delay(500);
            GetData(rawDatachart2, rawDatachart2);
            readWavrButton2.Enabled = true;
        }
    }
}
