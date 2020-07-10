using System;
using introspection;
namespace consotest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine(new libratest.Class1().AddOne(7));
            introspection.Outside.Inside.Within.DepperWithin.Some();
            
        }
    }
}
