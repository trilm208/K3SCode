using System;
using System.Collections.Generic;

namespace MyDependencyServices
{
    public interface IXmlReader
    {
        Dictionary<string, string> LoadFromXML(string path);
    }
}
