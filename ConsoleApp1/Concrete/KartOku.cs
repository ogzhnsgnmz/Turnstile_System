using ConsoleApp1.Abstract;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Concrete
{
    public class KartOku : IKomut
    {

        public void Calistir()
        {
            IsletimSistemi isletimSistemi = new IsletimSistemi();
            IBilgisayar calisanSistem = isletimSistemi.CalisanSistem();

            Console.WriteLine("Kart oku işlemi yapılıyor...");

            calisanSistem.KartOku();

            Console.WriteLine("İşlem bitti kuyruktan çıkartıldı");
        }

    }
}
