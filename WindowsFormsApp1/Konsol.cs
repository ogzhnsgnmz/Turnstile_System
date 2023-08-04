using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class Konsol
    {
        IsletimSistemi isletimSistemi;
        string path = null;
        string islSistemi;

        public void yol()
        {
            isletimSistemi = new IsletimSistemi();
            islSistemi = isletimSistemi.Calistir();

            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConsoleApp1" + ((islSistemi == "Windows") ? ".exe" : null));
        }

        public string Olustur()
        {
            yol();
            try
            {
                //File.WriteAllBytes(path, ((islSistemi == "Windows") ? Properties.Resources.ConsoleAppWindows : Properties.Resources.ConsoleAppLinux));

                if (islSistemi == "Windows")
                    File.WriteAllBytes(path, Properties.Resources.ConsoleAppWindows);
                if (islSistemi == "Unix")
                {
                    File.WriteAllBytes(path, Properties.Resources.ConsoleAppLinux);

                    // Unix ise dosya çalıştırma iznini ayarlar.
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "/bin/bash";
                    startInfo.Arguments = "-c \"chmod 774 ConsoleApp1\"";
                    startInfo.RedirectStandardOutput = true;
                    // Bir uygulamanın metin çıktısının akışa StandardOutput yazılıp yazılmadığını belirten bir değer alır veya ayarlar.
                    startInfo.UseShellExecute = false;
                    // İşlemi başlatmak için işletim sistemi kabuğunun kullanılıp kullanılmayacağını belirten bir değer alır veya ayarlar.

                    Process process = new Process();
                    process.StartInfo = startInfo;
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    Console.WriteLine(output);
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("IOException: " + ex.Message);
            }

            return path;
        }
        public void Ac(string arguments = null)
        {
            yol();
            Process process = new Process();
            process.StartInfo.FileName = path;
            if (arguments != null)
                process.StartInfo.Arguments = arguments;
            process.Start();
        }

    }
}
