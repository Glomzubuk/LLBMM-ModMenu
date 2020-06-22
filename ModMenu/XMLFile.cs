using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
using System.Xml.Serialization;


namespace LLModMenu
{
    public class XMLFile : IConfigFile
    {
        private static Dictionary<string, Mutex> fileMut = new Dictionary<string, Mutex>();

        private readonly string xmlPath;

        public XMLFile(string _xmlPath)
        {
            this.xmlPath = _xmlPath;
            if (!fileMut.ContainsKey(this.xmlPath))
            {
                Debug.Log("added " + this.xmlPath + "to Mutex file list");
                fileMut.Add(this.xmlPath, new Mutex());
            }
            if (!File.Exists(this.xmlPath)) this.Clear();

        }

        public void Clear()
        {
            Store(new List<Entry>());
        }

        public void Delete()
        {
            fileMut[this.xmlPath].WaitOne();
            File.Delete(this.xmlPath);
            fileMut[this.xmlPath].ReleaseMutex();
        }

        public List<Entry> Load()
        {
            fileMut[this.xmlPath].WaitOne();
            Debug.Log("Config File Read: " + this.xmlPath);
            TextReader reader = new StreamReader(this.xmlPath);
            XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
            List<Entry> entries = (List<Entry>)serializer.Deserialize(reader);
            reader.Close();
            fileMut[this.xmlPath].ReleaseMutex();
            return entries;
        }

        public void Store(List<Entry> config)
        {
            fileMut[this.xmlPath].WaitOne();
            Debug.Log("Config File Write: " + this.xmlPath);
            TextWriter writer = new StreamWriter(this.xmlPath);
            XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
            serializer.Serialize(writer, config);
            writer.Close();
            fileMut[this.xmlPath].ReleaseMutex();
        }

        public string GetPath()
        {
            return xmlPath;
        }
    }
}
