using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Cryptography_Toolkit.Components;
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
public sealed partial class OrderPrimitiveRootPage : Page
{
    public OrderPrimitiveRootPage()
    {
        InitializeComponent();
        OrderCalcBaseANumberBox.ValueChanged += OrderCalcNumberBox_OnValueChanged;
        OrderCalcModulusNNumberBox.ValueChanged += OrderCalcNumberBox_OnValueChanged;
        PrimitiveRootCalcModulusNNumberBox.ValueChanged += PrimitiveRootCalcNumberBox_OnValueChanged;
        OrderCalcSetup();
        PrimitiveRootCalcSetup();
    }

    private void OrderCalcNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        OrderCalcSetup();
    }

    private void OrderCalcSetup()
    {
        var millerRabinPrimalityTest = new Components.MillerRabinPrimalityTest();
        
        if (OrderCalcBaseANumberBox.Value is double paraAValue &&
            OrderCalcModulusNNumberBox.Value is double modulusPValue &&
            millerRabinPrimalityTest.MillerRabinPrimalityTestRun(8, (int)modulusPValue))
        {
            var paraA = (int)paraAValue;
            var modulusP = (int)modulusPValue;
            var common = new Helpers.Common();
        
            int phi = common.EulerPhiFunctionCalc(modulusP);
            OrderCalcEulerPhiTextBlock.Text = $"¦Õ({modulusP}) = {phi}";
        
            int order = -1;
            string log = $"Finding the order of {paraA} modulo {modulusP}:\n";
            for (int k = 1; k <= phi; k++)
            {
                int result = common.SquareMultiplyAlgorithmCalc(paraA, k, modulusP);
                log += $"{paraA}^{k} mod {modulusP} = {result}\n";
                if (result == 1)
                {
                    order = k;
                    break;
                }
            }
        
            if (order != -1)
            {
                OrderCalcOrderTextBlock.Text = $"{order}";
                OrderCalcLogTextBox.Text = log + $"Order found: {order}";
                OrderCalcCheckInfoBar.Severity = InfoBarSeverity.Success;
                OrderCalcCheckInfoBar.Message = "Order calculation succeeded.";
            }
            else
            {
                OrderCalcOrderTextBlock.Text = "Order not found.";
                OrderCalcLogTextBox.Text = log + "Order not found.";
                OrderCalcCheckInfoBar.Severity = InfoBarSeverity.Warning;
                OrderCalcCheckInfoBar.Message = "Order not found for the given parameters.";
            }
        }
        else
        {
            OrderCalcCheckInfoBar.Severity = InfoBarSeverity.Warning;
            OrderCalcCheckInfoBar.Message = "Please enter correct parameters.";
            OrderCalcOrderTextBlock.Text = string.Empty;
            OrderCalcEulerPhiTextBlock.Text = string.Empty;
            OrderCalcLogTextBox.Text = string.Empty;
        }
    }

    private List<int> PrimitiveRootFindPrimeFactor(int m)
    {
        List<int> Prime_List = new List<int>();
        for (int pri = 2; pri < m; pri++)
        {
            while (m % pri == 0 && pri != m)
            {
                if (Prime_List.IndexOf(pri) == -1)
                    Prime_List.Add(pri);
                m /= pri;
            }
        }
        if (Prime_List.IndexOf(m) == -1)
            Prime_List.Add(m);
        return Prime_List;
    }

    private void PrimitiveRootCalcNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        PrimitiveRootCalcSetup();
    }

    private void PrimitiveRootCalcSetup()
    {
        var millerRabinPrimalityTest = new Components.MillerRabinPrimalityTest();
        
        if (PrimitiveRootCalcModulusNNumberBox.Value is double modulusNValue)
        {
            var modulusN = (int)modulusNValue;
            var common = new Helpers.Common();
            string log = $"Finding primitive roots modulo {modulusN}:\n";
        
            // Check if modulusN is prime
            if (!millerRabinPrimalityTest.MillerRabinPrimalityTestRun(8, modulusN))
            {
                PrimitiveRootCalcCheckInfoBar.Severity = InfoBarSeverity.Warning;
                PrimitiveRootCalcCheckInfoBar.Message = "Modulus n must be a prime number.";
                PrimitiveRootCalcCountTextBlock.Text = string.Empty;
                PrimitiveRootCalcEulerTotientTextBlock.Text = string.Empty;
                PrimitiveRootCalcLogTextBox.Text = string.Empty;
                return;
            }
        
            int phi = common.EulerPhiFunctionCalc(modulusN);
            PrimitiveRootCalcEulerTotientTextBlock.Text = $"¦Õ({modulusN}) = {phi}";
        //
        //     List<int> Phi_m_Prime_List = PrimitiveRoot_Find_PrimeFactor(EulerPhi_Num);
        //     List<int> Phim_qi_List = new List<int>();
        //     foreach (var Item in Phi_m_Prime_List)
        //     {
        //         Phim_qi_List.Add(EulerPhi_Num / Item);
        //         PrimitiveRoot_Prime_List_Items.Add(new PrimitiveRoot_Prime_List_Item
        //         {
        //             PrimitiveRoot_qi = Item,
        //             PrimitiveRoot_phim_qi = EulerPhi_Num / Item
        //         });
        //     }
        //
        //     int g = 1;
        //     while (true)
        //     {
        //         if (PrimitiveRoot_EEA_Calc(g, PrimitiveRoot_m) != 1)
        //         {
        //             g++;
        //             continue;
        //         }
        //         string Single_Line = "";
        //         bool Single_Check = true;
        //         foreach (var Item in Phim_qi_List)
        //         {
        //             long Single_Calc = PrimitiveRoot_SM_Calc(g, Item, PrimitiveRoot_m);
        //             Single_Line += Single_Calc + ", ";
        //             if (Single_Calc == 1)
        //                 Single_Check = false;
        //         }
        //         PrimitiveRoot_Check_Items.Add(new PrimitiveRoot_Check_Item
        //         {
        //             PrimitiveRoot_g = g,
        //             PrimitiveRoot_Single = Single_Line,
        //             PrimitiveRoot_Check = Single_Check
        //         });
        //         if (Single_Check)
        //             break;
        //         g++;
        //     }
        //
        //     int PrimitiveRoot_MinRoot = g;
        //     int PrimitiveRoot_RootCount = PrimitiveRoot_EulerPhi_Calc(EulerPhi_Num);
        //     PrimitiveRoot_MinRoot_TextBox.Text += PrimitiveRoot_MinRoot;
        //     PrimitiveRoot_RootCount_TextBox.Text += PrimitiveRoot_RootCount;
        //
        //     int Root_Index = 1;
        //     for (int t_Test = 1; t_Test < EulerPhi_Num; t_Test++)
        //     {
        //         if (PrimitiveRoot_EEA_Calc(t_Test, EulerPhi_Num) != 1)
        //             continue;
        //         int Root_Num = PrimitiveRoot_SM_Calc(PrimitiveRoot_MinRoot, t_Test, PrimitiveRoot_m);
        //         PrimitiveRoot_Display_Items.Add(new PrimitiveRoot_Display_Item
        //         {
        //             PrimitiveRoot_Index = Root_Index,
        //             PrimitiveRoot_t = t_Test,
        //             PrimitiveRoot_Num = Root_Num
        //         });
        //         Root_Index++;
        //     }

            List<int> primitiveRoots = new();
            List<int> divisors = new();
            for (int i = 1; i * i <= phi; i++)
            {
                if (phi % i == 0)
                {
                    divisors.Add(i);
                    if (i != phi / i)
                        divisors.Add(phi / i);
                }
            }
            divisors = divisors.Where(d => d != phi).ToList();

            for (int g = 2; g < modulusN; g++)
            {
                bool isPrimitiveRoot = true;
                foreach (var d in divisors)
                {
                    if (common.SquareMultiplyAlgorithmCalc(g, d, modulusN) == 1)
                    {
                        isPrimitiveRoot = false;
                        break;
                    }
                }
                if (isPrimitiveRoot)
                {
                    primitiveRoots.Add(g);
                    log += $"{g} is a primitive root.\n";
                }
            }

            PrimitiveRootCalcCountTextBlock.Text = $"{primitiveRoots.Count}";
            log += $"Total primitive roots: {primitiveRoots.Count}\n";
            PrimitiveRootCalcLogTextBox.Text = log;
            PrimitiveRootCalcCheckInfoBar.Severity = InfoBarSeverity.Success;
            PrimitiveRootCalcCheckInfoBar.Message = "Primitive root calculation succeeded.";
        }
        else
        {
            PrimitiveRootCalcCheckInfoBar.Severity = InfoBarSeverity.Warning;
            PrimitiveRootCalcCheckInfoBar.Message = "Please enter correct parameters.";
            PrimitiveRootCalcCountTextBlock.Text = string.Empty;
            PrimitiveRootCalcEulerTotientTextBlock.Text = string.Empty;
            PrimitiveRootCalcLogTextBox.Text = string.Empty;
        }
    }
}
