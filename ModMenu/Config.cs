using System;
using System.Collections.Generic;
using UnityEngine;

namespace LLModMenu
{
    public enum ConfigFileFormat
    {
        NONE,
        JSON,
        XML,
        INI
    }
    public class Entry
    {
        public string Key;
        public string Value;
        public string Type;
        public int Order;
        public Entry()
        {
        }

        public Entry(string key, string value, string type = "string", int order = -1)
        {
            Key = key;
            Value = value;
            Type = type;
            Order = order;
        }
    }
    public class Config
    {
        private IConfigFile configFile;

        public List<Entry> optionList = new List<Entry>();
        public Dictionary<string, KeyCode> configKeys = new Dictionary<string, KeyCode>();
        public Dictionary<string, bool> configBools = new Dictionary<string, bool>();
        public Dictionary<string, int> configInts = new Dictionary<string, int>();
        public Dictionary<string, string> configSliders = new Dictionary<string, string>();
        public Dictionary<string, string> configHeaders = new Dictionary<string, string>();
        public Dictionary<string, int> configGaps = new Dictionary<string, int>();
        public Dictionary<string, string> configText = new Dictionary<string, string>();

        public Config(string _configPath, ConfigFileFormat fileFormat = ConfigFileFormat.XML)
        {
            switch (fileFormat)
            {
                case ConfigFileFormat.XML:
                    configFile = new XMLFile(_configPath + ".xml");
                    break;
                /*case ConfigFileFormat.JSON:
                    configFile = new JsonFile(_configName + ".json");
                    break;*/
                default:
                    throw new System.NotImplementedException();
            }
            this.LoadFromFile();
        }

        public void LoadFromFile()
        {
            this.optionList = this.configFile.Load();
            this.Load(this.optionList);
        }

        public void Load(List<Entry> options)
        {
            Debug.Log("Yes? who's calling? Hello, it's " + System.Environment.StackTrace);

            this.optionList = options;
            this.configInts.Clear();
            this.configBools.Clear();
            this.configKeys.Clear();
            this.configGaps.Clear();
            this.configSliders.Clear();
            this.configHeaders.Clear();
            this.configText.Clear();

            foreach (Entry entry in options)
            {
                //Debug.Log("Loading entry: " + entry.Key + " | " + entry.Value + " | " + entry.Type);
                switch (entry.Type)
                {
                    case "int":
                        this.configInts.Add(entry.Key, Convert.ToInt32(entry.Value)); break;
                    case "bool":
                        this.configBools.Add(entry.Key, entry.Value == "True" || entry.Value == "true"); break;
                    case "key":
                        this.configKeys.Add(entry.Key, ParseKeyCode(entry.Value)); break;
                    case "gap":
                        this.configGaps.Add(entry.Key, Convert.ToInt32(entry.Value)); break;
                    case "slider":
                        this.configSliders.Add(entry.Key, entry.Value); break;
                    case "header":
                        this.configHeaders.Add(entry.Key, entry.Value); break;
                    case "text":
                        this.configText.Add(entry.Key, entry.Value); break;
                }
            }
        }
        public KeyCode ParseKeyCode(string keyCode)
        {
            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (vKey.ToString() == keyCode)
                {
                    return vKey;
                }
            }
            return KeyCode.A;
        }

        public void Save(bool willStore = true)
        {
            this.optionList.Clear();
            foreach (var config in configInts) optionList.Add(new Entry(config.Key, config.Value.ToString(), "int"));
            foreach (var config in configBools) optionList.Add(new Entry(config.Key, config.Value.ToString(), "bool"));
            foreach (var config in configKeys) optionList.Add(new Entry(config.Key, config.Value.ToString(), "key"));
            foreach (var config in configSliders) optionList.Add(new Entry(config.Key, config.Value, "slider"));
            foreach (var config in configGaps) optionList.Add(new Entry(config.Key, config.Value.ToString(), "gap"));
            foreach (var config in configHeaders) optionList.Add(new Entry(config.Key, config.Value, "header"));
            foreach (var config in configText) optionList.Add(new Entry(config.Key, config.Value, "text"));

            if (willStore)
                configFile.Store(optionList);
        }

        public void Init(List<Entry> writeQueue)
        {
            if (this.optionList.Count != writeQueue.Count)
            {
                this.Load(writeQueue);
                this.Save();
                Debug.Log("ModMenu: " + configFile.GetPath() + " has been remade because it did not match what was expected");

            }
            else
            {
                for (int i = 0;  i < this.optionList.Count; i++)
                {
                    if (this.optionList[i].Key != writeQueue[i].Key || this.optionList[i].Type != writeQueue[i].Type)
                    {
                        this.Load(writeQueue);
                        this.Save();
                        Debug.Log("ModMenu: " + configFile.GetPath() + " has been remade because it did not match what was expected");
                    }
                }
            }
        }

        public void Delete()
        {
            this.optionList.Clear();
            this.configFile.Delete();
        }

        public void Clear()
        {
            this.optionList.Clear();
            this.configFile.Clear();
        }


        public KeyCode GetKeyCode(string optionName)
        {
            return configKeys[optionName];
        }

        public bool GetBool(string optionName)
        {
            return configBools[optionName];
        }

        public int GetSliderValue(string optionName)
        {
            string[] vals = configSliders[optionName].Split('|');
            return Convert.ToInt32(vals[0]);
        }

        public int GetInt(string optionName)
        {
            return configInts[optionName];
        }
    }
}
