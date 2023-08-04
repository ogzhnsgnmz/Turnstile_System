using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Abstract;

namespace ConsoleApp1.Concrete
{
    public class DurumGonder : IKomut
    {
        public void Calistir()
        {
            Console.WriteLine("Durum Gönder işlemi yapılıyor..."); 

            Console.WriteLine("İşlem bitti kuyruktan çıkartıldı");
        }
    }
}
