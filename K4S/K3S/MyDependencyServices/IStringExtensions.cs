using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyDependencyServices
{
    public  interface IStringExtensions
    {
         
         Byte[] StringBase64ToByteArray(String s);

         string GetCurrentAppDomain();
    }
}
