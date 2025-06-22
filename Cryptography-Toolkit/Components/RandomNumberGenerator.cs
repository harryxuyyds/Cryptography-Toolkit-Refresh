using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Cryptography_Toolkit.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cryptography_Toolkit.Components
{
    public class RandomNumberGenerator
    {
        private readonly Common _common;

        public RandomNumberGenerator()
        {
            _common = new Common();
        }

        public string PseudorandomNumberGenerator(BigInteger seed, BigInteger multiplier, BigInteger increment, BigInteger modulus, BigInteger quantity)
        {
            StringBuilder result = new StringBuilder();
            var current = seed;
            for (var i = 0; i < quantity; i++)
            {
                current = (multiplier * current + increment) % modulus;
                result.AppendLine(current.ToString());
            }
            return result.ToString();
        }
    }
}
