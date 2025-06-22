using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cryptography_Toolkit.Helpers;

namespace Cryptography_Toolkit.Components
{
    public class GaloisFields
    {
        private readonly Common _common;

        public GaloisFields()
        {
            _common = new Common();
        }

        public string GFMultiplicationCalc(string para1, string para2)
        {
            var xTimeCheck = new ArrayList();
            for (var index = 0; index < para2.Length; index++)
            {
                if (para2[index] == '1')
                {
                    xTimeCheck.Add(7 - index);
                }
            }
            var result = 0;
            foreach (int xTimeCount in xTimeCheck)
            {
                var init = para1;
                for (var temp = 0; temp < xTimeCount; temp++)
                {
                    if (init[0] == '0')
                    {
                        init = string.Concat(init.AsSpan(1), "0");
                    }
                    else
                    {
                        var initTemp = Convert.ToInt32(string.Concat(init.AsSpan(1), "0"), 2);
                        initTemp = initTemp ^ 27;
                        init = Convert.ToString(initTemp, 2).PadLeft(8, '0');
                    }
                }
                result ^= Convert.ToInt32(init, 2);
            }
            return result.ToString("X2");
        }

        public string GFInversionCalc(string para1)
        {
            var gfInversionList = new string[16, 16] {
                {"00", "01", "8D", "F6", "CB", "52", "7B", "D1", "E8", "4F", "29", "C0", "B0", "E1", "E5", "C7"},
                {"74", "B4", "AA", "4B", "99", "2B", "60", "5F", "58", "3F", "FD", "CC", "FF", "40", "EE", "B2"},
                {"3A", "6E", "5A", "F1", "55", "4D", "A8", "C9", "C1", "0A", "98", "15", "30", "44", "A2", "C2"},
                {"2C", "45", "92", "6C", "F3", "39", "66", "42", "F2", "35", "20", "6F", "77", "BB", "59", "19"},
                {"1D", "FE", "37", "67", "2D", "31", "F5", "69", "A7", "64", "AB", "13", "54", "25", "E9", "09"},
                {"ED", "5C", "05", "CA", "4C", "24", "87", "BF", "18", "3E", "22", "F0", "51", "EC", "61", "17"},
                {"16", "5E", "AF", "D3", "49", "A6", "36", "43", "F4", "47", "91", "DF", "33", "93", "21", "3B"},
                {"79", "B7", "97", "85", "10", "B5", "BA", "3C", "B6", "70", "D0", "06", "A1", "FA", "81", "82"},

                {"83", "7E", "7F", "80", "96", "73", "BE", "56", "9B", "9E", "95", "D9", "F7", "02", "B9", "A4"},
                {"DE", "6A", "32", "6D", "D8", "8A", "84", "72", "2A", "14", "9F", "88", "F9", "DC", "89", "9A"},
                {"FB", "7C", "2E", "C3", "8F", "B8", "65", "48", "26", "C8", "12", "4A", "CE", "E7", "D2", "62"},
                {"0C", "E0", "1F", "EF", "11", "75", "78", "71", "A5", "8E", "76", "3D", "BD", "BC", "86", "57"},
                {"0B", "28", "2F", "A3", "DA", "D4", "E4", "0F", "A9", "27", "53", "04", "1B", "FC", "AC", "E6"},
                {"7A", "07", "AE", "63", "C5", "DB", "E2", "EA", "94", "8B", "C4", "D5", "9D", "F8", "90", "6B"},
                {"B1", "0D", "D6", "EB", "C6", "0E", "CF", "AD", "08", "4E", "D7", "E3", "5D", "50", "1E", "B3"},
                {"5B", "23", "38", "34", "68", "46", "03", "8C", "DD", "9C", "7D", "A0", "CD", "1A", "41", "1C"}
            };
            var rowIndex = Convert.ToInt32(para1[0].ToString(), 16);
            var columnIndex = Convert.ToInt32(para1[1].ToString(), 16);
            return gfInversionList[rowIndex, columnIndex];
        }
    }
}
