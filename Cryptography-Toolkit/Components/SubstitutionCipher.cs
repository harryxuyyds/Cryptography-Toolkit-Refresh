using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cryptography_Toolkit.Helpers;

namespace Cryptography_Toolkit.Components
{
    class SubstitutionCipher
    {
        private readonly Common _common;

        public SubstitutionCipher()
        {
            _common = new Common();
        }

        public int SubstitutionCipherPreCheck(string plaintextAlphabet, string ciphertextAlphabet)
        {
            if (plaintextAlphabet.Length != ciphertextAlphabet.Length)
                return 1;

            if (!plaintextAlphabet.All(c => char.IsLetter(c) && char.IsAsciiLetter(c)) ||
                !ciphertextAlphabet.All(c => char.IsLetter(c) && char.IsAsciiLetter(c)))
                return 2;

            if (plaintextAlphabet.ToUpper().Distinct().Count() != ciphertextAlphabet.ToUpper().Distinct().Count())
                return 3;

            return 0;
        }

        public string SubstitutionCipherEncrypt(string plaintextAlphabet, string ciphertextAlphabet, string messageText,
            int selectedCharacterMode)
        {
            var plainMap = new Dictionary<char, char>();
            for (var i = 0; i < plaintextAlphabet.Length; i++)
            {
                var plainCharUpper = char.ToUpperInvariant(plaintextAlphabet[i]);
                var cipherCharUpper = char.ToUpperInvariant(ciphertextAlphabet[i]);
                var plainCharLower = char.ToLowerInvariant(plaintextAlphabet[i]);
                var cipherCharLower = char.ToLowerInvariant(ciphertextAlphabet[i]);
                plainMap.TryAdd(plainCharUpper, cipherCharUpper);
                plainMap.TryAdd(plainCharLower, cipherCharLower);
            }

            var ciphertext = new StringBuilder();
            foreach (var c in messageText)
            {
                if (char.IsLetter(c) && char.IsAsciiLetter(c))
                {
                    if (plainMap.TryGetValue(c, out char mapped))
                    {
                        ciphertext.Append(mapped);
                    }
                    else
                    {
                        ciphertext.Append(c);
                    }
                }
                else
                {
                    if (selectedCharacterMode == 0)
                    {
                        ciphertext.Append(c);
                    }
                }
            }
            return ciphertext.ToString();
        }
        public string SubstitutionCipherDecrypt(string plaintextAlphabet, string ciphertextAlphabet, string ciphertextText,
            int selectedCharacterMode)
        {
            var cipherMap = new Dictionary<char, char>();
            for (var i = 0; i < ciphertextAlphabet.Length; i++)
            {
                var cipherCharUpper = char.ToUpperInvariant(ciphertextAlphabet[i]);
                var plainCharUpper = char.ToUpperInvariant(plaintextAlphabet[i]);
                var cipherCharLower = char.ToLowerInvariant(ciphertextAlphabet[i]);
                var plainCharLower = char.ToLowerInvariant(plaintextAlphabet[i]);
                cipherMap.TryAdd(cipherCharUpper, plainCharUpper);
                cipherMap.TryAdd(cipherCharLower, plainCharLower);
            }

            var plaintext = new StringBuilder();
            foreach (var c in ciphertextText)
            {
                if (char.IsLetter(c) && char.IsAsciiLetter(c))
                {
                    if (cipherMap.TryGetValue(c, out char mapped))
                    {
                        plaintext.Append(mapped);
                    }
                    else
                    {
                        plaintext.Append(c);
                    }
                }
                else
                {
                    if (selectedCharacterMode == 0)
                    {
                        plaintext.Append(c);
                    }
                }
            }
            return plaintext.ToString();
        }
    }
}
