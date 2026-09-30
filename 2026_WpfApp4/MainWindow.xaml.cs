using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace _2026_WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Dictionary<string, int> drinkMenu = new Dictionary<string, int>()
        {
            { "紅茶大杯", 60 },
            { "紅茶小杯", 35 },
            { "綠茶大杯", 50 },
            { "綠茶小杯", 25 },
            { "可樂大杯", 50 },
            { "可樂小杯", 30 }
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {

            Dictionary<string, int> orderDetails = new Dictionary<string, int>();

            foreach (var child in DrinkListStackPanel.Children)
            {
                if (child is Grid itemGrid)
                {
                    CheckBox chk = null;
                    Slider sld = null;

                    foreach (var element in itemGrid.Children)
                    {
                        if (element is CheckBox checkBox)
                        {
                            chk = checkBox;
                        }
                        else if (element is Slider slider)
                        {
                            sld = slider;
                        }
                    }

                    if (chk != null && chk.IsChecked == true && sld != null && (int)sld.Value > 0)
                    {
                        string drinkName = chk.Content.ToString();
                        int amount = (int)sld.Value;
                        orderDetails[drinkName] = amount;
                    }
                }
            }

            if (orderDetails.Count == 0)
            {
                MessageBox.Show("請勾選欲購買的飲料並選擇數量！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                ResultTextBlock.Text = "";
                return;
            }

            int originalTotal = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("【訂購明細】");

            foreach (var item in orderDetails)
            {
                string name = item.Key;
                int amount = item.Value;
                int unitPrice = drinkMenu[name];
                int subtotal = unitPrice * amount;

                originalTotal += subtotal;
                sb.AppendLine($"• {name} x {amount} 杯 ＝ {subtotal} 元 (單價: {unitPrice} 元)");
            }

            double discountRate = 1.0;
            string discountType = "內用 (原價)";

            if (rbTakeOut.IsChecked == true)
            {
                discountRate = 0.9;
                discountType = "外帶 (九折優惠)";
            }
            else if (rbEcoCup.IsChecked == true)
            {
                discountRate = 0.85;
                discountType = "自備環保杯 (八五折優惠)";
            }

            int finalTotal = (int)Math.Round(originalTotal * discountRate);

            sb.AppendLine();
            sb.AppendLine($"原始總金額：{originalTotal} 元");
            sb.AppendLine($"購買方式與折扣：{discountType}");
            sb.AppendLine($"折扣後金額：{finalTotal} 元");

            ResultTextBlock.Text = sb.ToString();
            MessageBox.Show($"訂購完成！\n應付總金額為：{finalTotal} 元", "訊息提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {

            foreach (var child in DrinkListStackPanel.Children)
            {
                if (child is Grid itemGrid)
                {
                    foreach (var element in itemGrid.Children)
                    {
                        if (element is CheckBox chk) chk.IsChecked = false;
                        if (element is Slider sld) sld.Value = 1;
                    }
                }
            }

            rbDineIn.IsChecked = true;
            ResultTextBlock.Text = "";
        }
    }
}