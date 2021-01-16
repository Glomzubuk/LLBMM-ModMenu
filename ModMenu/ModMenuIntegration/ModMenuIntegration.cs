// Script used to connect to ModMenu
using LLModMenu;

namespace YourMod
{
    public class ModMenuIntegration : ModMenuIntegrationBase
    {

        override protected void  InitConfig()
        {
            WriteQueue writeQueue = new WriteQueue();
            /*
             * Mod menu now uses a single function to add options etc. (either Add or AddEntry)
             * your specified options should be added to this function in the same format as stated under
             * 
             * Old notation is supported for easier compatibility, but please consider switching to the new one.
             * Uncomment the blocks as needed.            
             */
            /*
            // Old Notation:
            // You Have to use the "Old" methods to use this, like "OldGetTrueFalse"
            // Note that KeyCodes accesses will be broken, you should now use them like the others and call OldGetKeyCode("(key)yourkey")
            // Keybindings:
            writeQueue.Add("(key)keyName", "LeftShift");         // value can be: Any KeyCode as a string e.g. "LeftShift"

            // Options:
            writeQueue.Add("(bool)boolName", "true");            // value can be: ["true" | "false"]
            writeQueue.Add("(int)intName", "27313");             // value can be: any number as a string. For instance "123334"
            writeQueue.Add("(slider)sliderName", "50|0|100");    // value must be: "Default value|Min Value|MaxValue"
            writeQueue.Add("(header)headerName", "Header Text"); // value can be: Any string
            writeQueue.Add("(gap)gapName", "identifier");        // value can be: any number representing the size of the gap

            // ModInformation:
            writeQueue.Add("(text)text1", "Descriptive text");   // value can be: Any string
            */
            /*
            // New Notation:  
            // Keybindings:
            writeQueue.AddEntry("keyName", "A", EntryType.KEY);                  // value can be: Any KeyCode as a string e.g. "LeftShift"

            // Options:
            writeQueue.AddEntry("boolName", "true", EntryType.BOOLEAN);          // value can be: ["true" | "false"]
            writeQueue.AddEntry("intName", "27313", EntryType.NUMERIC);          // value can be: any number as a string. For instance "123334"
            writeQueue.AddEntry("sliderName", "50|0|100", EntryType.SLIDER);     // value must be: "Default value|Min Value|MaxValue"
            writeQueue.AddEntry("headerName", "Header Text", EntryType.HEADER);  // value can be: Any string
            writeQueue.AddEntry("gapName", "20", EntryType.GAP);                 // value can be: any number representing the size of the gap

            // ModInformation:
            writeQueue.AddEntry("textLabel", "Descriptive text", EntryType.TEXT);//value can be: Any string
            */

            // Insert your options here \/

            this.config = ModMenu.Instance.configManager.InitConfig(gameObject.name, writeQueue);
            writeQueue.Clear();
        }
    }
}