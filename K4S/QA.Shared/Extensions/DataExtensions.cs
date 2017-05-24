using System.Data;
using System.IO;
using System.Linq;

namespace System
{
    public static class DataExtensions
    {
        public static string Item(this DataRow row, int column)
        {
            if (row != null)
            {
                if (row.Table.Columns.Count > column)
                {
                    return row[column] + "";
                }
            }

            return string.Empty;
        }
    }
}