// Script used to connect to ModMenu
using System.Collections.Generic;
using UnityEngine;
using LLModMenu;
using System.IO;
using System;

namespace LLModMenu
{
    public class ModMenuIntegration : MonoBehaviour
    {
        private ModMenu mm;
        private bool mmAdded = false;

        public List<LLModMenu.Entry> writeQueue = new List<LLModMenu.Entry>();


        private void Start()
        {
            InitConfig();
        }

        private void Update()
        {
            if (mmAdded == false)
            {
                mm = FindObjectOfType<ModMenu>();
                if (mm != null)
                {
                    mm.mods.Add(base.gameObject.name);
                    mmAdded = true;
                }
            }
        }

        private void InitConfig()
        {
            /*
             * Mod menu now uses a single function to add options etc. (AddToWriteQueue)
             * your specified options should be added to this function in the same format as stated under
             * 
            Keybindings:
            AddToWriteQueue("(key)keyName", "LeftShift");                                       value can be: Any KeyCode as a string e.g. "LeftShift"

            Options:
            AddToWriteQueue("(bool)boolName", "true");                                          value can be: ["true" | "false"]
            AddToWriteQueue("(int)intName", "27313");                                           value can be: any number as a string. For instance "123334"
            AddToWriteQueue("(slider)sliderName", "50|0|100");                                  value must be: "Default value|Min Value|MaxValue"
            AddToWriteQueue("(header)headerName", "Header Text");                               value can be: Any string
            AddToWriteQueue("(gap)gapName", "identifier");                                      value does not matter, just make name and value unique from other gaps

            ModInformation:
            AddToWriteQueue("(text)text1", "Descriptive text");                                  value can be: Any string
            */


            // Insert your options here \/

            ModMenu.Instance.configManager.InitConfig(gameObject.name, writeQueue);
            writeQueue.Clear();
        }

        public void AddToWriteQueue(string key, string value)
        {
            string[] splits = key.Remove(0,1).Split(')');
            if(splits[0] == "bool")
            {
                if (value == "true") value = "True";
                if (value == "false") value = "False";
            }
            writeQueue.Add(new Entry(splits[1], value, splits[0]));
        }

        public void AddEntryToWriteQueue(string key, string value, string type)
        {
            writeQueue.Add(new Entry(key, value, type));
        }

        public KeyCode GetKeyCode(string keyCode)
        {
            return ModMenu.Instance.configManager.GetModConfig(gameObject.name).GetKeyCode(keyCode.Split(')')[1]);
        }
        public KeyCode NewGetKeyCode(string keyCode)
        {
            return ModMenu.Instance.configManager.GetModConfig(gameObject.name).GetKeyCode(keyCode);
        }

        public bool GetTrueFalse(string boolName)
        {
            return ModMenu.Instance.configManager.GetModConfig(gameObject.name).GetBool(boolName.Split(')')[1]);
        }

        public bool NewGetTrueFalse(string boolName)
        {
            return ModMenu.Instance.configManager.GetModConfig(gameObject.name).GetBool(boolName);
        }

        public int GetSliderValue(string sliderName)
        {
            return ModMenu.Instance.configManager.GetModConfig(gameObject.name).GetSliderValue(sliderName.Split(')')[1]);
        }

        public int NewGetSliderValue(string sliderName)
        {
            return ModMenu.Instance.configManager.GetModConfig(gameObject.name).GetSliderValue(sliderName);
        }

        public int GetInt(string intName)
        {
            return ModMenu.Instance.configManager.GetModConfig(gameObject.name).GetInt(intName.Split(')')[1]);
        }

        public int NewGetInt(string intName)
        {
            return ModMenu.Instance.configManager.GetModConfig(gameObject.name).GetInt(intName);
        }
    }
}