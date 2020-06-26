using System;

namespace complex_project
{
    class Program
    {
        static void Main(string[] args)
        {
            var car = new Car();
            var helicopter = new Helicopter();
            var lambo = new Lambo();
            car.Park();
            helicopter.Land();
            car.TurnOn();
            helicopter.TurnOn();
            lambo.TurnOn();
            TurnItOn(lambo);
        }

        private static void TurnItOn(MotorizedTransport transport)
        {
            transport.TurnOn();
        }
    }
}
