using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cryptography_Toolkit.Helpers;
using Microsoft.UI.Xaml.Shapes;

namespace Cryptography_Toolkit.Components
{
    public class ShiftRegisterBasedStreamCipher
    {
        private readonly Common _common;

        public ShiftRegisterBasedStreamCipher()
        {
            _common = new Common();
        }

        public class LinearFeedbackShiftRegistersResult
        {
            public List<(int roundIndex, string roundSequence, string roundResult)> LogItems { get; set; } = new();
            public int Length { get; set; }
        }

        public LinearFeedbackShiftRegistersResult LinearFeedbackShiftRegistersRun(int degree, string feedbackCoefficients, string initialValues)
        {
            var lfsrLogItems = new List<(int, string, string)>();

            var feedbackCheck = new ArrayList();
            for (var index = 0; index < feedbackCoefficients.Length; index++)
            {
                if (feedbackCoefficients[index] == '1')
                    feedbackCheck.Add(index);
            }
            var roundIndex = 0;
            var lfsrLine = initialValues;
            for (var index = 1; index <= Math.Pow(2, degree) - 1; index++)
            {
                var roundResult = 0;
                roundIndex += 1;
                foreach (int feedbackIndex in feedbackCheck)
                {
                    roundResult ^= (lfsrLine[feedbackIndex] - '0');
                }
                lfsrLine = roundResult + lfsrLine;
                lfsrLogItems.Add((roundIndex, lfsrLine.Substring(0, lfsrLine.Length - 1), lfsrLine.Substring(lfsrLine.Length - 1)));
                lfsrLine = lfsrLine.Substring(0, lfsrLine.Length - 1);
                if (lfsrLine == initialValues)
                {
                    break;
                }
            }
            var lfsrLength = roundIndex;

            return new LinearFeedbackShiftRegistersResult
            {
                LogItems = lfsrLogItems,
                Length = lfsrLength,
            };
        }
    }
}
