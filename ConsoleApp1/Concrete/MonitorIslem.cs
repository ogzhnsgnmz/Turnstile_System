using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using ConsoleApp1.Abstract;

namespace ConsoleApp1.Concrete
{
    public class MonitorIslem : IKomut
    {

        private string _islem;
        public MonitorIslem(string islem)
        {
            _islem = islem;
        }

        public void Calistir()
        {
            IsletimSistemi isletimSistemi = new IsletimSistemi();
            IBilgisayar calisanSistem = isletimSistemi.CalisanSistem();

            Console.WriteLine(_islem +" işlemi yapılıyor...");

            if (_islem == "Monitör Aç")
                calisanSistem.MonitoruAc();
            if (_islem == "Monitör Kapat")
                calisanSistem.MonitoruKapat();

            Console.WriteLine("İşlem bitti kuyruktan çıkartıldı");
        }
    }
}
