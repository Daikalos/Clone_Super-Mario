namespace Super_Mario
{
    static class Extensions
    {
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
