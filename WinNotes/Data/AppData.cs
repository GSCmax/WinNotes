namespace WinNotes.Data
{
    internal class AppData
    {
        public static readonly string SavePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WinNotes");
        public static readonly string SaveName = "Data.xamlpkg";

        public byte[]? DocumentBytes { get; set; }
    }
}
