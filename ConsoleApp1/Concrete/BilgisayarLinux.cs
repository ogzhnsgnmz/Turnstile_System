using System.Diagnostics;
using ConsoleApp1.Abstract;

namespace ConsoleApp1.Concrete
{
    public class BilgisayarLinux : IBilgisayar
    {
        public void MonitoruAc()
        {
            Process.Start("xrandr --output Virtual1 --on");
        }

        public void MonitoruKapat()
        {
            Process.Start("xrandr --output Virtual1 --off");
        }

        public void KartOku()
        {
            throw new NotImplementedException();
        }
    }
}
