using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoints.Core.Cryptography
{
    static class Nonce
    {
        static readonly Random random;

        static Nonce()
        {
            random = new Random();
        }

        public static long Generate()
        {
            return random.Next(0, int.MaxValue);
        }
    }
}
