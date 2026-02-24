namespace WinNotes.Data
{
    internal class AppConfig
    {
        public static readonly string SavePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WinNotes");
        public static readonly string SaveName = "Config.json";

        public double X { get; set; } = 50;

        public double Y { get; set; } = 50;

        public double Width { get; set; } = 400;

        public double Height { get; set; } = 500;

        public double Opacity { get; set; } = 1;

        public bool CanDrag { get; set; } = true;

        public bool Topmost { get; set; } = true;

        public string HotKey { get; set; } = "Control+Alt,N";
    }


}
