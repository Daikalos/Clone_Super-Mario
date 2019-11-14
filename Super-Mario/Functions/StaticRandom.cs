using System;

namespace Super_Mario
{
    static class StaticRandom
    {
        private static readonly Random myRandom = new Random();
        private static readonly object mySyncLock = new object();
        public static int RandomNumber(int aMin, int aMax)
        {
            lock (mySyncLock)
            { // synchronize
                return myRandom.Next(aMin, aMax);
            }
        }
    }
}
