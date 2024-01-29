using MVRB.Models;
using Newtonsoft.Json;
using Serilog;

namespace MVRB.Configuration
{
    public class BotConfig
    {
        private readonly string _configDirectory = "configuration\\";
        private readonly string _configFileName = "configuration\\config.json";

        #region Global Settings
        public string Prefix { get; set; } = "!";
        public bool UseUpdater { get; set; } = true;
        #endregion

        #region Discord Settings
        public string Token { get; set; } = string.Empty;
        #endregion

        #region Guild Settings
        public List<ulong> ConfiguredChannelIds { get; set; } = new List<ulong>();
        public List<RoleTiers> RoleTiers { get; set; } = new List<RoleTiers>();
        #endregion

        public async Task InitConfiguration()
        {
            if (File.Exists(_configFileName))
            {
                Log.Information("Loading configuration data...");
                await LoadConfigurationAsync();
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
                File.Create(_configFileName).Dispose();
                SaveConfiguration();
            }
            catch (IOException error)
            {
                Log.Error("Unable to create the configuration file.\n{error}\n{stackTrace}", error, error.StackTrace);
            }
        }

        public void SaveConfiguration()
        {
            File.WriteAllText(_configFileName, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public void AddConfiguredChannelIds(ulong channelId)
        {
            if (ConfiguredChannelIds.Contains(channelId))
            {
                Log.Information("Channel with ID {channelId} already exists. Cannot add duplicate channel.", channelId);
            }
            else
            {
                ConfiguredChannelIds.Add(channelId);
                SaveConfiguration();
                Log.Information("Channel with ID {ChannelId} added successfully.", channelId);
            }
        }

        public void RemoveConfiguredChannelIds(ulong channelId)
        {
            ConfiguredChannelIds.Remove(channelId);
            SaveConfiguration();
        }

        public void AddRoleTier(ulong roleId, int requestLimit)
        {
            if (RoleTiers.Any(rt => rt.RoleId == roleId))
            {
                Log.Information("Role with ID {roleId} already exists. Cannot add duplicate role.", roleId);
            }
            else
            {
                RoleTiers.Add(new RoleTiers { RoleId = roleId, RequestLimit = requestLimit });
                SaveConfiguration();
                Log.Information("Role with ID {roleId} added successfully.", roleId);
            }
        }

        public void RemoveRoleTier(ulong roleId)
        {
            RoleTiers.RemoveAll(RoleTiers => RoleTiers.RoleId == roleId);
            SaveConfiguration();
        }

        private async Task LoadConfigurationAsync()
        {
            var json = await File.ReadAllTextAsync(_configFileName);
            JsonConvert.PopulateObject(json, this);
            Log.Information("Loaded configuration!");
            SaveConfiguration();
        }
    }
}