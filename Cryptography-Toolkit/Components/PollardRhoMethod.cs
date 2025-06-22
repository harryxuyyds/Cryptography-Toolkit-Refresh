using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cryptography_Toolkit.Helpers;

namespace Cryptography_Toolkit.Components
{
    public class PollardRhoMethod
    {
        private readonly Common _common;

        public PollardRhoMethod()
        {
            _common = new Common();
        }

        public class PollardRhoResult
        {
            public List<(int i, int ai, int bi, int outi)> LogItems { get; set; } = new();
            public int Result { get; set; }
            public int A1 { get; set; }
            public int B1 { get; set; }
            public int A2 { get; set; }
            public int B2 { get; set; }
        }

        public PollardRhoResult PollardRhoMethodRun(int generator, int element, int order)
        {
            int i = 0;
            int ai = 0;
            int bi = 0;
            int outi = 1;
            int result = -1;
            bool flagCheck = false;
            int[] duplicateCheck = new int[order + 1];
            int[] aiCheck = new int[order + 1];
            int[] biCheck = new int[order + 1];
            duplicateCheck[outi] += 1;
            aiCheck[outi] = ai;
            biCheck[outi] = bi;
            var pollardRhoLogItems = new List<(int, int, int, int)>();
            pollardRhoLogItems.Add((i, ai, bi, outi));
            while (true)
            {
                if (flagCheck)
                    return new PollardRhoResult
                    {
                        LogItems = pollardRhoLogItems, 
                        Result = result,
                        A1 = aiCheck[outi],
                        B1 = biCheck[outi],
                        A2 = ai,
                        B2 = bi,
                    };

                i++;
                int tmp = outi % 3;
                if (tmp == 0)
                {
                    ai = (2 * ai) % (order - 1);
                    bi = (2 * bi) % (order - 1);
                    outi = (outi * outi) % order;
                }
                else if (tmp == 1)
                {
                    ai = (ai + 1) % (order - 1);
                    outi = (outi * generator) % order;
                }
                else if (tmp == 2)
                {
                    bi = (bi + 1) % (order - 1);
                    outi = (outi * element) % order;
                }
                pollardRhoLogItems.Add((i, ai, bi, outi));
                if (duplicateCheck[outi] == 0)
                {
                    duplicateCheck[outi] += 1;
                    aiCheck[outi] = ai;
                    biCheck[outi] = bi;
                }
                else
                {
                    int aa = ai - aiCheck[outi];
                    int bb = biCheck[outi] - bi;
                    if (bb != 0)
                    {
                        int d = _common.EuclideanAlgorithmCalc(aa, bb);
                        int sign = 1;
                        if (d < 0 | aa < 0 | bb < 0)
                        {
                            d = -d;
                            sign = -1;
                            aa = Math.Abs(aa);
                            bb = Math.Abs(bb);
                        }
                        if ((order - 1) % d == 0)
                        {
                            aa = aa / d;
                            bb = bb / d;
                            int n = (order - 1) / d;

                            (int gcdTemp, int sTemp, int tTemp) = _common.ExtendedEuclideanAlgorithmCalc(n, bb);
                            var bbInverse = tTemp;
                            while (bbInverse < 0)
                            {
                                bbInverse = bbInverse + n;
                            }
                            result = aa * bbInverse % n;
                            while (result < 0)
                                result = result + n;
                            for (var t = 0; t < d; t++)
                            {
                                result += n * t;
                                if (sign == -1)
                                {
                                    result = (order - 1) - result;
                                }
                                if (_common.SquareMultiplyAlgorithmCalc(generator, result, order) == element)
                                {
                                    flagCheck = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
