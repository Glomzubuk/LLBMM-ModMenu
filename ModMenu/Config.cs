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
            //Debug.Log("Who's Loading? It's " + new System.Diagnostics.StackTrace().ToString());

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
                    case EntryType.NUMERIC:
                        this.configInts.Add(entry.Key, Convert.ToInt32(entry.Value)); break;
                    case EntryType.BOOLEAN:
                        this.configBools.Add(entry.Key, entry.Value == "True" || entry.Value == "true"); break;
                    case EntryType.KEY:
                        this.configKeys.Add(entry.Key, ParseKeyCode(entry.Value)); break;
                    case EntryType.GAP:
                        this.configGaps.Add(entry.Key, Convert.ToInt32(entry.Value)); break;
                    case EntryType.SLIDER:
                        this.configSliders.Add(entry.Key, entry.Value); break;
                    case EntryType.HEADER:
                        this.configHeaders.Add(entry.Key, entry.Value); break;
                    case EntryType.TEXT:
                        this.configText.Add(entry.Key, entry.Value); break;
                    default:
                        throw new System.NotImplementedException();
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
            foreach (Entry entry in this.optionList)
            {
                switch (entry.Type)
                {
                    case EntryType.NUMERIC:
                        entry.Value = this.configInts[entry.Key].ToString(); break;
                    case EntryType.BOOLEAN:
                        entry.Value = this.configBools[entry.Key].ToString(); break;
                    case EntryType.KEY:
                        entry.Value = this.configKeys[entry.Key].ToString(); break;
                    case EntryType.GAP:
                        entry.Value = this.configGaps[entry.Key].ToString(); break;
                    case EntryType.SLIDER:
                        entry.Value = this.configSliders[entry.Key]; break;
                    case EntryType.HEADER:
                        entry.Value = this.configHeaders[entry.Key]; break;
                    case EntryType.TEXT:
                        entry.Value = this.configText[entry.Key]; break;
                    default:
                        throw new System.NotImplementedException();
                }
            }

            if (willStore)
                configFile.Store(optionList);
        }

        public void Init(WriteQueue writeQueue)
        {
            List<Entry> entries = writeQueue.GetEntries();
            bool identical = true;
            if (this.optionList.Count != entries.Count)
            {
                identical = false;
                Debug.Log("ModMenu: optionList and writeQueue did not have the same number of entries: " + this.optionList.Count + " vs " + entries.Count);
            }
            else
            {
                for (int i = 0;  i < this.optionList.Count; i++)
                {
                    if (!this.optionList[i].Key.Equals(entries[i].Key))
                    {
                        Debug.Log("ModMenu: optionList and writeQueue had differing keys: \"" + this.optionList[i].Key + "\" vs \"" + entries[i].Key + "\"");
                        identical = false;
                        break;
                    }
                    else if (this.optionList[i].Type != entries[i].Type)
                    {
                        Debug.Log("ModMenu: optionList and writeQueue had" + this.optionList[i].Key + " differing type: " + this.optionList[i].Type + " vs " + entries[i].Type);
                        identical = false;
                        break;
                    }
                }
            }

            if (!identical)
            {
                this.Load(entries);
                this.Save();
                Debug.Log("ModMenu: " + configFile.GetPath() + " has been remade because it did not match what was expected");
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
