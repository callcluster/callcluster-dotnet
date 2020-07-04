using System;

namespace metrics
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }

        static int DoNormalComplexThings()
        {
            if(true)
            {
                return 10;
            }
            else
            {
                int b = 0;
                if(false)
                {
                    int q = 1;
                    q += 1;
                    b = q;
                }
                for(int i=0;i<10;i++)
                {
                    b+=i;
                }

                return b;
            }
        }

        static int DoNormalComplexThingsWithOneLessBranch()
        {
            int b = 0;
            if(false)
            {
                int q = 1;
                q += 1;
                b = q;
            }
            for(int i=0;i<10;i++)
            {
                b+=i;
            }
            return b;
        }

        static int OneLiner()
        {
            int b = 0; return b;
        }

        static int TwoLiner()
        {
            int b = 0;
            return b;
        }

        static int Returner()
        {
            return 10;
        }

    }
}
