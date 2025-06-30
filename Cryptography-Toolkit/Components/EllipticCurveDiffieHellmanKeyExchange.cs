using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptography_Toolkit.Helpers;

namespace Cryptography_Toolkit.Components
{
    public class EllipticCurveDiffieHellmanKeyExchange
    {
        private readonly Common _common;

        public EllipticCurveDiffieHellmanKeyExchange()
        {
            _common = new Common();
        }

        public int KeyExchangePreCheck(int pointGx, int pointGy, int a, int b, int p, int privateKeyA, int privateKeyB)
        {
            if (pointGx <= 1 || pointGy <= 1 || a <= 1 || b <= 1 || p <= 1 || privateKeyA <= 1 || privateKeyB <= 1)
                return 1; // 参数必须大于1
            
            var millerRabinPrimalityTest = new Components.MillerRabinPrimalityTest();
            if (!millerRabinPrimalityTest.MillerRabinPrimalityTestRun(8, p))
                return 2; // p必须为素数
            
            if ((pointGy * pointGy) % p != (pointGx * pointGx * pointGx + a * pointGx + b) % p)
                return 3; // 点G不在椭圆曲线上
            
            if (privateKeyA >= p || privateKeyB >= p)
                return 4; // 私钥必须小于p

            return 0; // 检查通过
        }

        public class KeyExchangeResult
        {
            public string? PublicKeyA { get; set; }
            public string? PublicKeyB { get; set; }
            public string? PrivateKeyJoint { get; set; }
        }

        public KeyExchangeResult KeyExchangeRun(int pointGx, int pointGy, int a, int b, int p, int privateKeyA,
            int privateKeyB)
        {
            var ellipticCurveHelper = new Helpers.EllipticCurveHelper();

            // 计算A的公钥：publicKeyA = privateKeyA * G
            var publicKeyA = ScalarMultiply(pointGx, pointGy, privateKeyA, a, p, ellipticCurveHelper);

            // 计算B的公钥：publicKeyB = privateKeyB * G
            var publicKeyB = ScalarMultiply(pointGx, pointGy, privateKeyB, a, p, ellipticCurveHelper);

            // A计算共享密钥：privateKeyJointA = privateKeyA * publicKeyB
            var privateKeyJointA = ScalarMultiply(publicKeyB[0], publicKeyB[1], privateKeyA, a, p, ellipticCurveHelper);

            // B计算共享密钥：privateKeyJointB = privateKeyB * publicKeyA
            var privateKeyJointB = ScalarMultiply(publicKeyA[0], publicKeyA[1], privateKeyB, a, p, ellipticCurveHelper);

            // 结果一致性校验
            string? jointKey = null;
            if (privateKeyJointA[0] == privateKeyJointB[0] && privateKeyJointA[1] == privateKeyJointB[1])
            {
                jointKey = $"({privateKeyJointA[0]},{privateKeyJointA[1]})";
            }

            return new KeyExchangeResult
            {
                PublicKeyA = $"({publicKeyA[0]},{publicKeyA[1]})",
                PublicKeyB = $"({publicKeyB[0]},{publicKeyB[1]})",
                PrivateKeyJoint = jointKey
            };
        }

        // Updated ScalarMultiply method to ensure result is never null
        private int[] ScalarMultiply(int x, int y, int k, int a, int p, Helpers.EllipticCurveHelper helper)
        {
            int[] result = [0, 0];
            int[] addend = [x, y];

            for (int i = 0; i < 32; i++)
            {
                if (((k >> i) & 1) == 1)
                {
                    if (result[0] == 0 && result[1] == 0) // Check if result is still default
                        result = addend;
                    else
                        result = helper.PointAddCalc(result[0], result[1], addend[0], addend[1], a, p);
                }
                addend = helper.PointDoubleCalc(addend[0], addend[1], a, p);
            }
            return result;
        }
    }
}
