using System;
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
        
    }
}
