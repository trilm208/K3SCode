using System;
using System.Reflection;
using LoginXF.Droid;
using MyDependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(Assembly_Android))]
namespace LoginXF.Droid
{
    public class Assembly_Android : IAssemblyExtensions
    {
        public Assembly AssemblyLoadFrom(string fileName)
        {
            return Assembly.Load(fileName);
        }

        public  Assembly[] AssemblyList(string fileName)
        {
                var currentdomain = typeof(string).GetTypeInfo().Assembly.GetType("System.AppDomain").GetRuntimeProperty("CurrentDomain").GetMethod.Invoke(null, new object[] { });
                var getassemblies = currentdomain.GetType().GetRuntimeMethod("GetAssemblies", new Type[] { });
                Assembly[] assemblies = getassemblies.Invoke(currentdomain, new object[]{ }) as Assembly[];
                return assemblies;
         }
        }
}
