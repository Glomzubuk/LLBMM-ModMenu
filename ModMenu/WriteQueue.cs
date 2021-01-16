using System.Collections.Generic;
using UnityEngine;

namespace LLModMenu
{
    public class WriteQueue
    {
        private List<LLModMenu.Entry> entryList;

        public WriteQueue()
        {
            entryList = new List<LLModMenu.Entry>();
        }

        public void Add(string key, string value)
        {
            string[] splits = key.Remove(0, 1).Split(')');
            EntryType type;
            switch (splits[0])
            {
                case "bool": type = EntryType.BOOLEAN; break;
                case "int": type = EntryType.NUMERIC; break;
                case "key": type = EntryType.KEY; break;
                case "gap": type = EntryType.GAP; break;
                case "slider": type = EntryType.SLIDER; break;
                case "string": type = EntryType.STRING; break;
                case "header": type = EntryType.HEADER; break;
                case "text": type = EntryType.TEXT; break;
                default: type = EntryType.TEXT; break;
            }
            AddEntry(key, value, type);
        }

        public void AddEntry(string key, string value, EntryType type)
        {
            if (type == EntryType.BOOLEAN)
            {
                if (value == "true") value = "True";
                if (value == "false") value = "False";
            }
            this.entryList.Add(new Entry(key, value, type));
        }

        public List<Entry> GetEntries()
        {
            return entryList;
        }

        public void Clear()
        {
            this.entryList.Clear();
        }
    }
}
