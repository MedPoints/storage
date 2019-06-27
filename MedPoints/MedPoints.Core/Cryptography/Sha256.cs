using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MedPoints.Core.Cryptography
{
    static class Sha256
    {
        public static string Calculate(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            using (var sha = new SHA256Managed())
            {
                var inputBuffer = Encoding.Default.GetBytes(input);
                var outputBuffer = sha.ComputeHash(inputBuffer);

                var output = string.Empty;

                foreach (var b in outputBuffer)
                    output += string.Format("{0:x2}", b);

                return output;
            }
        }
    }
}