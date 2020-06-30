using System;

namespace constructors
{
    public class Main
    {
        public Main(){
            DoThis();
            DoThat();
        }

        public Main(string v1, string v2)
        {
        }
        public Main(int v1, int v2)
        {
        }

        private void DoThat()
        {
            new Main();
            new Main("hello","world");
            new Main(1,1);
        }

        private void DoThis()
        {
            new Child();
            new Grandson();
        }
    }
}
