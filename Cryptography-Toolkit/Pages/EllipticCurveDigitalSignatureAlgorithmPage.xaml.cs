using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices.WindowsRuntime;
using Cryptography_Toolkit.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class EllipticCurveDigitalSignatureAlgorithmPage : Page
{
    private readonly Common _common;

    private int _ecCoeffA, _ecCoeffB, _ecModulusP;

    public EllipticCurveDigitalSignatureAlgorithmPage()
    {
        InitializeComponent();
        _common = new Common();
        DefinitionEcCoefficientANumberBox.ValueChanged += DefinitionEcNumberBox_OnValueChanged;
        DefinitionEcCoefficientBNumberBox.ValueChanged += DefinitionEcNumberBox_OnValueChanged;
        DefinitionEcFieldModulusNumberBox.ValueChanged += DefinitionEcNumberBox_OnValueChanged;
        DefinitionEcSetup();
    }

    private void DefinitionEcNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        DefinitionEcSetup();
    }

    private void DefinitionEcSetup()
    {
        var millerRabinPrimalityTest = new Components.MillerRabinPrimalityTest();

        if (DefinitionEcCoefficientANumberBox.Value is double ecCoeffAValue &&
            DefinitionEcCoefficientBNumberBox.Value is double ecCoeffBValue &&
            DefinitionEcFieldModulusNumberBox.Value is double ecModulusPValue &&
            millerRabinPrimalityTest.MillerRabinPrimalityTestRun(8, (int)ecModulusPValue))
        {
            _ecCoeffA = (int)ecCoeffAValue;
            _ecCoeffB = (int)ecCoeffBValue;
            _ecModulusP = (int)ecModulusPValue;

            var check = (4 * (int)Math.Pow(_ecCoeffA, 3) + 27 * (int)Math.Pow(_ecCoeffB, 2)) % _ecModulusP;
            if (check != 0)
            {
                DefinitionEcCheckInfoBar.Severity = InfoBarSeverity.Success;
                DefinitionEcCheckInfoBar.Message = $"Valid Curve (4a³ + 27b² ≡ {check} ≢ 0 mod p)​";
                LoadEllipticCurvePreview(_ecCoeffA, _ecCoeffB, _ecModulusP);
            }
            else
            {
                DefinitionEcCheckInfoBar.Severity = InfoBarSeverity.Error;
                DefinitionEcCheckInfoBar.Message = "Invalid Curve (4a³ + 27b² ≡ 0 mod p)​";
            }
        }
        else
        {
            DefinitionEcCheckInfoBar.Severity = InfoBarSeverity.Warning;
            DefinitionEcCheckInfoBar.Message = "Please enter correct parameters.";
        }
    }

    private void LoadEllipticCurvePreview(int coeffA, int coeffB, int modulusP)
    {
        var orderCount = 1;
        var points = new List<(int x, int y)>();

        for (int x = 0; x < modulusP; x++)
        {
            int rhs = (int)((((long)x * x % modulusP) * x % modulusP + coeffA * x % modulusP + coeffB) % modulusP);
            for (int y = 0; y < modulusP; y++)
            {
                if ((y * y) % modulusP == rhs)
                {
                    // 保存点坐标到列表
                    points.Add((x, y));
                    orderCount++;
                }
            }
        }

        DefinitionEcOrderTextBlock.Text = orderCount.ToString();

        PointPComboBox.Items.Clear();
        foreach (var (x, y) in points)
        {
            var display = $"({x}, {y})";
            PointPComboBox.Items.Add(display);
        }

        PointPComboBox.SelectionChanged += PointComboBox_OnSelectionChanged;
    }

    private void PointComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        KeyGenerationKeyPubQTextBlock.Text = String.Empty;
        CurveGeneratorOrderSetup();
    }

    private void CurveGeneratorOrderSetup()
    {
        var ellipticCurveHelper = new Helpers.EllipticCurveHelper();

        if (PointPComboBox.SelectedItem is not string selected || !selected.StartsWith("(") || !selected.EndsWith(")"))
            return;

        var trimmed = selected.Trim('(', ')').Split(',');
        if (trimmed.Length != 2 ||
            !int.TryParse(trimmed[0].Trim(), out int pointGx) ||
            !int.TryParse(trimmed[1].Trim(), out int pointGy))
            return;

        int pointPx = pointGx;
        int pointPy = pointGy;
        int k = 1;
        int slope = ellipticCurveHelper.PointSlopeCalc(pointGx, pointGy, pointPx, pointPy, _ecCoeffA, _ecModulusP);

        while (slope != -1)
        {
            k++;
            int pointPxPre = pointPx;
            int pointPyPre = pointPy;
            pointPx = (slope * slope - pointPxPre - pointGx) % _ecModulusP;
            pointPy = (slope * (pointPxPre - pointPx) - pointPyPre) % _ecModulusP;
            while (pointPx < 0)
                pointPx = (pointPx + _ecModulusP) % _ecModulusP;
            while (pointPy < 0)
                pointPy = (pointPy + _ecModulusP) % _ecModulusP;
            slope = ellipticCurveHelper.PointSlopeCalc(pointGx, pointGy, pointPx, pointPy, _ecCoeffA, _ecModulusP);
        }
        KeyGenerationCurveOrderTextBlock.Text = (k + 1).ToString();
        KeyGenerationKeyPrNumberBox.Maximum = k;
        SignatureGenerationEphemeralKeyNumberBox.Maximum = k;
    }

    private void KeyGenerationKeyPrNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        KeyPubGenerationSetup();
    }

    private void KeyPubGenerationSetup()
    {
        var ellipticCurveHelper = new Helpers.EllipticCurveHelper();

        // 获取选中的基点
        if (PointPComboBox.SelectedItem is not string selected || !selected.StartsWith("(") || !selected.EndsWith(")"))
            return;

        var trimmed = selected.Trim('(', ')').Split(',');
        if (trimmed.Length != 2 ||
            !int.TryParse(trimmed[0].Trim(), out int pointGx) ||
            !int.TryParse(trimmed[1].Trim(), out int pointGy))
            return;

        // 获取私钥
        var keyPr = (int)KeyGenerationKeyPrNumberBox.Value;

        // 计算公钥 keyPr * G
        int[] pubKey = { pointGx, pointGy };
        int[] temp = { pointGx, pointGy };
        for (var i = 1; i < keyPr; i++)
        {
            pubKey = ellipticCurveHelper.PointAddCalc(pubKey[0], pubKey[1], temp[0], temp[1], _ecCoeffA, _ecModulusP);
        }

        // 显示公钥
        KeyGenerationKeyPubQTextBlock.Text = $"({pubKey[0]}, {pubKey[1]})";
        DisplayEcdsaKeySet();
    }

    private void DisplayEcdsaKeySet()
    {
        // 获取参数
        int p = _ecModulusP;
        int a = _ecCoeffA;
        int b = _ecCoeffB;
        string n = KeyGenerationCurveOrderTextBlock.Text;
        string g = PointPComboBox.SelectedItem as string ?? "(?, ?)";
        string q = KeyGenerationKeyPubQTextBlock.Text;
        string d = KeyGenerationKeyPrNumberBox.Value.ToString();

        // 构造密钥对字符串

        // 改为表格样式英文输出：
        string kpub =
            "Public Key:\n" +
            "+-----+-----+-----+-----+----------------+----------------+\n" +
            "|  p  |  a  |  b  |  n  |       G        |       Q        |\n" +
            "+-----+-----+-----+-----+----------------+----------------+\n" +
            $"| {p,-3} | {a,-3} | {b,-3} | {n,-3} | {g,-14} | {q,-14} |\n" +
            "+-----+-----+-----+-----+----------------+----------------+\n";
        string kpr =
            "Private Key:\n" +
            "+-----+\n" +
            "|  d  |\n" +
            "+-----+\n" +
            $"| {d,-3} |\n" +
            "+-----+\n";

        KeyGenerationLogTextBox.Text = $"{kpub}\n{kpr}";
    }

    private void SignatureGenerationEphemeralKeyNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        SignatureGenerationSetup();
    }

    private void SignatureGenerationSetup()
    {
        // Get parameters
        int p = _ecModulusP;
        int a = _ecCoeffA;
        int b = _ecCoeffB;
        if (PointPComboBox.SelectedItem is not string selected || !selected.StartsWith("(") || !selected.EndsWith(")"))
            return;
        var trimmed = selected.Trim('(', ')').Split(',');
        if (trimmed.Length != 2 ||
            !int.TryParse(trimmed[0].Trim(), out int gx) ||
            !int.TryParse(trimmed[1].Trim(), out int gy))
            return;

        int n;
        if (!int.TryParse(KeyGenerationCurveOrderTextBlock.Text, out n) || n <= 1)
            return;

        int d = (int)KeyGenerationKeyPrNumberBox.Value; // private key
        int k = (int)SignatureGenerationEphemeralKeyNumberBox.Value; // ephemeral key k

        if (k <= 0 || k >= n)
        {
            SignatureGenerationSignatureTextBlock.Text = "Invalid k: 0 < k < n";
            return;
        }

        // Calculate k*G
        var ellipticCurveHelper = new Helpers.EllipticCurveHelper();
        int[] point = { gx, gy };
        int[] kG = { gx, gy };
        for (int i = 1; i < k; i++)
        {
            kG = ellipticCurveHelper.PointAddCalc(kG[0], kG[1], gx, gy, a, p);
        }
        int r = kG[0] % n;
        if (r == 0)
        {
            SignatureGenerationSignatureTextBlock.Text = "r = 0, choose another k";
            return;
        }

        // Get message hash m
        int m;
        if (!int.TryParse(SignatureGenerationMessageHashNumberBox.Text, out m))
        {
            SignatureGenerationSignatureTextBlock.Text = "Invalid message hash";
            return;
        }

        // Calculate modular inverse of k
        var (gcd, kInv, _) = _common.ExtendedEuclideanAlgorithmCalc(k, n);
        if (gcd != 1)
        {
            SignatureGenerationSignatureTextBlock.Text = "No inverse for k，(k, n) not coprime";
            return;
        }
        kInv = (kInv % n + n) % n;

        // Calculate s = k^(-1) * (m + d*r) mod n
        int s = (int)(((long)kInv * (m + (long)d * r % n)) % n);
        if (s == 0)
        {
            SignatureGenerationSignatureTextBlock.Text = "s = 0, choose another k";
            return;
        }

        SignatureGenerationSignatureTextBlock.Text = $"({r}, {s})";

        SignatureVerificationSetup();
    }

    private void SignatureVerificationSetup()
    {
        // Get parameters
        int p = _ecModulusP;
        int a = _ecCoeffA;
        int b = _ecCoeffB;
        if (PointPComboBox.SelectedItem is not string selected || !selected.StartsWith("(") || !selected.EndsWith(")"))
            return;
        var trimmed = selected.Trim('(', ')').Split(',');
        if (trimmed.Length != 2 ||
            !int.TryParse(trimmed[0].Trim(), out int gx) ||
            !int.TryParse(trimmed[1].Trim(), out int gy))
            return;

        int n;
        if (!int.TryParse(KeyGenerationCurveOrderTextBlock.Text, out n) || n <= 1)
            return;

        // Public key Q
        string qText = KeyGenerationKeyPubQTextBlock.Text;
        if (!qText.StartsWith("(") || !qText.EndsWith(")"))
            return;
        var qTrimmed = qText.Trim('(', ')').Split(',');
        if (qTrimmed.Length != 2 ||
            !int.TryParse(qTrimmed[0].Trim(), out int qx) ||
            !int.TryParse(qTrimmed[1].Trim(), out int qy))
            return;

        // Signature (r, s)
        string sigText = SignatureGenerationSignatureTextBlock.Text;
        if (!sigText.StartsWith("(") || !sigText.EndsWith(")"))
        {
            SignatureVerificationInfoBar.Severity = InfoBarSeverity.Error;
            SignatureVerificationInfoBar.Message = "Invalid signature format";
            return;
        }
        var sigTrimmed = sigText.Trim('(', ')').Split(',');
        if (sigTrimmed.Length != 2 ||
            !int.TryParse(sigTrimmed[0].Trim(), out int r) ||
            !int.TryParse(sigTrimmed[1].Trim(), out int s))
        {
            SignatureVerificationInfoBar.Severity = InfoBarSeverity.Error;
            SignatureVerificationInfoBar.Message = "Invalid signature format";
            return;
        }

        // Message hash m
        if (!int.TryParse(SignatureGenerationMessageHashNumberBox.Text, out int m))
        {
            SignatureVerificationInfoBar.Severity = InfoBarSeverity.Error;
            SignatureVerificationInfoBar.Message = "Invalid message hash";
            return;
        }

        // Check r, s range
        if (r <= 0 || r >= n || s <= 0 || s >= n)
        {
            SignatureVerificationInfoBar.Severity = InfoBarSeverity.Error;
            SignatureVerificationInfoBar.Message = "r, s are out of valid range";
            return;
        }

        // Compute w = s^(-1) mod n
        var (gcd, _, w) = _common.ExtendedEuclideanAlgorithmCalc(n, s);
        while (w < 0)
            w += n;
        if (gcd != 1)
        {
            SignatureVerificationInfoBar.Severity = InfoBarSeverity.Error;
            SignatureVerificationInfoBar.Message = "s and n are not coprime, cannot compute inverse";
            return;
        }
        // w = (w % n + n) % n;
        SignatureVerificationWTextBlock.Text = w.ToString();

        // Compute u = m * w mod n, uu = r * w mod n
        int u = (int)(((long)m * w) % n);
        int uu = (int)(((long)r * w) % n);
        SignatureVerificationUTextBlock.Text = u.ToString();
        SignatureVerificationUuTextBlock.Text = uu.ToString();

        // Compute u*G + uu*Q
        var ellipticCurveHelper = new Helpers.EllipticCurveHelper();
        int[] gPoint = [gx, gy];
        int[] qPoint = [qx, qy];

        // Compute u*G
        int[] uG = [gx, gy];
        if (u == 0)
        {
            uG = null;
        }
        else
        {
            uG = [gx, gy];
            for (int i = 1; i < u; i++)
            {
                uG = ellipticCurveHelper.PointAddCalc(uG[0], uG[1], gx, gy, a, p);
            }
        }

        // Compute uu*Q
        int[] uuQ = [qx, qy];
        if (uu == 0)
        {
            uuQ = null;
        }
        else
        {
            uuQ = [qx, qy];
            for (int i = 1; i < uu; i++)
            {
                uuQ = ellipticCurveHelper.PointAddCalc(uuQ[0], uuQ[1], qx, qy, a, p);
            }
        }

        // Compute P = uG + uuQ
        int[] pPoint = null;
        if (uG == null && uuQ == null)
        {
            SignatureVerificationPTextBlock.Text = "Point at infinity";
            SignatureVerificationInfoBar.Severity = InfoBarSeverity.Error;
            SignatureVerificationInfoBar.Message = "P is the point at infinity, signature invalid";
            return;
        }
        else if (uG == null)
        {
            pPoint = uuQ;
        }
        else if (uuQ == null)
        {
            pPoint = uG;
        }
        else
        {
            pPoint = ellipticCurveHelper.PointAddCalc(uG[0], uG[1], uuQ[0], uuQ[1], a, p);
        }

        SignatureVerificationPTextBlock.Text = $"({pPoint[0]}, {pPoint[1]})";

        // Verify r ≡ P.x mod n
        int pxModN = ((pPoint[0] % n) + n) % n;
        if (pxModN == r)
        {
            SignatureVerificationInfoBar.Severity = InfoBarSeverity.Success;
            SignatureVerificationInfoBar.Message = "Signature verification passed";
        }
        else
        {
            SignatureVerificationInfoBar.Severity = InfoBarSeverity.Error;
            SignatureVerificationInfoBar.Message = $"Signature verification failed (r={r}, P.x mod n={pxModN})";
        }
    }
}
