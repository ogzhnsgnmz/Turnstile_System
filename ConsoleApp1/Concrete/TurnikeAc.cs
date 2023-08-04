using ConsoleApp1.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Concrete
{
    public class TurnikeAc:IKomut
    {
        string _message;
        public TurnikeAc(string message)
        {
            _message = message;
        }
        public void Calistir()
        {

            Console.WriteLine("Turnike aç işlemi yapılıyor...");

            Console.WriteLine("Okutulan Kart: "+_message);

            Console.WriteLine("İşlem bitti kuyruktan çıkartıldı");
        }
    }
}
