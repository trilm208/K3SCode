using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDependencyServices
{
    public  interface IDataSetExtension

    {
        bool IsNull(DataSet ds);
    }
}
