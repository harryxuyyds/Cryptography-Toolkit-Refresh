using System;
using System.Collections.Generic;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ShiftRegisterBasedStreamCiphersPage : Page
{
    public ShiftRegisterBasedStreamCiphersPage()
    {
        InitializeComponent();
        LfsrDegreeNumberBox.ValueChanged += LfsrDegreeNumberBox_OnValueChanged;
        LfsrFeedbackCoefficientsTextBox.TextChanged += LfsrTextBox_OnTextChanged;
        LfsrInitialValuesTextBox.TextChanged += LfsrTextBox_OnTextChanged;
        LfsrSetup();
    }

    private void LfsrDegreeNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        LfsrSetup();
    }

    private void LfsrTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        LfsrSetup();
    }

    private void LfsrSetup()
    {
        var shiftRegisterBasedStreamCipher = new Components.ShiftRegisterBasedStreamCipher();
        var feedbackCoefficients = LfsrFeedbackCoefficientsTextBox.Text.Trim();
        var initialValues = LfsrInitialValuesTextBox.Text.Trim();
        var degreeValue = (int)LfsrDegreeNumberBox.Value;

        // 检查是否为空
        if (string.IsNullOrEmpty(feedbackCoefficients) || string.IsNullOrEmpty(initialValues))
        {
            // 这里可以显示错误信息或处理异常
            LfsrStatusInfoBar.Severity = InfoBarSeverity.Warning;
            LfsrStatusInfoBar.Message = "Feedback coefficients and initial values cannot be empty.";
            return;
        }

        // 检查是否只包含0和1
        if (!feedbackCoefficients.All(c => c == '0' || c == '1') ||
            !initialValues.All(c => c == '0' || c == '1'))
        {
            // 这里可以显示错误信息或处理异常
            LfsrStatusInfoBar.Severity = InfoBarSeverity.Warning;
            LfsrStatusInfoBar.Message = "Feedback coefficients and initial values must only contain 0 and 1.";
            return;
        }

        // 检查长度是否等于degreeValue
        if (feedbackCoefficients.Length != degreeValue || initialValues.Length != degreeValue)
        {
            // 这里可以显示错误信息或处理异常
            LfsrStatusInfoBar.Severity = InfoBarSeverity.Warning;
            LfsrStatusInfoBar.Message = "The length of feedback coefficients and initial values must be equal to the degree value.";
            return;
        }

        var result = shiftRegisterBasedStreamCipher.LinearFeedbackShiftRegistersRun(degreeValue, feedbackCoefficients, initialValues);
        var log = new System.Text.StringBuilder();

        log.AppendLine("Linear Feedback Shift Registers Run Log");
        log.AppendLine(new string('=', 40));
        log.AppendLine($"Degree: {degreeValue}");
        log.AppendLine($"Feedback Coefficients: {feedbackCoefficients}");
        log.AppendLine($"Initial Values: {initialValues}");
        log.AppendLine($"Total Rounds: {result.Length}");
        log.AppendLine();

        log.AppendLine($"{"Round",-8} | {"Register State",-20} | {"Output",-10}");
        log.AppendLine(new string('-', 45));

        foreach (var item in result.LogItems)
        {
            log.AppendLine($"{item.roundIndex,-8} | {item.roundSequence,-20} | {item.roundResult,-10}");
        }

        LfsrLogTextBox.Text = log.ToString();
        LfsrStatusInfoBar.Severity = InfoBarSeverity.Success;
        LfsrStatusInfoBar.Message = $"Linear feedback shift registers executed successfully. Length: {result.Length}.";
    }
}
