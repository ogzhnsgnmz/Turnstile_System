using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Abstract;

namespace ConsoleApp1.Concrete
{
    public class SmsGonder : IKomut
    {
        public void Calistir()
        {
            Console.WriteLine("Sms Gönder işlemi yapılıyor...");
            //Thread.Sleep(5000);
            Console.WriteLine("İşlem bitti kuyruktan çıkartıldı");
        }
    }
}
