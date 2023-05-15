using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modbus.Device;
using Nancy.Json;

namespace eTocprobeconfiguator.Para
{
    class getProbePNAddr
    {
        public List<ProbeItem> getProbesAddr(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            ProbePNAddr addrList = Serializer.Deserialize<ProbePNAddr>(jsonString);
            List<ProbeItem> probesAddr = addrList.Probes;
            return probesAddr;
        }
    }
    class getJsonParameter
    {
        public string Id;
        public string Date;
        public string Major;
        ArrayList RegisterList = new ArrayList();
        public List<PTSA_SystemItem> GetRegPara(byte[] JsonData)
        {
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            List<PTSA_SystemItem> parameters = new List<PTSA_SystemItem>();
            try
            {
                string jsonString = System.Text.Encoding.UTF8.GetString(JsonData);
                RegistersParameter registerList = Serializer.Deserialize<RegistersParameter>(jsonString);
                parameters = registerList.ParameterTableFile.Parameters.ptsa_system;
                Id = registerList.ParameterTableFile.id;
                Date = registerList.ParameterTableFile.data;
                Major = registerList.ParameterTableFile.major.ToString();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            return parameters;
        }
        public ArrayList GetProbeRegister(string fileAddr)
        {
            string jsonText = File.ReadAllText(fileAddr);
            JsonReader reader = new JsonTextReader(new StringReader(jsonText));
            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    if (reader.Value.ToString().Equals("id"))
                    {
                        reader.Read();
                        Id = reader.Value.ToString();
                    }
                    else if (reader.Value.ToString().Equals("date"))
                    {
                        reader.Read();
                        Date = reader.Value.ToString();
                    }
                    else if (reader.Value.ToString().Equals("major"))
                    {
                        reader.Read();
                        Major = reader.Value.ToString();
                    }
                    else if (reader.Value.ToString().Equals("name"))
                    {
                        ProbeRegister probeReg = new ProbeRegister();
                        reader.Read();
                        probeReg.Name = reader.Value.ToString();
                        reader.Read(); reader.Read();
                        probeReg.Addr = Convert.ToUInt32(reader.Value);
                        reader.Read(); reader.Read();
                        probeReg.Size = Convert.ToUInt32(reader.Value);
                        reader.Read(); reader.Read();
                        probeReg.Type = reader.Value.ToString();
                        RegisterList.Add(probeReg);

                    }
                }
            }
            return RegisterList;
        }
        public List<List<ProbesItem>> GetProbeTestDatas(byte[] JsonData)
        {
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            List<List<ProbesItem>> datas = new List<List<ProbesItem>>();
            try
            {
                string jsonString = System.Text.Encoding.UTF8.GetString(JsonData);
                ProbeTestData probesDatas = Serializer.Deserialize<ProbeTestData>(jsonString);
                datas = probesDatas.Probes;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
            return datas;
        }
        //public TokenParam GetTokenParameters(string jsonStr)   --从刘李耀处复制，暂时不需要HTTP服务，特屏蔽   2023.05.15 Bo
        //{
        //    JavaScriptSerializer Serializer = new JavaScriptSerializer();
        //    try
        //    {
        //        TokenParam tokenParam = Serializer.Deserialize<TokenParam>(jsonStr);
        //        return tokenParam;
        //    }
        //    catch (Exception)
        //    {
        //        return new TokenParam();
        //    }
        //}
    }
    class OperateRegister
    {
        public string[] parameterGettingContinuous(byte addr, UInt16 reg_addr, byte reg_lenth, string type, SerialPort port)
        {
            string[] result = { "Success", "Success" };
            IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);
            master.Transport.ReadTimeout = 1000;
            master.Transport.WriteTimeout = 1000;
            if (!port.IsOpen)
            {
                try
                {
                    port.Open();
                }
                catch (InvalidOperationException)
                {
                    result[0] = "Error";
                    result[1] = "Port was open";
                }
                catch (UnauthorizedAccessException)
                {
                    result[0] = "Erro";
                    result[1] = "Port was open";
                }
                catch (IOException)
                {
                    result[0] = "Erro";
                    result[1] = "Port was not found";
                }
            }
            if (result[0] == "Success")
            {
                if (type == "uint16")
                {
                    try
                    {
                        ushort[] data = master.ReadHoldingRegisters(addr, reg_addr, 1);
                        result[1] = data[0].ToString();
                    }
                    catch (InvalidOperationException)
                    {
                        result[0] = "Erro";
                        result[1] = "Port was not found";
                    }
                    catch (TimeoutException)
                    {
                        result[0] = "Erro";
                        result[1] = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result[0] = "Erro";
                        result[1] = $"Unknown exception : {ex.SlaveExceptionCode}";
                    }
                    catch (Exception ex)
                    {
                        result[0] = "Erro";
                        result[1] = $"Unknown exception : {ex.Message}";
                    }
                }
                else if (type == "int16")
                {
                    try
                    {
                        ushort[] data = master.ReadHoldingRegisters(addr, reg_addr, 1);
                        byte[] temp_bytes = BitConverter.GetBytes(data[0]);
                        int temp_int16 = BitConverter.ToInt16(temp_bytes, 0);
                        result[1] = temp_int16.ToString();
                    }
                    catch (InvalidOperationException)
                    {
                        result[0] = "Erro";
                        result[1] = "Port was not found";
                    }
                    catch (TimeoutException)
                    {
                        result[0] = "Erro";
                        result[1] = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result[0] = "Erro";
                        result[1] = $"Unknown exception : {ex.SlaveExceptionCode}";
                    }
                    catch (Exception ex)
                    {
                        result[0] = "Erro";
                        result[1] = $"Unknown exception : {ex.Message}";
                    }
                }
                else if (type == "uint32")
                {
                    try
                    {
                        ushort[] data = master.ReadHoldingRegisters(addr, reg_addr, 2);
                        UInt32 tData = ((UInt32)data[1] << 16) + data[0];
                        result[1] = tData.ToString();
                    }
                    catch (InvalidOperationException)
                    {
                        result[0] = "Erro";
                        result[1] = "Port was not found";
                    }
                    catch (TimeoutException)
                    {
                        result[0] = "Erro";
                        result[1] = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result[0] = "Erro";
                        result[1] = $"Unknown exception : {ex.SlaveExceptionCode}";
                    }
                    catch (Exception ex)
                    {
                        result[0] = "Erro";
                        result[1] = $"Unknown exception : {ex.Message}";
                    }
                }
                else if (type == "float")
                {
                    try
                    {
                        ushort[] data = master.ReadHoldingRegisters(addr, reg_addr, 2);
                        UInt32 tData = ((UInt32)data[1] << 16) + data[0];
                        byte[] temp_bytes = BitConverter.GetBytes(tData);
                        Single temp_float = BitConverter.ToSingle(temp_bytes, 0);
                        result[1] = temp_float.ToString("N4");
                    }
                    catch (InvalidOperationException)
                    {
                        result[0] = "Erro";
                        result[1] = "Port was not found";
                    }
                    catch (TimeoutException)
                    {
                        result[0] = "Erro";
                        result[1] = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result[0] = "Erro";
                        result[1] = $"Unknown exception : {ex.SlaveExceptionCode}";
                    }
                    catch (Exception ex)
                    {
                        result[0] = "Erro";
                        result[1] = $"Unknown exception : {ex.Message}";
                    }
                }
                else if (type == "char")
                {
                    try
                    {
                        ushort[] data = master.ReadHoldingRegisters(addr, reg_addr, reg_lenth);
                        char[] arrayChar = new char[data.Length * 2];
                        for (int i = 0, j = 0; i < data.Length; i++)
                        {
                            arrayChar[j] = (char)(data[i] & 0x00FF); j++;
                            arrayChar[j] = (char)(data[i] >> 8); j++;
                        }
                        result[1] = new string(arrayChar);
                    }
                    catch (InvalidOperationException)
                    {
                        result[0] = "Erro";
                        result[1] = "Port was not found";
                    }
                    catch (TimeoutException)
                    {
                        result[0] = "Erro";
                        result[1] = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result[0] = "Erro";
                        result[1] = $"Unknown exception : {ex.SlaveExceptionCode}";
                    }
                    catch (Exception ex)
                    {
                        result[0] = "Erro";
                        result[1] = $"Unknown exception : {ex.Message}";
                    }
                }
                else
                {
                    result[0] = "Erro";
                    result[1] = $"Unknown type : {reg_addr},{type}";
                }
            }
            return result;
        }
        public string parameterSetting(byte addr, UInt16 reg_addr, byte reg_lenth, UInt32 value, string type, SerialPort port)
        {
            string result = "Success";
            IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);
            master.Transport.ReadTimeout = 1000;
            master.Transport.WriteTimeout = 1000;
            if (!port.IsOpen)
            {
                try
                {
                    port.Open();
                }
                catch (InvalidOperationException)
                {
                    result = "Port was open";
                }
                catch (UnauthorizedAccessException)
                {
                    result = "Port was open";
                }
                catch (IOException)
                {
                    result = "Port was not found";
                }
            }
            if (result == "Success")
            {
                if (type == "uint16")
                {
                    try
                    {
                        master.WriteSingleRegister(addr, reg_addr, (UInt16)value);
                        //if (ST58X71XProbeConfigurator.ifSaveCommand)
                        //    master.WriteSingleRegister(addr, 1044 - 1, 2);
                    }
                    catch (InvalidOperationException)
                    {
                        result = "Port was not found";
                    }
                    catch (TimeoutException)
                    {
                        result = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result = $"Unknown exception : {ex.SlaveExceptionCode}";
                    }
                }
                else if (type == "uint32")
                {
                    try
                    {
                        ushort[] data = { (ushort)value, (ushort)(value >> 16) };
                        master.WriteMultipleRegisters(addr, reg_addr, data);
                        //if (ST58X71XProbeConfigurator.ifSaveCommand)
                        //    master.WriteSingleRegister(addr, 1044 - 1, 2);
                    }
                    catch (InvalidOperationException)
                    {
                        result = "Port was not found";
                    }
                    catch (TimeoutException)
                    {
                        result = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result = $"Unknown exception : {ex.SlaveExceptionCode}";
                    }
                }
            }
            port.Close();
            master.Dispose();
            return result;
        }
        public string parameterSetting(byte addr, UInt16 reg_addr, byte reg_lenth, Single value, string type, SerialPort port)
        {
            string result = "Success";
            IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);
            master.Transport.ReadTimeout = 1000;
            master.Transport.WriteTimeout = 1000;
            if (!port.IsOpen)
            {
                try
                {
                    port.Open();
                }
                catch (InvalidOperationException)
                {
                    result = "Port was open";
                }
                catch (UnauthorizedAccessException)
                {
                    result = "Port was open";
                }
                catch (IOException)
                {
                    result = "Port was not found";
                }
            }
            if (result == "Success")
            {
                try
                {
                    byte[] temp_bytes = BitConverter.GetBytes(value);
                    UInt32 temp_int = 0;
                    for (int i = temp_bytes.Length - 1; i >= 0; i--)
                    {
                        temp_int += (uint)temp_bytes[i] << (i * 8);
                    }
                    ushort[] data = { (ushort)temp_int, (ushort)(temp_int >> 16) };
                    master.WriteMultipleRegisters(addr, reg_addr, data);
                    //if (ST58X71XProbeConfigurator.ifSaveCommand)
                    //    master.WriteSingleRegister(addr, 1044 - 1, 2);
                }
                catch (InvalidOperationException)
                {
                    result = "Port was not found";
                }
                catch (TimeoutException)
                {
                    result = "Time out";
                }
                catch (Modbus.SlaveException ex)
                {
                    result = $"Unknown exception : {ex.SlaveExceptionCode}";
                }
            }
            port.Close();
            master.Dispose();
            return result;
        }
        public string parameterSetting(byte addr, UInt16 reg_addr, byte reg_lenth, String value, string type, SerialPort port)
        {
            string result = "Success";
            IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);
            master.Transport.ReadTimeout = 1000;
            master.Transport.WriteTimeout = 1000;
            if (!port.IsOpen)
            {
                try
                {
                    port.Open();
                }
                catch (InvalidOperationException)
                {
                    result = "port was open";
                }
                catch (UnauthorizedAccessException)
                {
                    result = "port was open";
                }
                catch (IOException)
                {
                    result = "port was not found";
                }
            }
            if (result == "Success")
            {
                if ((type == "uint16") && (reg_lenth == 1))
                {
                    try
                    {
                        master.WriteSingleRegister(addr, reg_addr, Convert.ToUInt16(value));
                        //if (ST58X71XProbeConfigurator.ifSaveCommand)
                        //    master.WriteSingleRegister(addr, 1044 - 1, 2);
                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                    }
                }
                else if ((type == "int16") && (reg_lenth == 1))
                {
                    try
                    {
                        Int16 Value = Convert.ToInt16(value);
                        byte[] temp_bytes = BitConverter.GetBytes(Value);
                        UInt16 temp_int = 0;
                        for (int i = 1; i >= 0; i--)
                        {
                            temp_int += Convert.ToUInt16(temp_bytes[i] << (i * 8));
                        }
                        master.WriteSingleRegister(addr, reg_addr, temp_int);
                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                    }
                }
                else if ((type == "uint32") && (reg_lenth == 2))
                {
                    try
                    {
                        UInt32 Value = Convert.ToUInt32(value);
                        ushort[] data = { (ushort)Value, (ushort)(Value >> 16) };
                        master.WriteMultipleRegisters(addr, reg_addr, data);
                    }
                    catch (InvalidOperationException)
                    {
                        result = "Port was not found";
                    }
                    catch (TimeoutException)
                    {
                        result = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result = $"Unknown exception : {ex.SlaveExceptionCode}";
                    }
                }
                else if ((type.ToLower() == "float") && (reg_lenth == 2))
                {
                    try
                    {
                        float Value = float.Parse(value);
                        byte[] temp_bytes = BitConverter.GetBytes(Value);
                        UInt32 temp_int = 0;
                        for (int i = temp_bytes.Length - 1; i >= 0; i--)
                        {
                            temp_int += (uint)temp_bytes[i] << (i * 8);
                        }
                        ushort[] data = { (ushort)temp_int, (ushort)(temp_int >> 16) };
                        master.WriteMultipleRegisters(addr, reg_addr, data);
                    }
                    catch (InvalidOperationException)
                    {
                        result = "Port was not found";
                    }
                    catch (TimeoutException)
                    {
                        result = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result = $"Unknown exception : {ex.SlaveExceptionCode}";
                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                    }
                }
                else if ((type.ToLower() == "char") && ((reg_lenth == 8) || (reg_lenth == 16)))
                {
                    try
                    {
                        if (value.Length > 16)
                        {
                            result = "String length erro";
                            return result;
                        }
                        //ushort[] data = new ushort[value.Length / 2 + 1];
                        ushort[] data = new ushort[reg_lenth];
                        int res = value.Length % 2;
                        for (int i = 0, j = 0; i < value.Length / 2; i++)
                        {
                            data[i] = (ushort)(value[j + 1] << 8 | value[j]);
                            j += 2;
                        }
                        if (res != 0)
                            data[value.Length / 2] = value[value.Length - 1];
                        master.WriteMultipleRegisters(addr, reg_addr, data);
                    }
                    catch (InvalidOperationException)
                    {
                        result = "Port was not found";
                    }
                    catch (TimeoutException)
                    {
                        result = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result = $"Unknown exception : {ex.SlaveExceptionCode}";
                    }
                }
                else
                {
                    result = $"type Erro : {type}";
                }
            }
            port.Close();
            master.Dispose();
            return result;
        }
    }
    class TargetConfig
    {
        SerialPort COM_485_Port = new SerialPort();
        private string portName_485;
        private int baudRate_485;
        private Parity parity_485;
        public byte Sequence_Number = 0;
        public void COM_485_Config(string portname, int baudrate, Parity parity)
        {
            COM_485_Port.ReadTimeout = 1000;
            portName_485 = portname;
            baudRate_485 = baudrate;
            parity_485 = parity;
            COM_485_Port.ReceivedBytesThreshold = 1;
        }
    }
    public static class RegisterAsync
    {
        public static async Task<ushort[]> ReadRegisters(this IModbusSerialMaster master, byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ushort[] data = await master.ReadHoldingRegistersAsync(slaveAddress, startAddress, numberOfPoints);
            await Task.Delay(50);
            return data;
        }
    }
    public static class OperationPort
    {

        public static string OpenMessage(this SerialPort port)
        {
            string result = "Success";
            if (!port.IsOpen)
            {
                try
                {
                    port.Open();
                }
                catch (InvalidOperationException)
                {
                    result = "Port was open";
                }
                catch (UnauthorizedAccessException)
                {
                    result = "Port was open";
                }
                catch (IOException)
                {
                    result = "Port was not found";
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            return result;
        }
    }
}

