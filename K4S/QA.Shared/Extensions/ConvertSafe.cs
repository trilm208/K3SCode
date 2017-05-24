using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Shared.Extensions
{
    public static class ConvertSafe
    {

        
        public static bool ToBoolean(object value)
        {
            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                return false;
            }
           
        }
        public static int ToInt32(object value)
        {
            if (value == null)
                return 0;

            if (value == null)
                return 0;

            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
            }

            return 0;
        }
      
    }
}
