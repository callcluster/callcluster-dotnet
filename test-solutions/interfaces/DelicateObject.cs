using System;

namespace interfaces
{
    internal class DelicateObject : IBreakable
    {
        public virtual void Break()
        {
            throw new NotImplementedException();
        }
    }
}