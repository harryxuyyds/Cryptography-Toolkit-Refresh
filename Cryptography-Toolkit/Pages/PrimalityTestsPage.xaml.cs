using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using Cryptography_Toolkit.Helpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PrimalityTestsPage : Page
{
    private readonly Common _common;

    public PrimalityTestsPage()
    {
        InitializeComponent();
        _common = new Common();
        MillerRabinPrimalityTestSecurityParameterNumberBox.ValueChanged += MillerRabinPrimalityTestNumberBox_OnValueChanged;
        MillerRabinPrimalityTestPrimeCandidateNumberBox.ValueChanged += MillerRabinPrimalityTestNumberBox_OnValueChanged;
    }

    private void MillerRabinPrimalityTestExecuteButton_OnClick(object sender, RoutedEventArgs e)
    {
        // 获取素数候选数
        if (MillerRabinPrimalityTestPrimeCandidateNumberBox.Value is double candidateValue &&
            MillerRabinPrimalityTestSecurityParameterNumberBox.Value is double securityValue &&
            candidateValue > 2)
        {
            MillerRabinPrimalityTestLogTextBox.Text = string.Empty;
            var primeCandidate = (long)candidateValue;
            var securityParameter = (int)securityValue;
            var candidateStr = ((long)candidateValue).ToString();
            int candidateStrLength = candidateStr.Length;

            MillerRabinPrimalityTestLogTextBox.Text +=
                $"Miller-Rabin Primality Test for {primeCandidate} with security parameter {securityParameter}:\n";

            var temp = primeCandidate - 1;
            var paraU = 0;
            while (temp % 2 == 0)
            {
                paraU += 1;
                temp /= 2;
            }
            var paraR = temp;

            var resultCheck = 0;
            for (int index = 1; index <= securityParameter; index++)
            {
                // 生成随机数
                var randomInt = _common.GenerateRandomInt(2, (int)primeCandidate - 1);

                var millerRabinPrimalityTest = new Components.MillerRabinPrimalityTest();
                var result = millerRabinPrimalityTest.MillerRabinPrimalityTestSingle(randomInt, (int)primeCandidate, paraU, (int)paraR);
                resultCheck += result;

                if (result == 1)
                {
                    MillerRabinPrimalityTestLogTextBox.Text +=
                        $"Test Round {index,2}: Random Int = {randomInt.ToString().PadLeft(candidateStrLength)}, Miller–Rabin Primality Test Result = Pass\n";
                }
                else
                {
                    MillerRabinPrimalityTestLogTextBox.Text +=
                        $"Test Round {index,2}: Random Int = {randomInt.ToString().PadLeft(candidateStrLength)}, Miller–Rabin Primality Test Result = Fail\n";
                }
            }

            // 检查结果
            if (resultCheck != securityParameter)
            {
                MillerRabinPrimalityTestFeedbackInfoBar.Severity = InfoBarSeverity.Warning;
                MillerRabinPrimalityTestFeedbackInfoBar.Message = $"After {securityParameter} rounds primality test, {primeCandidate} is composite.";
                MillerRabinPrimalityTestFeedbackInfoBar.IsOpen = true;
            }
            else
            {
                MillerRabinPrimalityTestFeedbackInfoBar.Severity = InfoBarSeverity.Success;
                MillerRabinPrimalityTestFeedbackInfoBar.Message = $"After {securityParameter} rounds primality test, {primeCandidate} is likely prime.";
                MillerRabinPrimalityTestFeedbackInfoBar.IsOpen = true;
            }
        }
        else
        {
            // 处理无效输入
            MillerRabinPrimalityTestFeedbackInfoBar.Severity = InfoBarSeverity.Error;
            MillerRabinPrimalityTestFeedbackInfoBar.Message = "Please enter correct parameters.";
            MillerRabinPrimalityTestFeedbackInfoBar.IsOpen = true;
        }
    }

    private void MillerRabinPrimalityTestNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        MillerRabinPrimalityTestLogTextBox.Text =
            "The running log generated during the primality test will be displayed here.";
        MillerRabinPrimalityTestFeedbackInfoBar.IsOpen = true;
        MillerRabinPrimalityTestFeedbackInfoBar.Severity = InfoBarSeverity.Informational;
        MillerRabinPrimalityTestFeedbackInfoBar.Message = "The final result of the primality test will be displayed here.";
    }

    private void FermatPrimalityTestExecuteButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (FermatPrimalityTestPrimeCandidateNumberBox.Value is double candidateValue &&
            FermatPrimalityTestSecurityParameterNumberBox.Value is double securityValue &&
            candidateValue > 2)
        {
            FermatPrimalityTestLogTextBox.Text = string.Empty;
            var primeCandidate = (long)candidateValue;
            var securityParameter = (int)securityValue;
            var candidateStr = ((long)candidateValue).ToString();
            int candidateStrLength = candidateStr.Length;

            FermatPrimalityTestLogTextBox.Text +=
                $"Fermat Primality Test for {primeCandidate} with security parameter {securityParameter}:\n";

            int resultCheck = 0;
            for (int index = 1; index <= securityParameter; index++)
            {
                // 生成随机数
                var randomInt = _common.GenerateRandomInt(2, (int)primeCandidate - 1);

                // 计算 a^(n-1) mod n
                int modResult = _common.SquareMultiplyAlgorithmCalc(randomInt, (int)primeCandidate - 1, (int)primeCandidate);

                if (modResult == 1)
                {
                    FermatPrimalityTestLogTextBox.Text +=
                        $"Test Round {index,2}: Random Int = {randomInt.ToString().PadLeft(candidateStrLength)}, Fermat Test Result = Pass\n";
                    resultCheck++;
                }
                else
                {
                    FermatPrimalityTestLogTextBox.Text +=
                        $"Test Round {index,2}: Random Int = {randomInt.ToString().PadLeft(candidateStrLength)}, Fermat Test Result = Fail\n";
                }
            }

            // 检查结果
            if (resultCheck != securityParameter)
            {
                FermatPrimalityTestFeedbackInfoBar.Severity = InfoBarSeverity.Warning;
                FermatPrimalityTestFeedbackInfoBar.Message = $"After {securityParameter} rounds primality test, {primeCandidate} is composite.";
                FermatPrimalityTestFeedbackInfoBar.IsOpen = true;
            }
            else
            {
                FermatPrimalityTestFeedbackInfoBar.Severity = InfoBarSeverity.Success;
                FermatPrimalityTestFeedbackInfoBar.Message = $"After {securityParameter} rounds primality test, {primeCandidate} is likely prime.";
                FermatPrimalityTestFeedbackInfoBar.IsOpen = true;
            }
        }
        else
        {
            // 处理无效输入
            FermatPrimalityTestFeedbackInfoBar.Severity = InfoBarSeverity.Error;
            FermatPrimalityTestFeedbackInfoBar.Message = "Please enter correct parameters.";
            FermatPrimalityTestFeedbackInfoBar.IsOpen = true;
        }
    }
}
