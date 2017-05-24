using System;
using System.IO;
using K3S.Droid;
using MyDependencyServices;
using Xamarin.Forms;


[assembly: Dependency(typeof(FileHelper))]
namespace K3S.Droid
{
	public class FileHelper : IFileHelper
	{
		public string GetLocalFilePath(string filename)
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			return Path.Combine(path, filename);
		}

        public string ReadAllBytes(string path)
        {
            try
            {
                var raw = System.IO.File.ReadAllBytes(path);
                var Data = Convert.ToBase64String(raw);
                return Data;
            }
            catch(Exception ex)
            {
                return "";
            }
        }
    }
}
