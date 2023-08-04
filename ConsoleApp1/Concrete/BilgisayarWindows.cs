using System.Runtime.InteropServices;
using ConsoleApp1.Abstract;
using System.IO.Ports;
using System.Diagnostics;

namespace ConsoleApp1.Concrete
{
    public class BilgisayarWindows : IBilgisayar
    {

        //monitör kapat

        [DllImport("powrprof.dll")]
        static extern void SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        //monitör aç

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        const int WM_SYSCOMMAND = 0x0112;
        const int SC_MONITORPOWER = 0xF170;
        const int MONITOR_OFF = 2;

        //kart oku

        SerialPort serialPort;
        string kartKodu;

        //Metotlar

        public void MonitoruAc()
        {
            //SendMessage(GetDesktopWindow(), WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_OFF);
        }
        public void MonitoruKapat()
        {
            SetSuspendState(false, true, true);
        }
        private bool sinyalVarMi(bool signaled)
        {
            bool Procces = false;
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName == "WindowsFormsApp1")
                {
                    signaled = false;
                    Procces = true;
                    break;
                }
            }
            if (Procces == false)
            {
                signaled = true;
                //Console.WriteLine("Ana program kapatıldı.");
            }
            return signaled;
        }

        public string portName = Program._KonsolParam;

        public async void KartOku()
        {
            await Task.Run(() =>
            {
                var OkuEmri = new Byte[8]
                {
                    //bilgiler
                };

                var TurnikeAc = new byte[2]
                {
                    Convert.ToByte('R'),
                    Convert.ToByte((char)200)
                };

                serialPort = new SerialPort(portName, 200, Parity.None, 8, StopBits.One);
                serialPort.Handshake = Handshake.None;
                serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);


                bool createdNew = true;
                var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
                var signaled = false;


                if (!createdNew)
                {
                    waitHandle.Set();
                    return;
                }

                do
                {
                    bool a = sinyalVarMi(signaled);
                    signaled = a;
                    if (signaled == true)
                        break;

                    serialPort.Open();

                    if (Program.turnikeAc == true)
                    {
                        serialPort.Write(TurnikeAc, 0, 2);
                        Program.turnikeAc = false;
                        Thread.Sleep(70);
                    }

                    serialPort.Write(OkuEmri, 0, 8);
                    Thread.Sleep(70);
                    signaled = waitHandle.WaitOne(70);

                    serialPort.Close();
                } while (!signaled);
            });
        }

        public void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            Program.KartIslemKontrol += new Program.DataKontrolHandler(Program.KartIslemKontrolYaz);

            SerialPort serialPort = (SerialPort)sender;
            kartKodu = serialPort.ReadExisting();
            kartKodu = kartKodu.Trim(' ');
            if (kartKodu != "ERR\u0001\u0002\r" && kartKodu != "" && kartKodu.Length == 8)
            {
                Program.KartKodu = kartKodu;
            }
        }
    }
}