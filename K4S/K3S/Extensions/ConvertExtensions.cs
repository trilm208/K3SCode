

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static class ConvertExtensions
    {


        public static string ToSafeString(this DateTime value)
        {
            return value.ToString("MM/dd/yyyy hh:mm:ss");
        }
    }
}
