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
            TurnOnCar(car);
            TurnOnCopter(helicopter);
            TurnOnLambo(lambo);
            TurnOnTransport(lambo);
        }

        private static void TurnOnCopter(Helicopter h){
            h.TurnOn();
        }

        private static void TurnOnCar(Car c){
            c.TurnOn();
        }

        private static void TurnOnLambo(Lambo l){
            l.TurnOn();
        }

        private static void TurnOnTransport(MotorizedTransport transport)
        {
            transport.TurnOn();
        }
    }
}
