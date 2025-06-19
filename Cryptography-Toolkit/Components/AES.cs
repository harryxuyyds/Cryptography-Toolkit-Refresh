using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cryptography_Toolkit.Helpers;

namespace Cryptography_Toolkit.Components
{
    public class AES
    {
        private readonly Common _common;

        public AES()
        {
            _common = new Common();
        }

        private static readonly char[] Constant =
        [
            '0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'
        ];

        private static readonly string[,] AesSboxList = new string[16, 16] {
                { "63", "7C", "77", "7B", "F2", "6B", "6F", "C5", "30", "01", "67", "2B", "FE", "D7", "AB", "76" },
                { "CA", "82", "C9", "7D", "FA", "59", "47", "F0", "AD", "D4", "A2", "AF", "9C", "A4", "72", "C0" },
                { "B7", "FD", "93", "26", "36", "3F", "F7", "CC", "34", "A5", "E5", "F1", "71", "D8", "31", "15" },
                { "04", "C7", "23", "C3", "18", "96", "05", "9A", "07", "12", "80", "E2", "EB", "27", "B2", "75" },
                { "09", "83", "2C", "1A", "1B", "6E", "5A", "A0", "52", "3B", "D6", "B3", "29", "E3", "2F", "84" },
                { "53", "D1", "00", "ED", "20", "FC", "B1", "5B", "6A", "CB", "BE", "39", "4A", "4C", "58", "CF" },
                { "D0", "EF", "AA", "FB", "43", "4D", "33", "85", "45", "F9", "02", "7F", "50", "3C", "9F", "A8" },
                { "51", "A3", "40", "8F", "92", "9D", "38", "F5", "BC", "B6", "DA", "21", "10", "FF", "F3", "D2" },

                { "CD", "0C", "13", "EC", "5F", "97", "44", "17", "C4", "A7", "7E", "3D", "64", "5D", "19", "73" },
                { "60", "81", "4F", "DC", "22", "2A", "90", "88", "46", "EE", "B8", "14", "DE", "5E", "0B", "DB" },
                { "E0", "32", "3A", "0A", "49", "06", "24", "5C", "C2", "D3", "AC", "62", "91", "95", "E4", "79" },
                { "E7", "C8", "37", "6D", "8D", "D5", "4E", "A9", "6C", "56", "F4", "EA", "65", "7A", "AE", "08" },
                { "BA", "78", "25", "2E", "1C", "A6", "B4", "C6", "E8", "DD", "74", "1F", "4B", "BD", "8B", "8A" },
                { "70", "3E", "B5", "66", "48", "03", "F6", "0E", "61", "35", "57", "B9", "86", "C1", "1D", "9E" },
                { "E1", "F8", "98", "11", "69", "D9", "8E", "94", "9B", "1E", "87", "E9", "CE", "55", "28", "DF" },
                { "8C", "A1", "89", "0D", "BF", "E6", "42", "68", "41", "99", "2D", "0F", "B0", "54", "BB", "16" }
            };
        private static readonly string[] AesSboxRowIndex = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F"];

        public string ByteSubstitutionLayerCalc(string state)
        {
            var result = new char[state.Length];
            var lineIndex = 0;
            while (lineIndex < state.Length)
            {
                int rowIndex = Convert.ToInt32(state[lineIndex].ToString(), 16);
                int columnIndex = Convert.ToInt32(state[lineIndex + 1].ToString(), 16);
                string sboxOut = AesSboxList[rowIndex, columnIndex];
                result[lineIndex] = sboxOut[0];
                result[lineIndex + 1] = sboxOut[1];
                if (lineIndex + 2 < state.Length)
                    result[lineIndex + 2] = ' ';
                lineIndex += 3;
            }
            return new string(result);
        }

        public string ShiftRowsLayerCalc(string state)
        {
            var result = new char[state.Length];
            var lineIndex = 0;
            for (lineIndex = 0; lineIndex < 12; lineIndex++)
            {
                result[lineIndex] = state[lineIndex];
            }
            for (lineIndex = 12; lineIndex < 24; lineIndex++)
            {
                if (state[lineIndex] == ' ')
                {
                    result[lineIndex] = ' ';
                }
                else
                {
                    result[lineIndex] = state[(lineIndex + 3) % 12 + 12];
                }
            }
            for (lineIndex = 24; lineIndex < 36; lineIndex++)
            {
                if (state[lineIndex] == ' ')
                {
                    result[lineIndex] = ' ';
                }
                else
                {
                    result[lineIndex] = state[(lineIndex + 6) % 12 + 24];
                }
            }
            for (lineIndex = 36; lineIndex < 47; lineIndex++)
            {
                if (state[lineIndex] == ' ')
                {
                    result[lineIndex] = ' ';
                }
                else
                {
                    result[lineIndex] = state[(lineIndex + 9) % 12 + 36];
                }
            }
            return new string(result);
        }

        public string MixColumnsLayerCalc(string state)
        {
            var result = new char[state.Length];
            for (var resultIndex = 0; resultIndex < state.Length; resultIndex++)
                result[resultIndex] = ' ';

            for (var columnIndex = 0; columnIndex < 10; columnIndex += 3)
            {
                var bRow1 = Convert.ToString(Convert.ToInt32(state.Substring(0 + columnIndex, 2), 16), 2).PadLeft(8, '0');
                var bRow2 = Convert.ToString(Convert.ToInt32(state.Substring(12 + columnIndex, 2), 16), 2).PadLeft(8, '0');
                var bRow3 = Convert.ToString(Convert.ToInt32(state.Substring(24 + columnIndex, 2), 16), 2).PadLeft(8, '0');
                var bRow4 = Convert.ToString(Convert.ToInt32(state.Substring(36 + columnIndex, 2), 16), 2).PadLeft(8, '0');
                var cRow1 = (Convert.ToInt32(_common.GfMultiplicationCalc("00000010", bRow1), 16) ^ Convert.ToInt32(_common.GfMultiplicationCalc("00000011", bRow2), 16) ^ Convert.ToInt32(_common.GfMultiplicationCalc("00000001", bRow3), 16) ^ Convert.ToInt32(_common.GfMultiplicationCalc("00000001", bRow4), 16)).ToString("X2");
                var cRow2 = (Convert.ToInt32(_common.GfMultiplicationCalc("00000001", bRow1), 16) ^ Convert.ToInt32(_common.GfMultiplicationCalc("00000010", bRow2), 16) ^ Convert.ToInt32(_common.GfMultiplicationCalc("00000011", bRow3), 16) ^ Convert.ToInt32(_common.GfMultiplicationCalc("00000001", bRow4), 16)).ToString("X2");
                var cRow3 = (Convert.ToInt32(_common.GfMultiplicationCalc("00000001", bRow1), 16) ^ Convert.ToInt32(_common.GfMultiplicationCalc("00000001", bRow2), 16) ^ Convert.ToInt32(_common.GfMultiplicationCalc("00000010", bRow3), 16) ^ Convert.ToInt32(_common.GfMultiplicationCalc("00000011", bRow4), 16)).ToString("X2");
                var cRow4 = (Convert.ToInt32(_common.GfMultiplicationCalc("00000011", bRow1), 16) ^ Convert.ToInt32(_common.GfMultiplicationCalc("00000001", bRow2), 16) ^ Convert.ToInt32(_common.GfMultiplicationCalc("00000001", bRow3), 16) ^ Convert.ToInt32(_common.GfMultiplicationCalc("00000010", bRow4), 16)).ToString("X2");
                result[0 + columnIndex] = cRow1[0];
                result[1 + columnIndex] = cRow1[1];
                result[12 + columnIndex] = cRow2[0];
                result[13 + columnIndex] = cRow2[1];
                result[24 + columnIndex] = cRow3[0];
                result[25 + columnIndex] = cRow3[1];
                result[36 + columnIndex] = cRow4[0];
                result[37 + columnIndex] = cRow4[1];
            }
            return new string(result);
        }

        private string FunctionG(string subKey, int roundCount)
        {
            string[] rCon = ["00000001", "00000010", "00000100", "00001000", "00010000", "00100000", "01000000", "10000000", "00011011", "00110110"];
            var vList = new char[subKey.Length];
            for (var resultIndex = 0; resultIndex < subKey.Length; resultIndex++)
            {
                vList[resultIndex] = ' ';
            }
            vList[0] = subKey[3];
            vList[1] = subKey[4];
            vList[3] = subKey[6];
            vList[4] = subKey[7];
            vList[6] = subKey[9];
            vList[7] = subKey[10];
            vList[9] = subKey[0];
            vList[10] = subKey[1];
            var vLine = new string(vList);
            var vByteSubLine = ByteSubstitutionLayerCalc(vLine);
            var rcAddition = Convert.ToInt32(KeyAdditionLayerCalc(Convert.ToString(Convert.ToInt32(vByteSubLine.Substring(0, 2), 16), 2).PadLeft(8, '0'), rCon[roundCount - 1]), 2).ToString("X2");
            vByteSubLine = rcAddition + vByteSubLine.Substring(2);
            return vByteSubLine;
        }

        public string[] KeyTransformCalc(string key)
        {
            string[] rCon = ["01", "02", "04", "08", "10", "20", "40", "80", "1B", "36"];
            var keyTransform = new string[11];
            keyTransform[0] = key;

            for (var count = 1; count < 11; count++)
            {
                var subKeyList = new char[key.Length];
                var subKeyRow1 = KeyAdditionLayerCalc(keyTransform[count - 1].Substring(0, 11), FunctionG(keyTransform[count - 1].Substring(36, 11), count));
                var subKeyRow2 = KeyAdditionLayerCalc(subKeyRow1, keyTransform[count - 1].Substring(12, 11));
                var subKeyRow3 = KeyAdditionLayerCalc(subKeyRow2, keyTransform[count - 1].Substring(24, 11));
                var subKeyRow4 = KeyAdditionLayerCalc(subKeyRow3, keyTransform[count - 1].Substring(36, 11));
                var tmpString = subKeyRow1 + " " + subKeyRow2 + " " + subKeyRow3 + " " + subKeyRow4;
                keyTransform[count] = tmpString;
            }

            var keyTransformReturn = new string[11];
            keyTransformReturn[0] = key;
            for (var i = 1; i < 11; i++)
            {
                var tmpString = keyTransform[i];
                var subKey = tmpString.Substring(0, 2) + " " + tmpString.Substring(12, 2) + " " + tmpString.Substring(24, 2) + " " + tmpString.Substring(36, 2) + " " + tmpString.Substring(3, 2) + " " + tmpString.Substring(15, 2) + " " + tmpString.Substring(27, 2) + " " + tmpString.Substring(39, 2) + " " + tmpString.Substring(6, 2) + " " + tmpString.Substring(18, 2) + " " + tmpString.Substring(30, 2) + " " + tmpString.Substring(42, 2) + " " + tmpString.Substring(9, 2) + " " + tmpString.Substring(21, 2) + " " + tmpString.Substring(33, 2) + " " + tmpString.Substring(45, 2);
                keyTransformReturn[i] = subKey;
            }
            return keyTransformReturn;

        }

        public string KeyAdditionLayerCalc(string state, string subKey)
        {
            if (state.Length != subKey.Length)
            {
                throw new ArgumentException("State and SubKey must be of the same length");
            }

            var result = new char[state.Length];
            for (var i = 0; i < state.Length; i++)
            {
                if (state[i] == ' ')
                {
                    result[i] = ' ';
                }
                else
                {
                    var stateValue = Convert.ToInt32(state[i].ToString(), 16);
                    var subKeyValue = Convert.ToInt32(subKey[i].ToString(), 16);
                    var xorValue = stateValue ^ subKeyValue;
                    result[i] = xorValue.ToString("X")[0];
                }
            }
            return new string(result);
        }
    }
}
