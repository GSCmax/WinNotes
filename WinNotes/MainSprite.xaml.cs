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

            // Windows11 不需要白色背景了，直接透明就行了
            if (Environment.OSVersion.Version.Build >= 22000)
                Background = System.Windows.Media.Brushes.Transparent;

            WindowBackdropHelper.Apply(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (GlobalDataHelper.appData?.DocumentBytes != null)
            {
                try
                {
                    TextRange range = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                    using MemoryStream ms = new MemoryStream(GlobalDataHelper.appData.DocumentBytes);
                    range.Load(ms, System.Windows.DataFormats.XamlPackage);
                }
                catch
                {
                    rtb.Document = new FlowDocument();
                }
            }
        }

        private void Window_Activated(object sender, EventArgs e) => rtb.Focus();

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
            e.Handled = true;
        }

        private void FontAdjust_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (rtb == null) return;
            if (e.Delta > 0)
            {
                EditingCommands.IncreaseFontSize.Execute(null, rtb);
            }
            else
            {
                EditingCommands.DecreaseFontSize.Execute(null, rtb);
            }
            e.Handled = true;
        }
    }
}
