using ConsoleApp1.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Concrete
{
    public class IsletimSistemi
    {
        private IBilgisayar _IBilgisayar;

        string islSistemi;

        public enum Isim
        {
            Windows,
            Unix
        }

        public string Calistir()
        {
            islSistemi = Environment.OSVersion.ToString();
            string ara;

            foreach (string sonuc in Enum.GetNames(typeof(Isim)))
            {
                ara = islSistemi.IndexOf(sonuc).ToString();
                if (ara != "-1")
                    return sonuc;
            }
            return null;
        }

        public IBilgisayar CalisanSistem()
        {
            if (Calistir() == "Windows")
                _IBilgisayar = new BilgisayarWindows();
            if (Calistir() == "Unix")
                _IBilgisayar = new BilgisayarLinux();
            return _IBilgisayar;
        }
    }
}
