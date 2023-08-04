using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Concrete
{
    public class KlasorIslem
    {
        public void KlasorOlustur(string KlasorAdi)
        {
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + KlasorAdi + "/");
        }
        public bool KlasorKontrol(string KlasorAdi)
        {
            bool Kontrol = Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + KlasorAdi +"/");
            if (Kontrol)
                return true;
            else
                return false;
            return false;
        }
    }
}
