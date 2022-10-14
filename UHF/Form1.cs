using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Converters;
using System.Net.Sockets;
using System.Text;

namespace UHF
{
    public partial class Form1 : Form
    {
        public static string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string path = rootPath + "\\VimassUHF\\dataUHF.json";
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
            button11.Enabled = false;

            byte[] arrBuffer = new byte[256];
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            byte[] arrBuffer = new byte[64];

            String strPort = comboBox1.Text;
            if (UHF.SWComApi.SWCom_OpenDevice(strPort, 115200))
            {
                this.SetText("DeviveConect");
                if (UHF.SWComApi.SWCom_GetDeviceSystemInfo(0xFF, arrBuffer) == false)
                {
                    this.SetText("Device is Out\r\n");
                    //RFID.SWComApi.SWCom_CloseDevice();
                    //return;
                }
            }
            else
            {
                this.SetText("Failed\r\n");
                return;
            }

            string str = "", str1 = "";
            str = String.Format("SoftVer:{0:D}.{0:D}\r\n", arrBuffer[0] >> 4, arrBuffer[0] & 0x0F);
            this.SetText(str);
            str = String.Format("HardVer:{0:D}.{0:D}\r\n", arrBuffer[1] >> 4, arrBuffer[1] & 0x0F);
            this.SetText(str);
            str = "SN:";
            for (int i = 0; i < 7; i++)
            {
                str1 = String.Format("{0:X2}", arrBuffer[2 + i]);
                str = str + str1;
            }
            str = str + "\r\n";
            this.SetText(str);
            button1.Enabled = false;
            button2.Enabled = true;
        }
        delegate void SetTextCallback(string text);
        private void SetText(string text)
        {
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                if (this.textBox1.TextLength > 1000) this.textBox1.Text = "";
                this.textBox1.Text = this.textBox1.Text + text;
                this.textBox1.SelectionStart = this.textBox1.Text.Length;
                this.textBox1.ScrollToCaret();
            }
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            this.SetText("ActiveMode\r\n");
            UHF.SWComApi.SWCom_ClearTagBuf();
            timer1.Interval = 7000;
            timer1.Enabled = true;
            button6.Enabled = false;
            button11.Enabled = true;
            
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            byte[] arrBuffer = new byte[64000];
            int iNum = 0;
            int iTotalLen = 0;
            byte bRet = 0;

            bRet = UHF.SWComApi.SWCom_GetTagBuf(arrBuffer, out iTotalLen, out iNum);
            if (bRet == 1)
            {
                this.SetText("DevOut");
                return; //DevOut
            }
            else if (bRet == 0) return; //No Connect
            int iTagLength = 0;
            int iTagNumber = 0;
            iTagLength = iTotalLen;
            iTagNumber = iNum;
            if (iTagNumber == 0) return;
            int iIndex = 0;
            int iLength = 0;
            byte bPackLength = 0;
            int i = 0;
            int iIDLen = 0;
            for (iIndex = 0; iIndex < iTagNumber; iIndex++)
            {
                bPackLength = arrBuffer[iLength];
                string str2 = "";
                string str1 = "";
                str1 = arrBuffer[1 + iLength + 0].ToString("X2");
                str2 = str2 + "Type:" + str1 + " ";  //Tag Type
                if ((arrBuffer[1 + iLength + 0] & 0x80) == 0x80)
                {   // with TimeStamp , last 6 bytes is time
                    iIDLen = bPackLength - 7;
                }
                else iIDLen = bPackLength - 1;

                str1 = arrBuffer[1 + iLength + 1].ToString("X2");
                str2 = str2 + "Ant:" + str1 + " Tag:";  //Ant

                string str3 = "";
                for (i = 2; i < iIDLen; i++)
                {
                    str1 = arrBuffer[1 + iLength + i].ToString("X2");
                    str3 = str3 + str1 + " ";
                }
                str2 = str2 + str3;
                str1 = arrBuffer[1 + iLength + i].ToString("X2");
                str2 = str2 + "RSSI:" + str1 + "\r\n";  //RSSI
                iLength = iLength + bPackLength + 1;
                str3 = str3.Trim();
                try
                {
                    String DulieuUHF = "";
                    DulieuUHF = docThongTin(path);
                    this.SetText(DulieuUHF);
                    var datauhf = JsonConvert.DeserializeObject<List<DataUHF>>(DulieuUHF);
                   
                    if (datauhf != null)
                    {
                        for (int i2 = 0; i2 < datauhf.Count; i2++)
                        {
                            if (datauhf[i2].tag.Equals(str3))
                            {
                          
                                
                                    UdpClient udpClient2 = new UdpClient();
                                    string strip2 = "192.168.44.101";
                                    udpClient2.Connect(strip2, Convert.ToInt16("5000"));
                                    Byte[] senddata2 = Encoding.ASCII.GetBytes("10000^200192168044108MO CUA_1284#");
                                    udpClient2.Send(senddata2, senddata2.Length);
                                    MessageBox.Show("danh");
                                

                            }
                            else
                            {
                                MessageBox.Show("Vui long lai gan cua");
                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }
        }

        private void button11_Click_1(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button9_Click_1(object sender, EventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            UHF.SWComApi.SWCom_CloseDevice();
            button1.Enabled = true;
            button2.Enabled = false;
            button6.Enabled = true;
            button11.Enabled = false;
            this.SetText("Close\r\n");
        }

        private void button8_Click_1(object sender, EventArgs e)
        {

        }
        public static string docThongTin(string path)
        {
            string temp = File.ReadAllText(path);
            return temp;
        }
    }
}