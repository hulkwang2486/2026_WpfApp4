using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace _2026_WpfApp4
{
    public partial class MainWindow : Window
    {
        // 儲存飲料資料 <品名, 單價>
        private Dictionary<string, int> drinks = new Dictionary<string, int>();
        // 儲存目前訂購 <品名, 數量>
        private Dictionary<string, int> orders = new Dictionary<string, int>();

        private string resultMessage = "";
        private string typeMessage = "內用";

        public MainWindow()
        {
            InitializeComponent();
            // 不再於啟動時自動讀取 CSV，改由使用者按下按鈕觸發
        }

        /// <summary>
        /// 按鈕事件：開啟 CSV 菜單檔案
        /// </summary>
        private void OpenMenuButton_Click(object sender, RoutedEventArgs e)
        {
            // 清空舊菜單資料與舊 UI，避免重複載入時錯誤
            drinks.Clear();
            DrinkMenuStackPanel.Children.Clear();

            // 1. 讀取 CSV
            AddDrinkItems(drinks);

            // 2. 動態繪製 UI 並設定 Binding
            DisplayDrinkMenu(drinks);
        }

        /// <summary>
        /// 使用 OpenFileDialog 選擇並讀取 CSV 檔案
        /// </summary>
        private void AddDrinkItems(Dictionary<string, int> drinks)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "請選擇飲料品項 CSV 檔案",
                Filter = "CSV 檔案 (*.csv)|*.csv|所有檔案 (*.*)|*.*"
            };

            if (ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;

                // 讀取 CSV 檔案所有行 (指定 UTF-8 避免中文亂碼)
                string[] lines = File.ReadAllLines(fileName, Encoding.UTF8);

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] tokens = line.Split(',');
                    if (tokens.Length >= 2 && int.TryParse(tokens[1].Trim(), out int price))
                    {
                        string drinkName = tokens[0].Trim();
                        drinks[drinkName] = price;
                    }
                }
            }
        }

        /// <summary>
        /// 動態建立視窗元件與設定資料繫結
        /// </summary>
        private void DisplayDrinkMenu(Dictionary<string, int> drinks)
        {
            DrinkMenuStackPanel.Children.Clear();

            foreach (var drink in drinks)
            {
                // 單一行容器
                StackPanel sp = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(2),
                    Height = 35,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = Brushes.AntiqueWhite
                };

                // 勾選框 (飲料名稱)
                CheckBox cb = new CheckBox
                {
                    Content = drink.Key,
                    FontFamily = new FontFamily("微軟正黑體"),
                    Width = 160,
                    FontSize = 16,
                    Margin = new Thickness(10, 0, 10, 0),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.DarkBlue
                };

                // 單價標籤
                Label lb_price = new Label
                {
                    Content = $"{drink.Value} 元",
                    FontFamily = new FontFamily("微軟正黑體"),
                    Width = 70,
                    FontSize = 16,
                    Margin = new Thickness(0, 0, 10, 0),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.DarkCyan
                };

                // 數量微調滑桿
                Slider sl = new Slider
                {
                    Width = 120,
                    Minimum = 0,
                    Maximum = 20,
                    Value = 0,
                    IsSnapToTickEnabled = true,
                    TickFrequency = 1,
                    VerticalAlignment = VerticalAlignment.Center
                };

                // 數量顯示標籤
                Label lb_amount = new Label
                {
                    Content = "0",
                    FontFamily = new FontFamily("微軟正黑體"),
                    Width = 40,
                    FontSize = 16,
                    Margin = new Thickness(10, 0, 10, 0),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.DarkGreen
                };

                // 動態設定 Slider 與 Label 的資料繫結 (Data Binding)
                Binding binding = new Binding("Value")
                {
                    Source = sl,
                    Mode = BindingMode.OneWay
                };
                lb_amount.SetBinding(Label.ContentProperty, binding);

                // 將元件依序加入橫向 StackPanel
                sp.Children.Add(cb);
                sp.Children.Add(lb_price);
                sp.Children.Add(sl);
                sp.Children.Add(lb_amount);

                // 加入至主畫面的 StackPanel
                DrinkMenuStackPanel.Children.Add(sp);
            }
        }

        /// <summary>
        /// 計算訂單並使用 SaveFileDialog 儲存檔案
        /// </summary>
        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            orders.Clear();
            resultMessage = "";

            double total = 0.0;
            string discountMessage = "無折扣";
            int index = 1;
            double sellPrice = 0.0;

            // 走訪動態產生的 UI 擷取選擇結果
            for (int i = 0; i < DrinkMenuStackPanel.Children.Count; i++)
            {
                if (DrinkMenuStackPanel.Children[i] is StackPanel sp)
                {
                    CheckBox cb = sp.Children[0] as CheckBox;
                    Slider sl = sp.Children[2] as Slider;

                    if (cb != null && sl != null)
                    {
                        int quantity = (int)sl.Value;

                        if (cb.IsChecked == true && quantity > 0)
                        {
                            string drinkName = cb.Content.ToString();
                            orders[drinkName] = quantity;
                        }
                    }
                }
            }

            if (orders.Count == 0)
            {
                MessageBox.Show("未勾選任何飲料或訂購數量為 0！", "提示");
                return;
            }

            // 彙整訂單內容
            resultMessage += $"訂購方式：{typeMessage}\n------------------------------\n訂購清單：\n";
            foreach (var item in orders)
            {
                string drinkName = item.Key;
                int price = drinks[drinkName];
                int quantity = item.Value;

                int subTotal = price * quantity;
                total += subTotal;
                resultMessage += $"{index}. {drinkName}：{price} 元 x {quantity} 杯 = {subTotal} 元\n";
                index++;
            }

            // 折扣計算（加入滿額條件說明）
            if (total >= 500)
            {
                discountMessage = "滿 500 元打 8 折";
                sellPrice = total * 0.8;
            }
            else if (total >= 300)
            {
                discountMessage = "滿 300 元打 85 折";
                sellPrice = total * 0.85;
            }
            else if (total >= 200)
            {
                discountMessage = "滿 200 元打 9 折";
                sellPrice = total * 0.9;
            }
            else
            {
                discountMessage = "未滿 200 元（無折扣）";
                sellPrice = total;
            }

            resultMessage += $"------------------------------\n小計：{total} 元\n優惠折扣：{discountMessage}\n實付金額：{sellPrice} 元\n";
            ResultTextBlock.Text = resultMessage;

            // 使用 SaveFileDialog 寫入訂單明細檔案
            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "儲存訂單明細",
                Filter = "文字檔案 (*.txt)|*.txt|所有檔案 (*.*)|*.*",
                FileName = $"Order_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            if (sfd.ShowDialog() == true)
            {
                string fileName = sfd.FileName;
                File.WriteAllText(fileName, resultMessage, Encoding.UTF8);
                MessageBox.Show("訂單已成功儲存！", "提示");
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                typeMessage = rb.Content.ToString();
            }
        }
    }
}