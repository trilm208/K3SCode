

using LoginXF.Droid;
using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using Xamarin.Forms;
using MyDependencyServices;
using System.Reflection;

[assembly: Dependency(typeof(StringExtensions_Android))]
namespace LoginXF.Droid
{
    public class StringExtensions_Android : IStringExtensions
    {
        public string GetCurrentAppDomain()
        {
            return Directory.GetParent((new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath).FullName;
           

        }

        public byte[] StringBase64ToByteArray(string s)
        {
            return null;
        }
    }
}