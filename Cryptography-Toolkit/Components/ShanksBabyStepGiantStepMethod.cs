using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cryptography_Toolkit.Helpers;

namespace Cryptography_Toolkit.Components
{
    public class ShanksBabyStepGiantStepMethod
    {
        private readonly Common _common;

        public ShanksBabyStepGiantStepMethod()
        {
            _common = new Common();
        }

        public class ShanksResult
        {
            public List<(int xb, int babyValue)> BabySteps { get; set; } = new();
            public List<(int i, int giantValue)> GiantSteps { get; set; } = new();
            public int? DiscreteLog { get; set; }
            public int GeneratorInverse { get; set; }
            public int T { get; set; }
        }

        public void ShanksBabyStepGiantStepMethodPreCheck(int generator, int element, int order)
        {

        }

        public ShanksResult ShanksBabyStepGiantStepMethodRun(int generator, int element, int order)
        {
            int orderSqrt = (int)Math.Ceiling(Math.Sqrt(order - 1));
            int? result = null;
            bool flagCheck = false;
            int[] babyStepXbList = new int[orderSqrt];
            int[] babyStepModList = new int[orderSqrt];
            int[] giantStepXbList = new int[orderSqrt];
            int[] giantStepModList = new int[orderSqrt];
            var babySteps = new List<(int, int)>();
            var giantSteps = new List<(int, int)>();

            for (int i = 0; i < orderSqrt; i++)
            {
                int baby = _common.SquareMultiplyAlgorithmCalc(generator, i, order);
                int j = 0;
                while (j < i && baby > babyStepModList[j])
                {
                    j++;
                }
                for (int k = i; k > j; k--)
                {
                    babyStepXbList[k] = babyStepXbList[k - 1];
                    babyStepModList[k] = babyStepModList[k - 1];
                }
                babyStepXbList[j] = i;
                babyStepModList[j] = baby;
                babySteps.Add((i, baby));
            }

            (int gcdTemp, int sTemp, int tTemp) = _common.ExtendedEuclideanAlgorithmCalc(order, generator);
            if (gcdTemp != 1)
            {
                return new ShanksResult
                {
                    BabySteps = babySteps,
                    GiantSteps = giantSteps,
                    DiscreteLog = null,
                    GeneratorInverse = tTemp,
                    T = 0
                };
            }
            var generatorInverse = tTemp;
            int t = _common.SquareMultiplyAlgorithmCalc(generatorInverse, orderSqrt, order);

            for (int i = 0; i < orderSqrt; i++)
            {
                if (flagCheck)
                    break;
                int giant = element * _common.SquareMultiplyAlgorithmCalc(t, i, order) % order;
                giantStepXbList[i] = i;
                giantStepModList[i] = giant;
                giantSteps.Add((i, giant));
                for (int j = 0; j < orderSqrt; j++)
                {
                    if (giant == babyStepModList[j])
                    {
                        result = (i * orderSqrt + babyStepXbList[j]) % (order - 1);
                        flagCheck = true;
                        break;
                    }
                }
            }
                
            return new ShanksResult
            {
                BabySteps = babySteps,
                GiantSteps = giantSteps,
                DiscreteLog = result,
                GeneratorInverse = generatorInverse,
                T = t
            };
        }
    }
}

