using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ConsoleApp1.Abstract;

namespace ConsoleApp1.Concrete
{
    public class LogTut : IKomut
    {
        KlasorIslem klasorIslem;

        string anaKlasor = "Loglar";

        private string _islem;
        private string _Message;
        public LogTut(string islem ,string Message)
        {
            _islem = islem;
            _Message = Message;
        }

        public int ProcessId()
        {
            int pId=0;
            Process current = Process.GetCurrentProcess();
            pId = current.Id;
            return pId;
        }

        public void LogKlasor(string anaKlasor, string[] altKlasor)
        {
            klasorIslem = new KlasorIslem();

            List<string> klasörler = new List<string>();

            foreach (string klasor in altKlasor)
                klasörler.Add(klasor);

            if (klasorIslem.KlasorKontrol(anaKlasor) == false)
            {
                klasorIslem.KlasorOlustur(anaKlasor);
                for (int i = 0; i < klasörler.Count; i++)
                    if (klasorIslem.KlasorKontrol(anaKlasor + "/" + klasörler[i]) == false)
                        klasorIslem.KlasorOlustur(anaKlasor + "/" + klasörler[i]);
            }
        }

        public void Calistir()
        {
            Console.WriteLine("Log Tut işlemi yapılıyor...");

            LogKlasor(anaKlasor, 
                new string[]
                {
                    "X",
                    "Y",
                    "Z"
                });

            // dosyanın ismini düzenler gün-ay-yıl.txt
            DateTime tarih = DateTime.Now.Date;
            string fileName = tarih.ToString().TrimEnd('0', ':');
            fileName = fileName.Replace(".", "-");
            fileName = fileName.TrimEnd();

            string path = AppDomain.CurrentDomain.BaseDirectory + anaKlasor + "/" + _islem + "/" + fileName + ".txt";

            StreamWriter sw = File.AppendText(path);
            sw.WriteLine(DateTime.Now.ToString("dd:MM:yyyy HH:mm:ss:fff") + " \t (" + ProcessId() + ") \t Durum: " + _Message);
            sw.Close();

            Console.WriteLine("İşlem bitti kuyruktan çıkartıldı");
        }
    }
}
