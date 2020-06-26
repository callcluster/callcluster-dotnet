using System;

namespace complex_project
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
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
