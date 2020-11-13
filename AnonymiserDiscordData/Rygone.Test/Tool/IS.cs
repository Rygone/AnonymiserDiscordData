using System;

namespace Rygone.Test.Tool
{
    class IS
    {
        public static IS a(object Object) => new IS(Object);
        public readonly object Object;
        public IS(object Object)
        {
            this.Object = Object;
        }

        public override bool Equals(object obj)
        {
            return Object.Equals(obj);
        }
        public override int GetHashCode()
        {
            return Object.GetHashCode();
        }
        public override string ToString()
        {
            return Object.ToString();
        }
    }
}
