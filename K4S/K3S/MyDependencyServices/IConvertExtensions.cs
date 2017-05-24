using K3S.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyDependencyServices
{
    public interface IConvertExtensions
    {
       
        DataTable ToTable<T>(List<T> listModel);
      
        string ImageToStringBase64( string  path);
      
    }


}
