using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDependencyServices
{
    public interface IListExtensions
    {
        List<T> Shuffle<T>(List<T> list);
        DataTable ToTable<T>(List<T> list);
    }
}
