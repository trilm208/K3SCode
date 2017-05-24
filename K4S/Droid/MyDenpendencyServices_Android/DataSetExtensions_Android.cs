using System;
using System.Data;
using LoginXF.Droid;
using MyDependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(DataSetExtensions_Android))]
namespace LoginXF.Droid
{
    public class DataSetExtensions_Android : IDataSetExtension
    {
        public bool IsNull(DataSet ds)
        {
            if(ds==null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
