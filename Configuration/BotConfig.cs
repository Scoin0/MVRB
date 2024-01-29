using Newtonsoft.Json;
using Serilog;

namespace MVRB.Configuration
{
    public class BotConfig
    {
        private readonly string _configDirectory = "configuration\\";
        private readonly string _config = "configuration\\config.json";

        #region Global Settings
        public string Prefix { get; set; } = "!";
        public bool UseUpdater { get; set; } = true;
        #endregion

        #region Discord Settings
        public string Token { get; set; } = string.Empty;
        #endregion

        public async Task InitConfiguration()
        {
            if (File.Exists(_config))
            {
                Log.Information("Loading configuration data...");
                await LoadConfiguration();
            }
            else
            {
                CreateConfiguration();
            }
        }

        public void CreateConfiguration()
        {
            try
            {
                Directory.CreateDirectory(_configDirectory);
                Log.Information("Configuration file not found. Creating one...");
                using (File.Create(_config)) { }
                SaveConfiguration();
            }
            catch (IOException error)
            {
                Log.Error("Unable to create the configuration file.\n{error}\n{stackTrace}", error, error.StackTrace);
            }
        }

        private async Task LoadConfiguration()
        {
            var json = await File.ReadAllTextAsync(_config);
            JsonConvert.PopulateObject(json, this);
            Log.Information("Loaded configuration!");
            SaveConfiguration();
        }

        public void SaveConfiguration()
        {
            using (StreamWriter file = File.CreateText(_config))
            {
                JsonSerializer serializer = new()
                {
                    Formatting = Formatting.Indented
                };
                serializer.Serialize(file, this);
            }
        }
    }
}