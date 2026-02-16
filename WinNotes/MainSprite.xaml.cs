using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using WinNotes.Helpers;

namespace WinNotes
{
    /// <summary>
    /// MainSprite.xaml 的交互逻辑
    /// </summary>
    public partial class MainSprite
    {
        public MainSprite()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (GlobalDataHelper.appData?.DocumentBytes != null)
            {
                TextRange range = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                using MemoryStream ms = new MemoryStream(GlobalDataHelper.appData.DocumentBytes);
                range.Load(ms, DataFormats.XamlPackage);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void Opacity_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var o = Math.Round(Opacity, 2);
            if (e.Delta > 0)
            {
                if (o < 1.0)
                    o += 0.05;
                else
                    o = 1.0;
            }
            else
            {
                if (o > 0.5)
                    o -= 0.05;
                else
                    o = 0.5;
            }
            Opacity = o;
        }

        private void FontAdjust_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (rtb == null) return;

            if (e.Delta > 0)
            {
                // 鼠标向上滚动，放大字体
                EditingCommands.IncreaseFontSize.Execute(null, rtb);
            }
            else
            {
                // 鼠标向下滚动，缩小字体
                EditingCommands.DecreaseFontSize.Execute(null, rtb);
            }

            e.Handled = true; // 阻止事件冒泡
        }

    }
}
