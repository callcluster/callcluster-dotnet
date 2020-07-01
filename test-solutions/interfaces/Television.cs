using System;

namespace interfaces
{
    internal class Television : ITurnable, IBreakable
    {
        public Television()
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