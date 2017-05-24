using System;
using System.Data;
using LoginXF.Droid;
using Xamarin.Forms;
using System.IO;
using System.IO.Compression;
using System.Net;
using MyDependencyServices;
using DataAccess;
using System.Collections.Generic;
using System.Xml;

[assembly: Dependency(typeof(XMLReader_Android))]
namespace LoginXF.Droid
{
    public class XMLReader_Android: IXmlReader
    {
        

        public Dictionary<string, string> LoadFromXML(string path)
        {
            Dictionary<string, string> Data = new Dictionary<string, string>();

            var doc = new XmlDocument();

            doc.Load(path);

            var entities = doc.SelectNodes("//Config/Entry");
            foreach (XmlNode node in entities)
            {
                var name = node.Attributes.GetNamedItem("Name");
                var value = node.Attributes.GetNamedItem("Value");

                if (name == null || value == null)
                    continue;

                if(Data.ContainsKey(name.Value))
                   {
                          Data[name.Value] = value.Value;
                   }
                   else
                   {
                          Data.Add(name.Value, value.Value);
                   }
            }

            return Data;
        }
    }
}