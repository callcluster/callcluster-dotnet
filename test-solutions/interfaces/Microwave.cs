using System;

namespace interfaces
{
    internal class Microwave : ITurnable, IBreakable
    {
        public Microwave()
        {
        }

        public void Break()
        {
            throw new NotImplementedException();
        }

        public void TurnOn()
        {
            throw new NotImplementedException();
        }
    }
}