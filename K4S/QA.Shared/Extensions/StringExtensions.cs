using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Data;

namespace System
{
    public static class StringExtensions
    {
            public static bool IsEmpty(this string value)
            {
            	if (value == null)
            		return true;

            	return value.Trim().Length == 0;
            }
            public static bool IsNotEmpty(this string value)
            {
                	return !value.IsEmpty();
             }
                public static bool IsValidEmail(this string value)
                {
                	// source: http://thedailywtf.com/Articles/Validating_Email_Addresses.aspx
                	string pattern = @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$";
                	return Regex.IsMatch(value, pattern);
                  }

        public static DataTable JsonStringToDataTable(this string jsonString)
        {
        	if (jsonString == null || jsonString == "null")
        	{
        		return null;
        	}
        	    return (DataTable)JsonConvert.DeserializeObject(jsonString,(typeof(DataTable)));
                }
            }
}
