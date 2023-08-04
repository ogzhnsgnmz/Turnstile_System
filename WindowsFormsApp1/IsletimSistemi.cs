using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class IsletimSistemi
    {
        string islSistemi;

        public enum Isim
        {
            Windows,
            Linux,
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
    }
}
