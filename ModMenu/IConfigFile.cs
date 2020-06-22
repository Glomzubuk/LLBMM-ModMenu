using System.Collections.Generic;
namespace LLModMenu
{
    public interface IConfigFile
    {
        List<Entry> Load();
        void Store(List<Entry> config);
        void Clear();
        void Delete();
        string GetPath();
    }
}
