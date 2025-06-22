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

        public bool MillerRabinPrimalityTestRun(int securityParameter, int primeCandidate)
        {
            var temp = primeCandidate - 1;
            var paraU = 0;
            while (temp % 2 == 0)
            {
                paraU += 1;
                temp /= 2;
            }
            var paraR = temp;

            var resultCheck = 0;
            for (var index = 1; index <= securityParameter; index++)
            {
                // 生成随机数
                var randomInt = _common.GenerateRandomInt(2, primeCandidate - 1);

                var millerRabinPrimalityTest = new Components.MillerRabinPrimalityTest();
                var result = millerRabinPrimalityTest.MillerRabinPrimalityTestSingle(randomInt, (int)primeCandidate, paraU, (int)paraR);
                resultCheck += result;
            }

            // 检查结果
            if (resultCheck != securityParameter)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
