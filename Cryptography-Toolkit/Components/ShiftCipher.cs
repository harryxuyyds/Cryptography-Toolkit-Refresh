using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace Cryptography_Toolkit.Components
{
    public class ShiftCipher
    {
        public string ShiftCipher_Encrypt(string plaintext, int shiftValue, int selectedCharacterMode)
        {
            byte[] origin = Encoding.ASCII.GetBytes(plaintext);
            string ciphertext = string.Empty;
            for (int i = 0; i < origin.Length; i++)
            {
                int asciiCode = origin[i];
                switch (selectedCharacterMode)
                {
                    case 0:
                    {
                        if (asciiCode >= 65 && asciiCode <= 90)
                        {
                            ciphertext += (char)((asciiCode - 65 + shiftValue) % 26 + 65);
                        }
                        else if (asciiCode >= 97 && asciiCode <= 122)
                        {
                            ciphertext += (char)((asciiCode - 97 + shiftValue) % 26 + 97);
                        }
                        else
                        {
                            ciphertext += (char)asciiCode;
                        }

                        break;
                    }
                    case 1:
                    {
                        if (asciiCode >= 65 && asciiCode <= 90)
                        {
                            ciphertext += (char)((asciiCode - 65 + shiftValue) % 26 + 65);
                        }
                        else if (asciiCode >= 97 && asciiCode <= 122)
                        {
                            ciphertext += (char)((asciiCode - 97 + shiftValue) % 26 + 97);
                        }

                        break;
                    }
                }
            }
            return ciphertext;
        }

        public string ShiftCipher_Decrypt(string ciphertext, int shiftValue, int selectedCharacterMode)
        {
            byte[] origin = Encoding.ASCII.GetBytes(ciphertext);
            string plaintext = string.Empty;
            for (int i = 0; i < origin.Length; i++)
            {
                int asciiCode = origin[i];
                switch (selectedCharacterMode)
                {
                    case 0:
                    {
                        if (asciiCode >= 65 && asciiCode <= 90)
                        {
                            int shiftValueTmp = asciiCode - 65 - shiftValue;
                            while (shiftValueTmp < 0)
                                shiftValueTmp += 26;
                            plaintext += (char)(shiftValueTmp % 26 + 65);
                        }
                        else if (asciiCode >= 97 && asciiCode <= 122)
                        {
                            int shiftValueTmp = asciiCode - 97 - shiftValue;
                            while (shiftValueTmp < 0)
                                shiftValueTmp += 26;
                            plaintext += (char)(shiftValueTmp % 26 + 97);
                        }
                        else
                        {
                            plaintext += (char)asciiCode;
                        }

                        break;
                    }
                    case 1:
                    {
                        if (asciiCode >= 65 && asciiCode <= 90)
                        {
                            int shiftValueTmp = asciiCode - 65 - shiftValue;
                            while (shiftValueTmp < 0)
                                shiftValueTmp += 26;
                            plaintext += (char)(shiftValueTmp % 26 + 65);
                        }
                        else if (asciiCode >= 97 && asciiCode <= 122)
                        {
                            int shiftValueTmp = asciiCode - 97 - shiftValue;
                            while (shiftValueTmp < 0)
                                shiftValueTmp += 26;
                            plaintext += (char)(shiftValueTmp % 26 + 97);
                        }

                        break;
                    }
                }
            }
            return plaintext;
        }
    }
}
