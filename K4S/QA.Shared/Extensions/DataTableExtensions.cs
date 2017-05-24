using System;
using System.Data;
using Newtonsoft.Json;

namespace System
{
    public static class DataTableExtensions
    {
        public static string DataTableToJSON(this DataTable dt)
        {
            return JsonConvert.SerializeObject(dt);
        }
        public static int _FindIndex(this DataTable _table, string columnName, string value)
        {
            try
            {
            	if ( _table.Rows.Count == 0)
            		return -2;
            	for (int i = 0; i < _table.Rows.Count; i++)
            	{
                        if (_table.Rows[i][columnName].ToString().Trim() == value.Trim())
                		{
                			return i;
                		}
            	}
            	return -1;
            }
            catch
            {
                return -2;
            }
        }

        public static DataRow FirstRow(this DataTable dataTable)
        {
          
            try
            {
        	    return dataTable.Rows[0];
            }
            catch 
            {
                return null;
            }
       }
    }
}
