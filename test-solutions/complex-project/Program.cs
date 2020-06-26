using System;

namespace complex_project
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var car = new Car();
            var helicopter = new Helicopter();
            var lambo = new Lambo();
            car.Park();
            helicopter.Land();
            car.TurnOn();
            helicopter.TurnOn();
            lambo.TurnOn();
        }

        static void One(){
            Two();
            Three();
        }

        static void Two(){
            Three();
        }

        static void Three(){
            LoopHead();
        }

        static void LoopHead(){
            LoopTail();
        }

        static void LoopTail(){
            LoopHead();
        }
    }
}
