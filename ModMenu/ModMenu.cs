using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LLScreen;
using LLGUI;
using LLHandlers;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace LLModMenu
{
    public class ModMenu : MonoBehaviour
    {
        private static ModMenu instance = null;
        public static ModMenu Instance { get { return instance; } }
        public static void Initialize() { GameObject gameObject = new GameObject("ModMenu"); ModMenu modLoader = gameObject.AddComponent<ModMenu>(); DontDestroyOnLoad(gameObject); instance = modLoader; ModMenuStyle.InitStyle();  }


        public LLButton button = null;
        public List<string> mods = new List<string>();
        public List<LLButton> modButtons = new List<LLButton>();
        public bool inModOptions = false;
        public bool inModSubOptions = false;
        public ScreenMenu mainmenu = null;
        public ScreenBase submenu = null;

        public string currentOpenMod = "";
        private string previousOpenMod = "";

        private bool sliderChange = false;
        private string keyToRebind = "";
        private bool rebindingKey = false;
        private readonly Array keyCodes = System.Enum.GetValues(typeof(KeyCode));
        public Vector2 keybindScrollpos = new Vector2(0,0);
        public Vector2 optionsScrollpos = new Vector2(0, 0);
        public Vector2 optionsTextpos = new Vector2(0, 0);

        public ModMenuIntegration MMI = null;

        public int switchInputModeTimer = 0;

        private string modVersion = "v1.1.0";
        private string iniLocation = Path.Combine(Path.GetDirectoryName(Application.dataPath), "ModSettings");
        public ConfigManager configManager = new ConfigManager(Path.Combine(Path.GetDirectoryName(Application.dataPath), "ModSettings"));


        private void Update()
        {
            if (this.sliderChange && Input.GetKeyUp(KeyCode.Mouse0))
            {
                Config modConfig = configManager.GetModConfig(this.currentOpenMod);
                modConfig.Save();
                this.sliderChange = false;
            }

            if (this.rebindingKey && Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in keyCodes)
                {
                    if (Input.GetKey(keyCode))
                    {
                        Config modConfig = configManager.GetModConfig(this.currentOpenMod);
                        modConfig.configKeys[this.keyToRebind] = keyCode;
                        modConfig.Save();
                        this.rebindingKey = false;
                    }
                }
            }

            if (!Directory.Exists(iniLocation)) Directory.CreateDirectory(iniLocation);
            if (mainmenu == null) { mainmenu = FindObjectOfType<ScreenMenu>(); }
            if (submenu == null) { submenu = UIScreen.currentScreens[1]; }
            else
            {

                List<LLButton> buttons = new List<LLButton>();
                if (submenu.screenType == ScreenType.MENU_OPTIONS && button == null)
                {
                    ScreenMenuOptions SMO = FindObjectOfType<ScreenMenuOptions>();
                    buttons.Add(SMO.btGame);
                    buttons.Add(SMO.btInput);
                    buttons.Add(SMO.btAudio);
                    buttons.Add(SMO.btVideo);
                    buttons.Add(SMO.btCredits);

                    button = Instantiate(buttons[4], buttons[4].transform, true);
                    button.name = "btMods";
                    button.SetText("mod settings");
                    button.onClick = new LLClickable.ControlDelegate(this.ModSettingsClick);

                    buttons.Add(button);

                    for(var i = 0; i < buttons.Count(); i++)
                    {
                        Vector3 scale = buttons[i].transform.localScale;

                        int modx = 2560;
                        int mody = 1440;

                        if (buttons[i] == button)
                        {
                            buttons[i].transform.localPosition = new Vector3(buttons[i].transform.localPosition.x - ((modx / 100) * 1), buttons[i].transform.localPosition.y - ((mody / 100) * 9.5f));
                        }
                        else if (buttons[i] != SMO.btGame)
                        {
                            buttons[i].transform.localPosition = new Vector3(SMO.btGame.transform.localPosition.x - (((modx / 100) * 0.55f) * i), SMO.btGame.transform.localPosition.y - (((mody / 100) * 5.3f) * i));
                            buttons[i].transform.localScale = new Vector3(scale.x * 0.85f, scale.y * 0.85f, scale.z);
                        }
                        else
                        {
                            buttons[i].transform.localPosition = new Vector3(-((modx / 100)*8.375f), ((mody / 100) * 12.625f));
                            buttons[i].transform.localScale = new Vector3(scale.x * 0.85f, scale.y * 0.85f, scale.z);
                        }
                    }
                }
            }


            if (switchInputModeTimer != 30) switchInputModeTimer++;

            if (inModSubOptions && this.currentOpenMod != this.previousOpenMod) //If we are within the options of a spiesific mod.
            {
                this.previousOpenMod = this.currentOpenMod;
                this.configManager.GetModConfig(this.currentOpenMod).LoadFromFile();
            }

            if (Controller.mouseKeyboard.GetButton(InputAction.ESC))
            {
                if (inModOptions)
                {
                    this.previousOpenMod = "";
                    UIScreen.Open(ScreenType.MENU_OPTIONS, 1, ScreenTransition.MOVE_RIGHT);
                    inModOptions = false;
                    inModSubOptions = false;
                }
            }
        }

        private void ModSettingsClick(int playerNr)
        {
            ScreenBase screen = UIScreen.Open(ScreenType.OPTIONS, 1);
            inModOptions = true;
            GameObject.Find("btQuit").GetComponent<LLButton>().onClick = new LLClickable.ControlDelegate(QuitClick);
            mainmenu.lbTitle.text = "MOD SETTINGS";

            var vcount = 0;
            var hcount = 0;

            foreach (string mod in mods)
            {
                var obj = Instantiate(button, screen.transform);
                obj.SetText(mod, 50);
                obj.transform.localScale = new Vector3(0.4f, 0.3f);
                obj.transform.position = new Vector3(-1.5f + (0.75f * hcount), 0.80f - (0.125f * vcount));
                obj.onClick = delegate (int pNr) { ModSubSettingsClick(mod); };
                modButtons.Add(obj);
                if (vcount < 12) { vcount++; } else { hcount++; vcount = 0; }
            }
        }

        private void QuitClick(int playerNr)
        {
            this.previousOpenMod = "";
            if (inModOptions == true)
            {
                UIScreen.Open(ScreenType.MENU_OPTIONS, 1);
            } else
            {
                if (submenu != null)
                {
                    if (submenu.screenType == ScreenType.MENU_MAIN)
                    {
                        DNPFJHMAIBP.GKBNNFEAJGO(Msg.QUIT, playerNr, -1);
                    }
                    else
                    {
                        DNPFJHMAIBP.GKBNNFEAJGO(Msg.BACK, playerNr, -1);
                    }
                }
            }
            if (UIScreen.currentScreens[1].screenType == ScreenType.MENU_OPTIONS)
            {
                mainmenu.lbTitle.text = "OPTIONS";
                inModOptions = false;
                inModSubOptions = false;
                AudioHandler.PlayMenuSfx(Sfx.MENU_BACK);
                AudioHandler.PlayMenuSfx(Sfx.MENU_CONFIRM);
            }
        }

        #region GUIStuff
        private void OnGUI()
        {
            var x1 = Screen.width / 6;
            var y1 = Screen.height / 10;
            var x2 = Screen.width - (Screen.width/6)*2;
            var y2 = Screen.height - (Screen.height / 6);
            GUI.Box(new Rect(10, 10, 20, 20), "ModMenu " + modVersion, ModMenuStyle.versionBox);


            if (inModSubOptions)
            {
                GUIContent guic = new GUIContent("   ModMenu " + modVersion + "   ");
                Vector2 calc = GUI.skin.box.CalcSize(guic);
                GUI.Box(new Rect(10, 10, calc.x, calc.y), "ModMenu " + modVersion, ModMenuStyle.versionBox);
                GUI.Window(0, new Rect(x1, y1, x2, y2 / 3), new GUI.WindowFunction(OpenKeybindsWindow), "Keybindings", ModMenuStyle.windStyle);
                GUI.Window(1, new Rect(x1, y1 + y2 / 3 + 10, x2, y2 / 3), new GUI.WindowFunction(OpenOptionsWindow), "Options", ModMenuStyle.windStyle);
                GUI.Window(2, new Rect(x1, y1 + ((y2 / 3 + 10)*2), x2, y2 / 5), new GUI.WindowFunction(OpenTextWindow), "Mod Information", ModMenuStyle.windStyle);
                GUI.skin.window = null;
            }
            GUI.skin.label.fontSize = 15;
        }

        private void ModSubSettingsClick(string modName)
        {
            inModSubOptions = true;
            currentOpenMod = modName;
            ScreenBase screen = UIScreen.Open(ScreenType.OPTIONS, 1);
            mainmenu.lbTitle.text = modName.ToUpper() + " SETTINGS";
        }


        private void OpenKeybindsWindow(int wId)
        {
            GUILayout.Space(30);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            keybindScrollpos = GUILayout.BeginScrollView(keybindScrollpos, false, true);

            Config modConfig = this.configManager.GetModConfig(currentOpenMod);

            foreach (KeyValuePair<string, KeyCode> entry in modConfig.configKeys)
            {
                string formatted = UppercaseFirst(Regex.Replace(entry.Key, "([a-z])([A-Z])", "$1 $2"));
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(formatted + ":", ModMenuStyle.labStyle);
                GUILayout.Space(10);

                string displayText;

                if (this.rebindingKey && this.keyToRebind == entry.Key)
                    displayText = "WAITING FOR KEY";
                else
                    displayText = entry.Value.ToString();

                if (GUILayout.Button("[" + displayText + "]", ModMenuStyle.button, GUILayout.MinWidth(100)))
                {
                    this.rebindingKey = true;
                    this.keyToRebind = entry.Key;
                }
                
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void OpenOptionsWindow(int wId)
        {
            GUILayout.Space(30);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            optionsScrollpos = GUILayout.BeginScrollView(optionsScrollpos, false, true);

            Config modConfig = this.configManager.GetModConfig(this.currentOpenMod);
            List<Entry> tmpOptionList = new List<Entry>(modConfig.optionList); // needed since we might modify the list during the loop

            foreach (Entry option in tmpOptionList)
            {
                if (option.Type == "bool")
                {
                    string key = option.Key;
                    bool val = modConfig.GetBool(key);
                    string formatted = UppercaseFirst(Regex.Replace(key, "([a-z])([A-Z])", "$1 $2"));

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(formatted + ":", ModMenuStyle.labStyle);
                    GUILayout.Space(10);

                    var str = "";
                    if (val) str = "Enabled";
                    else str = "Disabled";

                    bool isPressed = GUILayout.Button(str, ModMenuStyle.button, GUILayout.MinWidth(100));
                    if (isPressed)
                    {
                        modConfig.configBools[key] = !val;
                        modConfig.Save();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                else if (option.Type == "int")
                {
                    string key = option.Key;
                    int value = modConfig.GetInt(key);

                    string formatted = UppercaseFirst(Regex.Replace(key, "([a-z])([A-Z])", "$1 $2"));
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(formatted + ": ", ModMenuStyle.labStyle);
                    GUILayout.Box(value.ToString(), ModMenuStyle.box);
                    GUILayout.Space(10);

                    bool isMinusPressed = GUILayout.Button("  -  ", ModMenuStyle.button);
                    if (isMinusPressed)
                    {
                        modConfig.configInts[key] = value - 1;
                        modConfig.Save();
                    }

                    bool isPlusPressed = GUILayout.Button("  +  ", ModMenuStyle.button);
                    if (isPlusPressed)
                    {
                        modConfig.configInts[key] = value + 1;
                        modConfig.Save();
                    }


                    GUILayout.Space(30);

                    string intValue = GUILayout.TextField(value.ToString(), 10, ModMenuStyle._textFieldStyle, GUILayout.MinWidth(32));

                    bool isFromTextPressed = GUILayout.Button("Set Value From Textbox", ModMenuStyle.button);
                    if (Int32.TryParse(intValue, out int n))
                    {
                        modConfig.configInts[key] = n;
                        if (isFromTextPressed)
                        {
                            modConfig.Save();
                        }
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                else if (option.Type == "slider")
                {

                    string key = option.Key;
                    string value = modConfig.configSliders[key];
                    int storedSliderValue = modConfig.GetSliderValue(key);

                    string formatted = UppercaseFirst(Regex.Replace(key, "([a-z])([A-Z])", "$1 $2"));

                    string[] valMinMax = value.Split('|');
                    float sliderValue = storedSliderValue;

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(formatted + ": ", ModMenuStyle.labStyle);
                    sliderValue = GUILayout.HorizontalSlider(sliderValue, float.Parse(valMinMax[1]), float.Parse(valMinMax[2]), ModMenuStyle._sliderBackgroundStyle, ModMenuStyle._sliderThumbStyle, GUILayout.Width(300));
                    GUILayout.Box(System.Math.Round(Double.Parse(sliderValue.ToString())).ToString(), ModMenuStyle.box);

                    int newSliderValue = (int)System.Math.Round(sliderValue);
                    if (newSliderValue != storedSliderValue)
                    {
                        this.sliderChange = true;
                        modConfig.configSliders[key] = newSliderValue + "|" + valMinMax[1] + "|" + valMinMax[2];
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                else if (option.Type == "header")
                {
                    string key = option.Key;
                    string value = modConfig.configHeaders[key];

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Box(value, ModMenuStyle.headerBox);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                else if (option.Type == "gap")
                {
                    string key = option.Key;
                    int value = modConfig.configGaps[key];

                    GUILayout.Space(value);
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void OpenTextWindow(int wId)
        {
            GUILayout.Space(30);
            GUILayout.BeginVertical();
            optionsTextpos = GUILayout.BeginScrollView(optionsTextpos, false, true);


            Config modConfig = this.configManager.GetModConfig(currentOpenMod);

            if (modConfig.configText.Count > 0)
            {
                foreach (KeyValuePair<string, string> keyval in modConfig.configText)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(keyval.Value, ModMenuStyle.readStyle);
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }
        #endregion
    }
}
