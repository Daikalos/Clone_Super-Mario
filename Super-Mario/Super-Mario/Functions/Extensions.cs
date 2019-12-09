namespace Super_Mario
{
    static class Extensions
    {
        /// <summary>
        /// Returns what sign of specified number, e.g. if negative or positive
        /// </summary>
        public static int Signum(float aNumber)
        {
            if (aNumber < 0)
            {
                return -1;
            }
            if (aNumber >= 0)
            {
                return 1;
            }
            return 0;
        }
    }
}
