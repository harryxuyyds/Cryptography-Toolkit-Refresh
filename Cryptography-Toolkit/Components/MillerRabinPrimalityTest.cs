using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cryptography_Toolkit.Helpers;

namespace Cryptography_Toolkit.Components
{
    public class MillerRabinPrimalityTest
    {
        private readonly Common _common;

        public MillerRabinPrimalityTest()
        {
            _common = new Common();
        }

        public int MillerRabinPrimalityTestSingle(int randomInt, int primeCandidate, int paraU, int paraR)
        {
            if (primeCandidate < 2)
                return 0;
            if (primeCandidate == 2)
                return 1;
            int x = _common.SquareMultiplyAlgorithmCalc(randomInt, paraR, primeCandidate);
            if (x == 1 || x == primeCandidate - 1)
            {
                return 1; // 合格
            }
            for (int i = 0; i < paraU - 1; i++)
            {
                x = _common.SquareMultiplyAlgorithmCalc(x, 2, primeCandidate);
                if (x == primeCandidate - 1)
                {
                    return 1; // 合格
                }
            }
            return 0; // 不合格
        }
    }
}
