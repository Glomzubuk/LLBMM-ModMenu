using System;
using UnityEngine;

namespace LLModMenu
{
    public class ModMenuIntegrationBase : MonoBehaviour
    {

        public Config config;
        protected ModMenu mm;
        protected bool mmAdded = false;

        protected void Start()
        {
            InitConfig();
        }

        protected void Update()
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

        virtual protected void InitConfig()
        {
            WriteQueue writeQueue = new WriteQueue();

            writeQueue.AddEntry("undefInitConfig", "To the mod Dev: You have not defined your InitConfig properly", EntryType.TEXT);

            this.config = ModMenu.Instance.configManager.InitConfig(gameObject.name, writeQueue);
            writeQueue.Clear();
        }


        public KeyCode OldGetKeyCode(string keyName)
        {
            return this.config.GetKeyCode(keyName.Split(')')[1]);
        }
        public KeyCode GetKeyCode(string keyName)
        {
            return this.config.GetKeyCode(keyName);
        }

        public bool OldGetTrueFalse(string boolName)
        {
            return this.config.GetBool(boolName.Split(')')[1]);
        }

        public bool GetTrueFalse(string boolName)
        {
            return this.config.GetBool(boolName);
        }

        public int OldGetSliderValue(string sliderName)
        {
            return this.config.GetSliderValue(sliderName.Split(')')[1]);
        }

        public int GetSliderValue(string sliderName)
        {
            return this.config.GetSliderValue(sliderName);
        }

        public int OldGetInt(string intName)
        {
            return this.config.GetInt(intName.Split(')')[1]);
        }

        public int GetInt(string intName)
        {
            return this.config.GetInt(intName);
        }
    }
}
