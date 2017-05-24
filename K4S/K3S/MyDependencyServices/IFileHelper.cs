using System;
namespace MyDependencyServices
{
    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);
        string ReadAllBytes(string path);
    }
}
