using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptography_Toolkit.Helpers
{

    public class AttacksDLPHelper
    {
        private readonly Common _common;

        public AttacksDLPHelper()
        {
            _common = new Common();
        }
        public int AttacksDLPMethodPreCheck(int generator, int element, int order)
        {
            var millerRabinPrimalityTest = new Components.MillerRabinPrimalityTest();
            if (!millerRabinPrimalityTest.MillerRabinPrimalityTestRun(6, order))
            {
                return 1;
            }

            // 2. 检查 α 和 n 互质
            (int gcd, int s, int t) = _common.ExtendedEuclideanAlgorithmCalc(generator, order);
            if (gcd != 1)
            {
                return 2;
            }

            // 3. 检查 β 是否在 α 生成的子群中
            if (!IsInCyclicSubgroup(generator, element, order))
            {
                return 3;
            }

            return 0;
        }

        // 检查 β 是否在 α 生成的子群中
        private bool IsInCyclicSubgroup(int alpha, int beta, int n)
        {
            // 计算 α 的乘法阶数（最小的 k 使得 α^k ≡ 1 mod n）
            int order = MultiplicativeOrder(alpha, n);
            if (order == -1)
            {
                return false; // 无法计算阶数
            }

            // 检查 β^order ≡ 1 mod n
            return _common.SquareMultiplyAlgorithmCalc(beta, order, n) == 1;
        }

        // 计算 a 的乘法阶数（仅适用于 a 和 n 互质的情况）
        private int MultiplicativeOrder(int a, int n)
        {
            (int gcd, int s, int t) = _common.ExtendedEuclideanAlgorithmCalc(a, n);
            if (gcd != 1)
            {
                return -1; // a 和 n 不互质，阶数不存在
            }

            int phi = _common.EulerPhiFunctionCalc(n);
            var factors = Factorize(phi);

            int order = phi;
            foreach (int p in factors)
            {
                while (order % p == 0 && _common.SquareMultiplyAlgorithmCalc(a, order / p, n) == 1)
                {
                    order /= p;
                }
            }
            return order;
        }

        // 质因数分解（返回所有不同的质因数）
        private static HashSet<int> Factorize(int n)
        {
            var factors = new HashSet<int>();
            while (n % 2 == 0)
            {
                factors.Add(2);
                n /= 2;
            }
            for (int p = 3; p * p <= n; p += 2)
            {
                while (n % p == 0)
                {
                    factors.Add(p);
                    n /= p;
                }
            }
            if (n > 1)
            {
                factors.Add(n);
            }
            return factors;
        }
    }
}
