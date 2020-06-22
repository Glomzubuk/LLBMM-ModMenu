using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace LLModMenu
{
    public class ConfigManager
    {
        private readonly string configDirectoryPath;
        private Dictionary<string, Config> configs = new Dictionary<string, Config>();


        public ConfigManager(string _configDirectoryPath)
        {
            this.configDirectoryPath = _configDirectoryPath;
        }

        public Config GetModConfig(string modName)
        {
            if (!configs.ContainsKey(modName))
            {
                //configs.Add(modName, new Config(Path.Combine(this.configDirectoryPath, modName)));
                throw new KeyNotFoundException("This config does not exists or has not yet been initialized");
            }
            return configs[modName];
        }

        public void InitConfig(string modName, List<Entry> writeQueue)
        {
            if (!configs.ContainsKey(modName))
            {
                configs.Add(modName, new Config(Path.Combine(this.configDirectoryPath, modName)));
            }
            configs[modName].Init(writeQueue);
        }
    }
}
