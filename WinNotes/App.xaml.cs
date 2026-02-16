using HandyControl.Controls;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using WinNotes.Helpers;

namespace WinNotes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        MainSprite? mainSprite;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            GlobalDataHelper.Init();

            mainSprite = new();
            mainSprite.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            GlobalDataHelper.appConfig!.X = mainSprite!.Left;
            GlobalDataHelper.appConfig!.Y = mainSprite!.Top;
            GlobalDataHelper.appConfig!.Width = mainSprite!.Width;
            GlobalDataHelper.appConfig!.Height = mainSprite!.Height;
            GlobalDataHelper.appConfig!.Topmost = mainSprite!.Topmost;
            GlobalDataHelper.appConfig!.Opacity = Math.Round(mainSprite!.Opacity, 2);
            GlobalDataHelper.appConfig!.CanDrag = WindowAttach.GetIsDragElement(mainSprite!);

            TextRange range = new TextRange(mainSprite!.rtb.Document.ContentStart, mainSprite!.rtb.Document.ContentEnd);
            using MemoryStream ms = new MemoryStream();
            range.Save(ms, DataFormats.XamlPackage);
            GlobalDataHelper.appData!.DocumentBytes = ms.ToArray();

            GlobalDataHelper.Save();
        }
    }
}
