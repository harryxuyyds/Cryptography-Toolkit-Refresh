using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptography_Toolkit.Helpers;

namespace Cryptography_Toolkit.Components
{
    public class DiffieHellmanKeyExchange
    {
        private readonly Common _common;

        public DiffieHellmanKeyExchange()
        {
            _common = new Common();
        }

        public int DiffieHellmanKeyExchangePreCheck(int modulus, int generator, int privateKeyA, int privateKeyB)
        {
            var millerRabinPrimalityTest = new Components.MillerRabinPrimalityTest();
            // 检查参数是否大于1
            if (modulus <= 1 || generator <= 1 || privateKeyA <= 1 || privateKeyB <= 1)
                return 1; // 参数必须大于1

            // 检查modulus是否为素数
            if (!millerRabinPrimalityTest.MillerRabinPrimalityTestRun(8, modulus))
                return 2; // modulus必须为素数

            // 检查generator是否为modulus的生成元
            if (!IsPrimitiveRoot(generator, modulus))
                return 3; // generator不是生成元

            // 检查私钥是否在合法范围
            if (privateKeyA >= modulus || privateKeyB >= modulus)
                return 4; // 私钥必须小于modulus

            return 0; // 检查通过
        }

        public DiffieHellmanKeyExchangeResult DiffieHellmanKeyExchangeRun(int modulus, int generator, int privateKeyA, int privateKeyB)
        {
            var publicKeyA = _common.SquareMultiplyAlgorithmCalc(generator, privateKeyA, modulus);
            var publicKeyB = _common.SquareMultiplyAlgorithmCalc(generator, privateKeyB, modulus);
            var privateKeyJoint = _common.SquareMultiplyAlgorithmCalc(publicKeyB, privateKeyA, modulus);
            return new DiffieHellmanKeyExchangeResult
            {
                PrivateKeyJoint = privateKeyJoint,
                PublicKeyA = publicKeyA,
                PublicKeyB = publicKeyB,
            };
        }

        // 判断素数
        private bool IsPrime(int n)
        {
            if (n < 2) return false;
            if (n == 2) return true;
            if (n % 2 == 0) return false;
            int sqrt = (int)Math.Sqrt(n);
            for (int i = 3; i <= sqrt; i += 2)
            {
                if (n % i == 0) return false;
            }
            return true;
        }

        // 判断生成元
        private bool IsPrimitiveRoot(int g, int p)
        {
            int phi = p - 1;
            var factors = GetPrimeFactors(phi);
            foreach (var factor in factors)
            {
                if (_common.SquareMultiplyAlgorithmCalc(g, phi / factor, p) == 1)
                    return false;
            }
            return true;
        }

        // 获取n的所有素因子
        private HashSet<int> GetPrimeFactors(int n)
        {
            var factors = new HashSet<int>();
            for (int i = 2; i * i <= n; i++)
            {
                while (n % i == 0)
                {
                    factors.Add(i);
                    n /= i;
                }
            }
            if (n > 1) factors.Add(n);
            return factors;
        }

    }
    public class DiffieHellmanKeyExchangeResult
    {
        public int PublicKeyA { get; set; }
        public int PublicKeyB { get; set; }
        public int PrivateKeyJoint { get; set; }
    }
}
