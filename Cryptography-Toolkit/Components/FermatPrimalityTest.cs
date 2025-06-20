using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cryptography_Toolkit.Helpers;

namespace Cryptography_Toolkit.Components
{
    public class FermatPrimalityTest
    {
        private readonly Common _common;

        public FermatPrimalityTest()
        {
            _common = new Common();
        }

        public int FermatPrimalityTestSingle(int randomInt, int primeCandidate)
        {
            if (primeCandidate < 2)
                return 0;
            if (primeCandidate == 2)
                return 1;

            int exponent = primeCandidate - 1;
            int result = _common.SquareMultiplyAlgorithmCalc(randomInt, exponent, primeCandidate);

            return result == 1 ? 1 : 0;
        }
    }
}
