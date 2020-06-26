using System;

namespace complex_project
{
    internal class Lambo : MotorizedTransport
    {
        public Lambo()
        {
        }

        override public void TurnOn(){
            Console.Write("VROOOOM");
        }
    }
}