﻿using System;
using System.Linq;

namespace functional
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        private void MapNumbers(){
            string AsNumberPrivate(int n){
                return "h";
            }
            (new int[] {1,2,3,4,5}).Select(AsNumbersMethod);
            (new int[] {1,2,3,4,5}).Select(AsNumberPrivate);
            (new int[] {1,2,3,4,5}).Select((new Thing()).Transform);
        }

        private object AsNumbersMethod(int arg1, int arg2)
        {
            throw new NotImplementedException();
        }
    }

    internal class Thing
    {
        internal object Transform(int arg1, int arg2)
        {
            throw new NotImplementedException();
        }
    }
}
