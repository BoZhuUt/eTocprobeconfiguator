using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST58X_71XConfigurator
{
    internal class EEEECondData
    {
        public float ConduS;
        public float CondIGain;
        public float CondVGain;

        public ushort[] CondIWave = new ushort[1024];
        public ushort[] CondVWave = new ushort[1024];
        public ushort[] ReceiveData;

        public string StandardConduS = "";
        public EEEECondData()
        {

        }
        public void ParsingData(byte[] buffer)
        {
            ReceiveData = new ushort[buffer.Length / 2];
            CondIGain = buffer[0];
            int i = 0;
            try
            {
                for (int j = 0; j < ReceiveData.Length; j++)
                {
                    ReceiveData[j] = BitConverter.ToUInt16(buffer, i);
                    i += 2;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        public EEEECondData(byte[] buffer)
        {
            int i = 0;
            CondIGain = buffer[0];//8-9-10-11
            //i += 1;
            //for (int j = 0; j < 1024; j++)
            //{
            //    CondVWave[j] = BitConverter.ToUInt16(buffer, i);
            //    i += 2;
            //}
            for (int j = 0; j < 1024; j++)
            {
                CondIWave[j] = BitConverter.ToUInt16(buffer, i);
                i += 2;
            }
        }
        public EEEECondData(string csvline)
        {
            string[] datas = csvline.Split(',');
            try
            {
                StandardConduS = datas[0];

                CondIGain = float.Parse(datas[1]);

                for (int i = 0; i < 1024; i++)
                {
                    CondIWave[i] = UInt16.Parse(datas[2 + i]);
                }
                for (int i = 0; i < 1024; i++)
                {
                    CondVWave[i] = UInt16.Parse(datas[2 + 1024 + i]);
                }
            }
            catch
            {
                throw new NotImplementedException();
            }

        }
        public static string CSVHeader = "Comment,CondIGain\n";
        public string Convert2CsvLine(string StandarduS)
        {
            StandardConduS = StandarduS;
            string line = StandarduS + ",";

            line += CondIGain.ToString() + ",";

            foreach (var a in CondIWave)
            {
                line += a.ToString() + ",";
            }
            foreach (var a in CondVWave)
            {
                line += a.ToString() + ",";
            }
            line += "\n";
            return line;
        }
    }
}
