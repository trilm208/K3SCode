using System;
using System.Data;
using LoginXF.Droid;
using MyDependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileExtensions_Android))]
namespace LoginXF.Droid
{
public class FileExtensions_Android : IFileExtensions
    {
        public bool IsExists(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }
    }
}
