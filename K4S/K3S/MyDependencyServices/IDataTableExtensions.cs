using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDependencyServices
{
    public  interface IDataTableExtensions
    {
        List<T> ToModel<T>(DataTable dt);
        T RowToModel<T>(DataRow row);
        bool IsNull(DataTable dt);
    }
}
