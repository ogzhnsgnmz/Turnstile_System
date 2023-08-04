using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string konsolDosyasi;

        private NamedPipeServerStream formToConsol;
        private NamedPipeServerStream consolToForm;

        string durum;
        string islem;
        string mesaj;

        public Form1()
        {
            InitializeComponent();
            KonsolKapat();
            KonsolOlustur();
            formToConsol = new NamedPipeServerStream("FormToConsol", PipeDirection.Out);
            formToConsol.WaitForConnectionAsync();

            consolToForm = new NamedPipeServerStream("ConsolToForm", PipeDirection.In);
            consolToForm.WaitForConnectionAsync();
        }

        // konsol yoksa yeni konsol dosyası oluştur
        private void KonsolOlustur()
        {
            Konsol konsol = new Konsol();
            konsolDosyasi = konsol.Olustur();

            if (File.Exists(konsolDosyasi) == true)
            {
                Process.Start(konsolDosyasi);
            }
            else
            {
                MessageBox.Show("Dosya bulunamadı!");
            }
        }

        // konsol dosyası aç
        private static void KonsolAc(string arguments)
        {
            Konsol konsol = new Konsol();
            konsol.Ac(arguments);
        }

        // konsolu kapat
        private void KonsolKapat()
        {
            foreach (Process p in Process.GetProcesses())
            {
                string file = "ConsoleApp1";
                if (p.ProcessName == file)
                {
                    p.Kill();
                    break;
                }
            }
        }

        static int konsolSayisi = 0;
        // kaç adet konsol açık olduğunu döndürür
        private void KonsolSayiKontrol()
        {
            konsolSayisi = 0;
            foreach (Process p in Process.GetProcesses())
            {
                string file = "ConsoleApp1";
                if (p.ProcessName == file)
                {
                    konsolSayisi += 1;
                }
            }
        }

        public enum cihazAdi
        {
            COM3,
            COM4
        }

        string[] ports = SerialPort.GetPortNames();
        List<string> cihazlar = new List<string>();

        Dictionary<string, NamedPipeServerStream> cihazFormToConsol = new Dictionary<string, NamedPipeServerStream>();
        Dictionary<string, NamedPipeServerStream> cihazConsolToForm = new Dictionary<string, NamedPipeServerStream>();

        public async Task Yaz(string durum, string islem, string mesaj, NamedPipeServerStream pipe)
        {
            string GonderilenMesaj = durum + "-" + islem + "-" + mesaj;
            StreamWriter writer = new StreamWriter(pipe);
            writer.AutoFlush = true;

            foreach (string cihaz in Enum.GetNames(typeof(cihazAdi)))
                cihazlar.Add(cihaz);

            if (durum == "Kart Oku Aç")
                foreach (string port in ports)
                    foreach (string cihaz in cihazlar)
                        if (port == cihaz)
                        {
                            KonsolSayiKontrol();
                            if (konsolSayisi == 0)
                            {
                                MessageBox.Show("Hiç konsol yok");
                            }
                            cihazFormToConsol.Add(cihaz + "FormToConsol", new NamedPipeServerStream(cihaz + "FormToConsol", PipeDirection.Out));
                            cihazConsolToForm.Add(cihaz + "ConsolToForm", new NamedPipeServerStream(cihaz + "ConsolToForm", PipeDirection.In));
                            //KonsolAc("Kart Oku-" + cihaz);
                            KonsolAc(cihaz);
                            cihazFormToConsol[cihaz + "FormToConsol"].WaitForConnectionAsync();
                            cihazConsolToForm[cihaz + "ConsolToForm"].WaitForConnectionAsync();

                        }

            await writer.WriteLineAsync(GonderilenMesaj);

            listBox1.Items.Add(GonderilenMesaj);

            DurumKontrol(durum);
        }

        void DurumKontrol(string durum)
        {
            lblDurum.Text = durum;

            if (lblDurum.Text == "Kart Oku Aç")
            {
                //comboBox1.Items.Remove("Kart Oku Aç");
                comboBox1.Text = "Log Tut";
                lblDurum.Text = "";
                button2.Enabled = true;
            }
        }

        string _okunanBilgi = "";
        string _okunanMesaj = "";
        string _okunanParam = "";

        static int oncekiKonsolSayisi = 0;

        private async Task Oku(NamedPipeServerStream pipe)
        {
            StreamReader reader = new StreamReader(pipe);

            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            var signaled = false;

            do
            {
                signaled = waitHandle.WaitOne(70);


                string okunanMesajlar = await reader.ReadLineAsync();
                listBox1.Items.Add(okunanMesajlar);

                string ara = okunanMesajlar.IndexOf("-").ToString();
                
                if (ara != "-1")
                {
                    string[] okunanMesaj = okunanMesajlar.Split('-');
                    _okunanBilgi = okunanMesaj[0];
                    _okunanMesaj = okunanMesaj[1];
                    _okunanParam = okunanMesaj[2];
                }
                else
                    _okunanMesaj = okunanMesajlar;

                if (_okunanBilgi == "Kart Kodu" && _okunanMesaj != "")
                {
                    Yaz("Turnike Aç", "X", _okunanMesaj, cihazFormToConsol[_okunanParam + "FormToConsol"]);
                }

                KonsolSayiKontrol();
                if (oncekiKonsolSayisi == 0)
                    oncekiKonsolSayisi = konsolSayisi;

                if (oncekiKonsolSayisi != konsolSayisi)
                {
                    //KonsolAc("COM3");

                }
                oncekiKonsolSayisi = konsolSayisi;

            } while (!signaled);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool isOpen = false;

            durum = comboBox1.Text;
            islem = comboBox2.Text;
            mesaj = comboBox3.Text;

            foreach (Process p in Process.GetProcesses())
            {
                string file = "ConsoleApp1";
                if (p.ProcessName == file)
                {
                    Yaz(durum, islem, mesaj, formToConsol);
                    isOpen = true;
                    break;
                }
            }
            if (isOpen == false)
            {
                formToConsol.Close();
                consolToForm.Close();
                KonsolOlustur();
                formToConsol = new NamedPipeServerStream("FormToConsol", PipeDirection.Out);
                formToConsol.WaitForConnectionAsync();
                consolToForm = new NamedPipeServerStream("ConsolToForm", PipeDirection.In);
                consolToForm.WaitForConnectionAsync();
                Thread.Sleep(300);
                Yaz(durum, islem, mesaj, formToConsol);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            foreach (string cihaz in cihazlar)
                Oku(cihazConsolToForm[cihaz + "ConsolToForm"]);
        }
    }
}
