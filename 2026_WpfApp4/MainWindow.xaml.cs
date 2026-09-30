using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _2026_WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var targetTextBox = sender as TextBox;
            var targetStackPanel = targetTextBox.Parent as StackPanel;
            var targetNameLabel = targetStackPanel.Children[0] as Label;
            var targetPriceLabel = targetStackPanel.Children[1] as Label;

            int amount;
            bool success = int.TryParse(targetTextBox.Text, out amount);
            if (!success)
            {
                MessageBox.Show("請輸入正確數值", "輸入錯誤");
                //targetTextBox.Text = "";
            }
            else
            {
                string drinkName = targetNameLabel.Content.ToString();
                int price = Convert.ToInt32(targetPriceLabel.Content.ToString().Substring(0, 2));
                //MessageBox.Show($"您選擇了 {drinkName} ‧ 數量為 {amount} ‧ 總價為 {price * amount} 元", "訂購資訊");
                ResultTextBlock.Text += $"您選擇了 {drinkName} ‧ 數量為 {amount} ‧ 總價為 {price * amount} 元\n";
            }
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            // 檢查是否有訂購紀錄
            if (!string.IsNullOrWhiteSpace(ResultTextBlock.Text))
            {
                MessageBox.Show($"感謝您的訂購！訂購清單如下：\n\n{ResultTextBlock.Text}", "訂購成功");
            }
            else
            {
                MessageBox.Show("目前尚未輸入任何訂購數量，請先選擇飲料數量！", "提示");
            }
        }
    }
}