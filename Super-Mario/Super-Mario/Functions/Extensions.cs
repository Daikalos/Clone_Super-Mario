using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
