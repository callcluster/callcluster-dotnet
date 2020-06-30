using System;

namespace complex_project
{
    internal class Lambo : Car
    {
        public Lambo()
        {
        }

        override public void TurnOn(){
            Console.Write("VROOOOM");
        }
    }
}