namespace constructors
{
    internal class Child : Main
    {
        public Child() : base("yes","no")
        {
        }

        public Child(int v) : base(v,v)
        {
        }
    }
}