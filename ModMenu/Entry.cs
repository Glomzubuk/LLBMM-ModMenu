
namespace LLModMenu
{
    public enum EntryType
    {
        NUMERIC,
        STRING,
        BOOLEAN,
        KEY,
        GAP,
        SLIDER,
        HEADER,
        TEXT,
    }

    public class Entry
    {
        public string Key;
        public string Value;
        public EntryType Type;
        public int Order;
        public Entry()
        {
        }

        public Entry(string key, string value, EntryType type = EntryType.TEXT, int order = -1)
        {
            Key = key;
            Value = value;
            Type = type;
            Order = order;
        }
    }
}
