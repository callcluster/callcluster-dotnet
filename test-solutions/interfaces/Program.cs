using System;

namespace interfaces
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var tv = new Television();
            TurnOnTv(tv);
            TurnOnTurnable(tv);
            var mw = new Microwave();
            TurnOnMicrowave(mw);
            TurnOnTurnable(mw);
        }

        private static void TurnOnMicrowave(Microwave mw)
        {
            mw.TurnOn();
        }

        private static void TurnOnTv(Television tv)
        {
            tv.TurnOn();
        }

        private static void TurnOnTurnable(ITurnable turnable)
        {
            turnable.TurnOn();
        }
    }
}
