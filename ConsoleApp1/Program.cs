using ConsoleApp1.Abstract;
using ConsoleApp1.Concrete;
using System.Diagnostics;
using System.IO.Pipes;

namespace ConsoleApp1
{
    class Program
    {
        static NamedPipeClientStream formToConsol;
        static NamedPipeClientStream consolToForm;

        static string okunanMesaj;

        static IKomut komut;
        static Queue<IKomut> komutlar = new Queue<IKomut>();

        static string hataMesaji = "";

        static bool formKontrol = false;
        static bool FormKontrol()
        {
            foreach (Process p in Process.GetProcesses())
            {
                string file = "WindowsFormsApp1";
                if (p.ProcessName == file)
                    formKontrol = true;
                else
                    formKontrol = false;
            }
            return formKontrol;
        }

        public static bool turnikeAc = false;

        //public static async Task Oku(string cihazMessage, NamedPipeClientStream pipe)
        public static async Task Oku(string mesajParam, NamedPipeClientStream pipe)
        {
            //StreamReader reader = new StreamReader(pipe);
            StreamReader reader = new StreamReader(pipe);
            bool createdNew;
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            var signaled = false;

            do
            {
                signaled = waitHandle.WaitOne(100);
                bool a = sinyalVarMi(signaled);
                signaled = a;
                if (signaled == true && komutlar.Count == 0)
                    break;

                // parametre aldıysa okuma işlemine alır
                if (mesajParam != null)
                    okunanMesaj = mesajParam;
                // formdan geleni işleme alır
                else
                    okunanMesaj = await reader.ReadLineAsync();
                Console.WriteLine(okunanMesaj);

                // gelen mesajı parçala
                string[] mesaj = okunanMesaj.Split('-');

                // mesaj boş ise geç
                if (okunanMesaj == null)
                    continue;

                // parçalanan mesaj değişkenleri
                string durum = mesaj[0];
                string islem = mesaj[1];
                string mesajj = mesaj[2];

                switch (durum)
                {
                    case "Log Tut":
                        komut = new LogTut(islem, mesajj);
                        break;
                    case "Sms Gönder":
                        komut = new SmsGonder();
                        break;
                    case "Durum Gönder":
                        komut = new DurumGonder();
                        break;
                    case "Monitör Aç":
                    case "Monitör Kapat":
                        komut = new MonitorIslem(durum);
                        break;
                    case "Kart Oku":
                        komut = new KartOku();
                        break;
                    case "Kart Oku Aç":
                        Console.WriteLine("Kart Okuma cihazı bağlandı!");
                        continue;
                    case "Turnike Aç":
                        turnikeAc = true;
                        continue;
                }

                komutlar.Enqueue(komut);
                Console.WriteLine("Kuyruğa eklendi: {0}", okunanMesaj);
                mesajParam = null;
            } while (!signaled);
        }


        static string kartKodu;
        public static string KartKodu
        {
            get { return kartKodu; }
            set
            {
                kartKodu = value;
                KartIslemKontrol();
            }
        }

        public delegate void DataKontrolHandler();
        public static event DataKontrolHandler KartIslemKontrol;

        static string sonGelen = "";

        // kart okutma işlemi yapılınca çalışan evente ait void method
        // yaz fonksiyonun cihaz pipe ile çağırıyor
        public static void KartIslemKontrolYaz() => Yaz(consolToForm);
        public static async Task Yaz(NamedPipeClientStream pipe)
        {
            StreamWriter writer = new StreamWriter(pipe);
            writer.AutoFlush = true;

            //kart kodu gönder
            if (sonGelen != kartKodu)
            {
                sonGelen = kartKodu;
                await writer.WriteLineAsync("Kart Kodu-" + kartKodu +"-"+ _KonsolParam);
                Console.WriteLine("Kart okutuldu.");
            }
        }

        static async void Islem()
        {
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
                signaled = waitHandle.WaitOne(100);
                bool a = sinyalVarMi(signaled);
                signaled = a;
                if (signaled == true && komutlar.Count == 0)
                    break;

                if (komutlar.Count > 0)
                {
                    var v = komutlar.Dequeue();
                    if (v != null)
                    {
                        new Thread(() =>
                        {
                            v.Calistir();
                        }).Start();
                    }
                }

            } while (!signaled);
        }

        static bool sinyalVarMi(bool signaled)
        {
            bool Procces = false;
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName == "WindowsFormsApp1" || p.ProcessName == "mono")
                {
                    signaled = false;
                    Procces = true;
                    break;
                }
            }
            if (Procces == false)
            {
                signaled = true;
                hataMesaji = "Bağlantı kesildi!";
            }
            return signaled;
        }

        private static void OnTimerElapsed(object state)
        {
            Console.WriteLine("Timer elapsed.");
        }

        public static string _KonsolParamIslem;
        public static string _KonsolParam;

        public static bool anaKonsol = false;

        static void Main(string[] args)
        {
            if (null != args && args.Length > 0)
            {
                foreach (var formdanGelenMesaj in args)
                {
                    _KonsolParam = formdanGelenMesaj;
                    /*
                    string[] _mesajParcala = formdanGelenMesaj.Split("-");

                    _formdanGelenMesajIslem = _mesajParcala[0];
                    _formdanGelenMesaj = _mesajParcala[1];
                    */
                }
            }

            Console.WriteLine("Bağlantı bekleniyor...");

            //form açık mı
            FormKontrol();
            if (formKontrol == true) Environment.Exit(0);

            //parametre geldiyse başına o isimde gelmediyse ana konsol yani değişken boş olacak şekilde pipe oluşturur
            formToConsol = new NamedPipeClientStream(".", _KonsolParam + "FormToConsol", PipeDirection.In);
            formToConsol.ConnectAsync();
            consolToForm = new NamedPipeClientStream(".", _KonsolParam + "ConsolToForm", PipeDirection.Out);
            consolToForm.ConnectAsync();

            //konsola parametre geldiyse kart oku'yu çalıştırır fakat cihaz için özelleştirilmedi COM3, COM4 gibi
            if (_KonsolParam != null)
                Oku("Kart Oku-X-Mesaj1", formToConsol);
            else
                anaKonsol = true;

            Console.WriteLine("Bağlandı.");

            Oku(null, formToConsol);
            Islem();

            if (anaKonsol != true)
            {

            }

            if (!formToConsol.IsConnected)
                Console.WriteLine(hataMesaji);
        }

    }
}