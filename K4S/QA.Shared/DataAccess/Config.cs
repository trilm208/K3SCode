using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace DataAccess
{
    class Config
    {
        Dictionary<string, string> Data = new Dictionary<string, string>();
        static string DefaultPath = typeof(string).GetTypeInfo().Assembly.GetType("AppDomain").GetRuntimeProperty("CurrentDomain").GetMethod.Invoke(null, new object[] { })+ "Config.xml";


        public string this[string key]
        {
            get
            {
                key = key.ToLower();

                if (Data.ContainsKey(key))
                    return Data[key];

                return string.Empty;
            }
            set
            {
                key = key.ToLower();

                if (Data.ContainsKey(key))
                    Data[key] = value;
                else
                    Data.Add(key, value);
            }
        }


        public static Config Load()
        {
            return Load(DefaultPath);
        }


        private static Config Load(string path)
        {
            var result = new Config();

            try
            {
                //var doc = new XmlDocument();

                //doc.Load(path);

                //var entities = doc.SelectNodes("//Config/Entry");
                //foreach (XmlNode node in entities)
                //{
                //    var name = node.Attributes.GetNamedItem("Name");
                //    var value = node.Attributes.GetNamedItem("Value");

                //    if (name == null || value == null)
                //        continue;

                //    result[name.Value] = value.Value;
                //}
            }
            catch
            {
            }

            return result;
        }


        public void Save()
        {
            Save(DefaultPath);
        }


        private void Save(string path)
        {
            try
            {
                //var doc = new XmlDocument();

                //doc.LoadXml("<Config></Config>");
                //var root = doc.SelectSingleNode("//Config");

                //foreach (var item in Data)
                //{
                //    var entry = doc.CreateElement("Entry");
                //    var name = doc.CreateAttribute("Name");
                //    var value = doc.CreateAttribute("Value");

                //    name.Value = item.Key;
                //    value.Value = item.Value;

                //    entry.Attributes.Append(name);
                //    entry.Attributes.Append(value);
                //    root.AppendChild(entry);
                //}

                //doc.Save(path);
            }
            catch
            {
            }
        }
    }
}