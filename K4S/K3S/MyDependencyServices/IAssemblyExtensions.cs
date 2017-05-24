using System;
using System.Reflection;

namespace MyDependencyServices
{
        public interface IAssemblyExtensions

        {
                Assembly AssemblyLoadFrom(string fileName);
                Assembly[] AssemblyList(string fileName);
        }
}
