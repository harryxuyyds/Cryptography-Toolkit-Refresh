using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptography_Toolkit.Helpers
{
    public class Common
    {
        private static readonly char[] Constant =
        {
            '0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'
        };

        public string GenerateRandomNumber(int length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(16);
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                newRandom.Append(Constant[rd.Next(16)]);
            }
            return newRandom.ToString();
        }

        public int GenerateRandomInt(int minValue, int maxValue)
        {
            Random rd = new Random();
            return rd.Next(minValue, maxValue);
        }

        public int SquareMultiplyAlgorithmCalc(int baseElement, int exponent, int modulus)
        {
            string exponentToBin = Convert.ToString(exponent, 2);
            int exponentLength = exponentToBin.Length;
            int index;
            long result = 1;
            for (index = 1; index <= exponentLength; index++)
            {
                result = result * result % modulus;
                if (exponentToBin[index - 1] == '1')
                {
                    result = result * baseElement % modulus;
                }
            }
            return (int)result;
        }

        public string GfMultiplicationCalc(string para1, string para2)
        {
            ArrayList xTimeCheck = new ArrayList();
            for (int index = 0; index < para2.Length; index++)
            {
                if (para2[index] == '1')
                {
                    xTimeCheck.Add(7 - index);
                }
            }
            int gfMultiplicationOut = 0;
            foreach (int xTimeCount in xTimeCheck)
            {
                string init = para1;
                for (int temp = 0; temp < xTimeCount; temp++)
                {
                    if (init[0] == '0')
                    {
                        init = string.Concat(init.AsSpan(1), "0");
                    }
                    else
                    {
                        int initTemp = Convert.ToInt32(string.Concat(init.AsSpan(1), "0"), 2);
                        initTemp = initTemp ^ 27;
                        init = Convert.ToString(initTemp, 2).PadLeft(8, '0');
                    }
                }
                gfMultiplicationOut ^= Convert.ToInt32(init, 2);
            }
            return gfMultiplicationOut.ToString("X2");
        }
    }
}
