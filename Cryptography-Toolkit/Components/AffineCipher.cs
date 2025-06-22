using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

using Cryptography_Toolkit.Helpers;

namespace Cryptography_Toolkit.Components
{

    public class AffineCipher
    {
        private readonly Common _common;

        // Constructor to initialize the readonly field
        public AffineCipher()
        {
            _common = new Common();
        }

        public string AffineCipher_Encrypt(string plaintext, int additiveKey, int multiplicativeKey,
            int selectedCharacterMode)
        {
            Debug.WriteLine(plaintext);
            var origin = Encoding.ASCII.GetBytes(plaintext);
            var ciphertext = string.Empty;
            for (var i = 0; i < origin.Length; i++)
            {
                int asciiCode = origin[i];
                if (selectedCharacterMode == 0)
                {
                    if (asciiCode >= 65 && asciiCode <= 90)
                    {
                        ciphertext += (char)((multiplicativeKey * (asciiCode - 65) + additiveKey) % 26 + 65);
                    }
                    else if (asciiCode >= 97 && asciiCode <= 122)
                    {
                        ciphertext += (char)((multiplicativeKey * (asciiCode - 97) + additiveKey) % 26 + 97);
                    }
                    else
                    {
                        ciphertext += (char)asciiCode;
                    }
                }
                else if (selectedCharacterMode == 1)
                {
                    if (asciiCode >= 65 && asciiCode <= 90)
                    {
                        ciphertext += (char)((multiplicativeKey * (asciiCode - 65) + additiveKey) % 26 + 65);
                    }
                    else if (asciiCode >= 97 && asciiCode <= 122)
                    {
                        ciphertext += (char)((multiplicativeKey * (asciiCode - 97) + additiveKey) % 26 + 97);
                    }
                }
            }

            return ciphertext;
        }

        public string AffineCipher_Decrypt(string ciphertext, int additiveKey, int multiplicativeKey,
            int selectedCharacterMode)
        {
            var origin = Encoding.ASCII.GetBytes(ciphertext);
            var plaintext = string.Empty;
            for (var i = 0; i < origin.Length; i++)
            {
                int asciiCode = origin[i];
                if (selectedCharacterMode == 0)
                {
                    if (asciiCode >= 65 && asciiCode <= 90)
                    {
                        (int gcd, int s, int t) = _common.ExtendedEuclideanAlgorithmCalc(26, multiplicativeKey);
                        var aInverse = t;
                        var tmp = aInverse * (asciiCode - 65 - additiveKey);
                        while (tmp < 0)
                            tmp += 26;
                        plaintext += (char)((tmp % 26) + 65);
                    }
                    else if (asciiCode >= 97 && asciiCode <= 122)
                    {
                        (int gcd, int s, int t) = _common.ExtendedEuclideanAlgorithmCalc(26, multiplicativeKey);
                        var aInverse = t;
                        int tmp = aInverse * (asciiCode - 97 - additiveKey);
                        while (tmp < 0)
                            tmp += 26;
                        plaintext += (char)((tmp % 26) + 97);
                    }
                    else
                    {
                        plaintext += (char)asciiCode;
                    }
                }
                else if (selectedCharacterMode == 1)
                {
                    if (asciiCode >= 65 && asciiCode <= 90)
                    {
                        (int gcd, int s, int t) = _common.ExtendedEuclideanAlgorithmCalc(26, multiplicativeKey);
                        var aInverse = t;
                        int tmp = aInverse * (asciiCode - 65 - additiveKey);
                        while (tmp < 0)
                            tmp += 26;
                        plaintext += (char)((tmp % 26) + 65);
                    }
                    else if (asciiCode >= 97 && asciiCode <= 122)
                    {
                        (int gcd, int s, int t) = _common.ExtendedEuclideanAlgorithmCalc(26, multiplicativeKey);
                        var aInverse = t;
                        int tmp = aInverse * (asciiCode - 97 - additiveKey);
                        while (tmp < 0)
                            tmp += 26;
                        plaintext += (char)((tmp % 26) + 97);
                    }
                }
            }

            return plaintext;
        }
    }
}
