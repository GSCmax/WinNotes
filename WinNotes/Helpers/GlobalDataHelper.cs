using Newtonsoft.Json;
using System.IO;
using WinNotes.Data;

namespace WinNotes.Helpers
{
    static class GlobalDataHelper
    {
        public static AppConfig? appConfig;
        public static AppData? appData;

        public static string ConfigFile = Path.Combine(AppConfig.SavePath, AppConfig.SaveName);
        public static string DataFile = Path.Combine(AppData.SavePath, AppData.SaveName);

        public static void Init()
        {
            if (File.Exists(ConfigFile))
                try
                {
                    var json = File.ReadAllText(ConfigFile);
                    appConfig = (string.IsNullOrEmpty(json) ? new AppConfig() : JsonConvert.DeserializeObject<AppConfig>(json)) ?? new AppConfig();
                }
                catch
                {
                    appConfig = new AppConfig();
                }
            else
                appConfig = new AppConfig();

            if (File.Exists(DataFile))
                try
                {
                    var data = File.ReadAllBytes(DataFile);
                    appData = new() { DocumentBytes = data };
                }
                catch
                {
                    appData = new AppData { DocumentBytes = Array.Empty<byte>() };
                }
            else
                appData = new AppData { DocumentBytes = Array.Empty<byte>() };
        }

        public static void Save()
        {
            var json1 = JsonConvert.SerializeObject(appConfig, Formatting.Indented);
            Directory.CreateDirectory(AppConfig.SavePath);
            File.WriteAllText(ConfigFile, json1);

            Directory.CreateDirectory(AppData.SavePath);
            File.WriteAllBytes(DataFile, appData?.DocumentBytes ?? Array.Empty<byte>());
        }
    }
}
